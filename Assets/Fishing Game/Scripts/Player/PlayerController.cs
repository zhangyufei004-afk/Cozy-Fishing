using UnityEngine;
using UnityEngine.InputSystem;
using FishingGame.GameManagement;
using Unity.Cinemachine;
using FishingGame.Reeling;
using TMPro;
using System.Collections;

namespace FishingGame.Player
{
    /// <summary>
    /// Player movement, grappling (manual control via W/S), and animation integration.
    /// Grapple plays player throw animation, then spawns rope after a short delay.
    /// Rope automatically conforms to terrain to prevent clipping.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        private static readonly int ThrowTrigger = Animator.StringToHash("ThrowTrigger");
        private static readonly int IsGrappling = Animator.StringToHash("IsGrappling");
        private readonly int _speed = Animator.StringToHash("Speed");

        [Header("Player Core")]
        [SerializeField] private GameObject playerBody;
        [SerializeField] private GameObject playerMesh;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Animator animator;

        [Header("Movement Settings")]
        [SerializeField] private float movementSpeed = 6f;
        [SerializeField] private float rotationSpeed = 20f;

        [Header("Grapple Settings")]
        [SerializeField] private float grappleRange = 15f;
        [SerializeField] private float grappleSpeed = 10f;
        [SerializeField] private float grappleAscendSpeed = 3f;
        [SerializeField] private float grappleDescendSpeed = 3f;
        [SerializeField] private bool hasGrapple = true;
        [SerializeField] private float rotateToGrappleSpeed = 8f;

        [Header("Grapple Delay")]
        [SerializeField, Tooltip("Delay before rope spawns after throw animation")]
        private float grappleStartDelay = 0.15f;

        [Header("Grapple Visuals")]
        [SerializeField] private LineRenderer grappleLine;
        [SerializeField] private Transform grappleTip;
        [SerializeField] private TextMeshProUGUI grapplePromptText;

        [Header("Environment Layers")]
        [SerializeField] private LayerMask waterLayerMask;
        [SerializeField] private LayerMask terrainLayerMask;

        // === Private Fields ===
        private Vector2 _moveInput;
        private float _initialMovementSpeed;
        private float _initialRotationSpeed;

        private bool _isCurrentlyEngaged;
        private bool _isGrappling;
        private bool _isInGrappleZone;
        private bool _isThrowing;

        private Vector3 _grappleAnchor;
        private Vector3 _grappleDestination;
        private float _currentRopeLength;

        private bool _isDead;
        private Vector3 _previousSafePlace;
        private int _raycastLayerMask;

        private GrappleZone _currentZone;

        // Input Actions
        private InputAction _grappleUpAction;
        private InputAction _grappleDownAction;
        private InputAction _grappleCancelAction;

        private void OnEnable()
        {
            InputActionAsset inputActions = InputSystem.actions;
            InputActionMap playerActionMap = inputActions.FindActionMap("Player");
            playerActionMap.Enable();

            playerActionMap.FindAction("Move").performed += Move;
            playerActionMap.FindAction("Move").canceled += CancelMove;
            playerActionMap.FindAction("Crouch").performed += ToggleGrapple;
            playerActionMap.FindAction("Interact").performed += AttemptToPickupItem;

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
            GameManager.Instance.GameEvents.OnBecomeOccupied += engaged => _isCurrentlyEngaged = engaged;
            GameManager.Instance.GameEvents.OnPlayerDeathScreenActive += RespawnPlayer;

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
            HandleGrappleRotation();
        }

        private void Movement()
        {
            if (!_isGrappling && !_isThrowing)
            {
                Vector3 direction = Vector3.ClampMagnitude(new Vector3(_moveInput.x, 0, _moveInput.y), 1);
                characterController.SimpleMove(direction * movementSpeed);

                float animSpeed = Mathf.Clamp(characterController.velocity.magnitude / 2f, 0, 2f);
                animator.SetFloat(_speed, animSpeed);

                if (_moveInput != Vector2.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                    playerBody.transform.rotation = Quaternion.Slerp(playerBody.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
            }
            else
            {
                animator.SetFloat(_speed, 0);
            }
        }

        // === Grapple ===
        private void ToggleGrapple(InputAction.CallbackContext context)
        {
            if (!context.performed || !hasGrapple || _isCurrentlyEngaged)
                return;

            if (_isGrappling || _isThrowing)
            {
                CancelGrapple();
                return;
            }

            if (_isInGrappleZone && _currentZone?.anchorPoint)
            {
                _isThrowing = true;
                GameManager.Instance.GameEvents.SetPlayerOccupied(true);

                if (animator is not null)
                {
                    animator.ResetTrigger("CastTrigger");
                    animator.SetTrigger(ThrowTrigger);
                    animator.SetBool(IsGrappling, true);
                    Debug.Log("[PlayerController] Player throw animation triggered.");
                }

                StartCoroutine(StartGrappleAfterDelay());
            }
        }

        private IEnumerator StartGrappleAfterDelay()
        {
            _grappleDestination = _currentZone.destinationPoint?.position ?? _currentZone.anchorPoint.position;
            GameManager.Instance.GameEvents.ToggleGrappleMode(true, _grappleDestination);
            
            yield return new WaitForSeconds(grappleStartDelay);

            _grappleAnchor = _currentZone.anchorPoint.position;

            _currentRopeLength = Vector3.Distance(transform.position, _grappleAnchor);
            _isGrappling = true;
            _isThrowing = false;

            Debug.Log($"[PlayerController] Grapple started after {grappleStartDelay}s delay.");
        }

        // === Manual W/S control toward destination ===
        private void GrappleBehaviour()
        {
            if (!_isGrappling) return;

            float verticalInput = 0f;
            if (_grappleUpAction != null && _grappleUpAction.IsPressed()) verticalInput = 1f;
            else if (_grappleDownAction != null && _grappleDownAction.IsPressed()) verticalInput = -1f;

            if (verticalInput != 0f)
            {
                Vector3 toDestination = (_grappleDestination - transform.position).normalized;
                float moveSpeed = verticalInput > 0 ? grappleAscendSpeed : grappleDescendSpeed;

                Vector3 moveDelta = toDestination * (moveSpeed * Time.deltaTime * verticalInput);
                characterController.Move(moveDelta);
            }

            float distToDest = Vector3.Distance(transform.position, _grappleDestination);
            if (distToDest < 1.2f)
            {
                CancelGrapple();
                Debug.Log("[PlayerController] Reached destination point.");
            }
        }

        private void HandleGrappleRotation()
        {
            if (_isGrappling)
            {
                Vector3 direction = (_grappleAnchor - playerBody.transform.position).normalized;
                direction.y = 0;
                if (direction != Vector3.zero)
                {
                    playerBody.transform.rotation = 
                        Quaternion.Lerp(playerBody.transform.rotation, Quaternion.LookRotation(direction, Vector3.up), 
                            rotateToGrappleSpeed * Time.deltaTime);
                }
            }
        }

        private void HandlePromptUI()
        {
            if (grapplePromptText is null) return;
            grapplePromptText.enabled = _isInGrappleZone && hasGrapple && !_isGrappling && !_isThrowing;
            if (grapplePromptText.enabled)
                grapplePromptText.text = "Press Ctrl to Grapple";
        }

        private void CancelGrapple()
        {
            if (_isGrappling)
            {
                _isGrappling = false;
                _isThrowing = false;
                GameManager.Instance.GameEvents.SetPlayerOccupied(false);
                Debug.Log("[PlayerController] Grapple canceled.");
                animator.SetBool(IsGrappling, false);
                GameManager.Instance.GameEvents.ToggleGrappleMode(false, Vector3.zero);
            }
        }

        private void AttemptToPickupItem(InputAction.CallbackContext context)
        {
            GameManager.Instance.GameEvents.AttemptItemPickup();
        }

        private void Move(InputAction.CallbackContext context) => _moveInput = context.ReadValue<Vector2>();
        private void CancelMove(InputAction.CallbackContext context) => _moveInput = Vector2.zero;

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
        public void DisableGrapple() => hasGrapple = false;

        public void ToggleMovement(bool enabled)
        {
            if (enabled)
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
