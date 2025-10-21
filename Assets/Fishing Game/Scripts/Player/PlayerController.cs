using UnityEngine;
using UnityEngine.InputSystem;
using FishingGame.GameManagement;
using Unity.Cinemachine;
using FishingGame.Reeling;
using TMPro;

namespace FishingGame.Player
{
    /// <summary>
    /// Player Movement Controller.
    /// Handles player movement, grappling, interactions, water detection, and death using CharacterController and the new Input System.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        private readonly int _speed = Animator.StringToHash("Speed");

        [Header("Player Core")]
        [SerializeField] private GameObject playerBody;
        [SerializeField] private GameObject playerMesh;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Animator animator;
        [SerializeField] private CinemachineBrain brain;

        [Header("Movement Settings")]
        [SerializeField] private float movementSpeed;
        [SerializeField] private float rotationSpeed;

        [Header("Grapple Settings")]
        [SerializeField] private float grappleRange = 15f;
        [SerializeField] private float grappleSpeed = 10f;
        [SerializeField] private float grappleAscendSpeed = 3f;
        [SerializeField] private float grappleDescendSpeed = 3f;
        [SerializeField] private bool hasGrapple;

        [Header("Grapple Visuals")]
        [SerializeField] private LineRenderer grappleLine;
        [SerializeField] private Transform grappleTip;
        [SerializeField] private TextMeshProUGUI grapplePromptText;
        [SerializeField, Range(3, 20)] private int ropeSegmentCount = 8;
        [SerializeField] private float ropeElasticity = 0.15f;

        [Header("Fishing Reference")]
        [SerializeField] private FishingRod currentFishingRod;

        [Header("Environment Layers")]
        [SerializeField] private LayerMask waterLayerMask;
        [SerializeField] private LayerMask terrainLayerMask;

        // === Private Fields ===
        private Vector2 _moveInput;
        private float _initialMovementSpeed;
        private float _initialRotationSpeed;

        private bool _isCurrentlyEngaged;
        private bool _grappleMode;
        private bool _isGrappling;
        private Vector3 _grappleStart;
        private Vector3 _grappleAnchor;
        private float _currentRopeLength;
        private bool _isInGrappleZone;

        private bool _isDead;
        private Vector3 _previousSafePlace;
        private int _raycastLayerMask;

        // Grapple Zone reference
        private GrappleZone _currentZone;

        // Input Actions
        private InputAction _grappleUpAction;
        private InputAction _grappleDownAction;
        private InputAction _grappleCancelAction;

        // Rope positions for LineRenderer
        private Vector3[] _ropePositions;

        // === Properties ===
        public bool HasGrappleHook => hasGrapple;
        public FishingRod CurrentFishingRod => currentFishingRod;
        public bool IsInGrappleZone => _isInGrappleZone;

        private void OnEnable()
        {
            InputActionAsset inputActions = InputSystem.actions;
            InputActionMap playerActionMap = inputActions.FindActionMap("Player");
            playerActionMap.Enable();

            playerActionMap.FindAction("Move").performed += Move;
            playerActionMap.FindAction("Move").canceled += CancelMove;
            playerActionMap.FindAction("Crouch").performed += ToggleCrouch;
            playerActionMap.FindAction("Interact").performed += AttemptToPickupItem;

            // Grapple Up / Down / Cancel
            _grappleUpAction = playerActionMap.FindAction("GrappleUp");
            _grappleDownAction = playerActionMap.FindAction("GrappleDown");
            _grappleCancelAction = playerActionMap.FindAction("GrappleCancel");
            if (_grappleUpAction != null) _grappleUpAction.Enable();
            if (_grappleDownAction != null) _grappleDownAction.Enable();
            if (_grappleCancelAction != null)
            {
                _grappleCancelAction.performed += ctx => CancelGrapple();
                _grappleCancelAction.Enable();
            }

            _initialMovementSpeed = movementSpeed;
            _initialRotationSpeed = rotationSpeed;

            GameManager.Instance.GameEvents.OnTogglePlayerMovement += ToggleMovement;
            GameManager.Instance.GameEvents.OnBecomeOccupied += isEngaged => _isCurrentlyEngaged = isEngaged;
            GameManager.Instance.GameEvents.OnPlayerDeathScreenActive += RespawnPlayer;

            // Subscribe to all GrappleZones
            GrappleZone[] zones = FindObjectsOfType<GrappleZone>();
            foreach (var zone in zones)
            {
                zone.OnPlayerEntered += OnEnterZone;
                zone.OnPlayerExited += OnExitZone;
            }

            _raycastLayerMask = waterLayerMask | terrainLayerMask;
            _previousSafePlace = Vector3.zero;
        }

        private void OnEnterZone(GrappleZone zone)
        {
            _currentZone = zone;
            _isInGrappleZone = true;
        }

        private void OnExitZone(GrappleZone zone)
        {
            if (_currentZone == zone)
            {
                _currentZone = null;
                _isInGrappleZone = false;
            }
        }

        private void Update()
        {
            if (!_isDead && IsInWater())
            {
                _isDead = true;
                GameManager.Instance.GameEvents.PlayerDied();
                characterController.SimpleMove(Vector3.zero);
                animator.SetFloat(_speed, 0);
            }

            if (_isDead) return;

            if (CanWalkInDirection(Mathf.Abs(characterController.velocity.y)) && _previousSafePlace == Vector3.zero)
                _previousSafePlace = transform.position;

            Movement();
            GrappleBehaviour();
            HandlePromptUI();
            HandleLineRenderer();
            HandleGrappleRotation();
        }

        // === Movement ===
        private void Movement()
        {
            if (!_grappleMode && !_isGrappling)
            {
                Vector3 directionNormalized = Vector3.ClampMagnitude(new Vector3(_moveInput.x, 0, _moveInput.y), 1);
                characterController.SimpleMove(directionNormalized * movementSpeed);

                float animationSpeed = Mathf.Clamp(characterController.velocity.magnitude / 2f, 0, 2f);
                animator.SetFloat(_speed, animationSpeed);

                if (_moveInput != Vector2.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionNormalized, Vector3.up);
                    playerBody.transform.rotation = Quaternion.Slerp(playerBody.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
            }
            else
            {
                animator.SetFloat(_speed, 0);
            }
        }

        // === Grapple ===
        private void GrappleBehaviour()
        {
            if (!hasGrapple) return;

            // Fire when grapple mode is activated
            if (_grappleMode && !_isGrappling && _isInGrappleZone && _currentZone?.anchorPoint != null)
            {
                _grappleAnchor = _currentZone.anchorPoint.position;
                _isGrappling = true;
                _grappleStart = transform.position;
                _currentRopeLength = Vector3.Distance(_grappleStart, _grappleAnchor);

                // Player becomes occupied when grapple starts
                GameManager.Instance.GameEvents.SetPlayerOccupied(true);
            }

            if (_isGrappling)
            {
                Vector3 direction = (_grappleAnchor - transform.position).normalized;

                float verticalInput = 0f;
                if (_grappleUpAction != null && _grappleUpAction.IsPressed()) verticalInput = 1f;
                else if (_grappleDownAction != null && _grappleDownAction.IsPressed() && !_isGrounded()) verticalInput = -1f;

                _currentRopeLength -= verticalInput * Time.deltaTime *
                                      (verticalInput > 0 ? grappleAscendSpeed : grappleDescendSpeed);
                _currentRopeLength = Mathf.Clamp(_currentRopeLength, 1f, grappleRange);

                Vector3 targetPosition = _grappleAnchor - direction * _currentRopeLength;

                if (Physics.Raycast(targetPosition + Vector3.up * 0.1f, Vector3.down, out RaycastHit hitInfo, 5f, terrainLayerMask))
                {
                    float minHeight = hitInfo.point.y + 0.1f;
                    if (targetPosition.y < minHeight)
                        targetPosition.y = minHeight;
                }

                characterController.Move((targetPosition - transform.position) * (grappleSpeed * Time.deltaTime));

                // Grapple ends: release occupied state
                if (Vector3.Distance(transform.position, _grappleAnchor) < 1f)
                {
                    _isGrappling = false;
                    _grappleMode = false;
                    GameManager.Instance.GameEvents.SetPlayerOccupied(false);
                }
            }
        }

        // === Player auto face grapple point ground ===
        private void HandleGrappleRotation()
        {
            if (_isGrappling || _grappleMode)
            {
                Vector3 target = _grappleAnchor;
                if (Physics.Raycast(_grappleAnchor + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 10f, terrainLayerMask))
                {
                    target.y = hit.point.y;
                }

                Vector3 lookDirection = (target - transform.position).normalized;
                lookDirection.y = 0;
                if (lookDirection != Vector3.zero)
                    playerBody.transform.rotation = Quaternion.Slerp(playerBody.transform.rotation, Quaternion.LookRotation(lookDirection), rotationSpeed * Time.deltaTime);
            }
        }

        // === Grapple Prompt UI ===
        private void HandlePromptUI()
        {
            if (grapplePromptText == null) return;

            grapplePromptText.enabled = _isInGrappleZone && hasGrapple;
            if (_isInGrappleZone && hasGrapple)
                grapplePromptText.text = "Press Ctrl to Grapple";
        }

        // === Grapple Line Renderer with Elasticity & Collision ===
        private void HandleLineRenderer()
        {
            if (grappleLine == null || grappleTip == null) return;

            if (_isGrappling)
            {
                if (!grappleLine.enabled) grappleLine.enabled = true;

                if (_ropePositions == null || _ropePositions.Length != ropeSegmentCount)
                    _ropePositions = new Vector3[ropeSegmentCount];

                Vector3 startPoint = grappleTip.position;
                Vector3 endPoint = _grappleAnchor;

                _ropePositions[0] = startPoint;
                _ropePositions[_ropePositions.Length - 1] = endPoint;

                for (int i = 1; i < _ropePositions.Length - 1; i++)
                {
                    float t = (float)i / (_ropePositions.Length - 1);
                    Vector3 targetPos = Vector3.Lerp(startPoint, endPoint, t);
                    float sag = Mathf.Sin(t * Mathf.PI) * 0.3f;
                    targetPos.y -= sag;

                    _ropePositions[i] = Vector3.Lerp(_ropePositions[i], targetPos, ropeElasticity);

                    if (Physics.Raycast(_ropePositions[i] + Vector3.up * 0.1f, Vector3.down, out RaycastHit hitInfo, 5f, terrainLayerMask))
                    {
                        _ropePositions[i].y = Mathf.Max(_ropePositions[i].y, hitInfo.point.y + 0.05f);
                    }
                }

                grappleLine.positionCount = _ropePositions.Length;
                grappleLine.SetPositions(_ropePositions);
            }
            else if (grappleLine.enabled)
            {
                grappleLine.enabled = false;
            }
        }

        // === Input Handlers ===
        private void ToggleCrouch(InputAction.CallbackContext context)
        {
            if (context.performed && hasGrapple && !_isCurrentlyEngaged && _isInGrappleZone)
            {
                _grappleMode = !_grappleMode;

                // Auto-fire the grapple when entering grapple mode inside a valid zone
                if (_grappleMode && !_isGrappling && _currentZone?.anchorPoint != null)
                {
                    _isGrappling = false;
                }
            }
        }

        private void AttemptToPickupItem(InputAction.CallbackContext context)
        {
            GameManager.Instance.GameEvents.AttemptItemPickup();
        }

        private void Move(InputAction.CallbackContext context) => _moveInput = context.ReadValue<Vector2>();
        private void CancelMove(InputAction.CallbackContext context) => _moveInput = Vector2.zero;

        private void CancelGrapple()
        {
            _isGrappling = false;
            _grappleMode = false;

            // Reset occupied state when grapple canceled
            GameManager.Instance.GameEvents.SetPlayerOccupied(false);
        }

        // === Utility ===
        private bool _isGrounded() => characterController.isGrounded;

        private bool IsInWater()
        {
            return Physics.Raycast(transform.position + Vector3.up * 1.5f, Vector3.down, 0.2f, waterLayerMask);
        }

        private bool CanWalkInDirection(float yVelocityAbs)
        {
            Vector3 startPositionOffset = transform.position + playerBody.transform.forward * 3.5f + Vector3.up * 2f;
            if (Physics.Raycast(startPositionOffset, Vector3.down, out RaycastHit hitInfo, 20f, _raycastLayerMask))
            {
                if (1 << hitInfo.transform.gameObject.layer == waterLayerMask || yVelocityAbs > 0.25f)
                    return false;

                _previousSafePlace = Vector3.zero;
            }
            return true;
        }

        public void EnableGrapple() => hasGrapple = true;

        public void DisableGrapple()
        {
            hasGrapple = false;
            _grappleMode = false;
        }

        public void ToggleMovement(bool isMovementEnabled)
        {
            if (isMovementEnabled)
            {
                movementSpeed = _initialMovementSpeed;
                rotationSpeed = _initialRotationSpeed;
            }
            else
            {
                movementSpeed = 0;
                rotationSpeed = 0;
            }
        }

        public void RespawnPlayer(bool deathScreenActive)
        {
            switch (_isDead)
            {
                case true when deathScreenActive:
                    characterController.enabled = false;
                    transform.position = _previousSafePlace;
                    animator.SetFloat(_speed, 0);
                    _previousSafePlace = Vector3.zero;
                    characterController.enabled = true;
                    ToggleMovement(false);
                    GameManager.Instance.GameEvents.SetPlayerOccupied(true);
                    break;

                case true when !deathScreenActive:
                    ToggleMovement(true);
                    GameManager.Instance.GameEvents.SetPlayerOccupied(false);
                    _isDead = false;
                    break;

                default:
                    Debug.LogError("Attempted to respawn player while not dead!");
                    break;
            }
        }
    }
}
