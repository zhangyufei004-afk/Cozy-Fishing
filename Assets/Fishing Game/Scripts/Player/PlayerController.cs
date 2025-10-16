using Unity.Burst;
using UnityEngine;
using UnityEngine.InputSystem;
using FishingGame.GameManagement;
using Unity.Cinemachine;
using FishingGame.Reeling;
using TMPro;

using Cursor = UnityEngine.Cursor;

namespace FishingGame.Player
{
    /// <summary>
    /// Player Movement Controller.
    /// Handles movement, grappling, and interactions using CharacterController and the new Input System.
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
        [SerializeField] private GameObject fpsCamera;

        [Header("Movement Settings")]
        [SerializeField] private float movementSpeed;
        [SerializeField] private float rotationSpeed;

        [Header("Grapple Settings")]
        [SerializeField] private LayerMask rayLayerMask;
        [SerializeField] private float grappleRange = 15f;
        [SerializeField] private float grappleSpeed = 10f;
        [SerializeField] private float grappleAscendSpeed = 3f;
        [SerializeField] private float grappleDescendSpeed = 3f;
        [SerializeField] private bool hasGrapple;

        [Header("Grapple Visuals")]
        [SerializeField] private LineRenderer grappleLine;
        [SerializeField] private Transform grappleTip;
        [SerializeField] private TextMeshProUGUI grapplePromptText;

        [Header("Fishing Reference")]
        [SerializeField] private FishingRod currentFishingRod;

        // === Private Fields ===
        private Vector2 _moveInput;
        private float _initialMovementSpeed;
        private float _initialRotationSpeed;

        private bool _isCurrentlyEngaged;
        private bool _grappleMode;
        private bool _fireGrapple;
        private bool _targetHooked;
        private bool _isGrappling;
        private Vector3 _grappleStart;
        private Vector3 _grappleAnchor;
        private float _currentRopeLength;

        private bool _isInGrappleZone;

        // Input Actions
        private InputAction _grappleUpAction;
        private InputAction _grappleDownAction;
        private InputAction _grappleCancelAction;

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
            playerActionMap.FindAction("Crouch").performed += ToggleGrapple;
            playerActionMap.FindAction("Reel").performed += Fire;
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

            GrappleZone zone = FindObjectOfType<GrappleZone>();
            if (zone != null)
            {
                zone.OnPlayerEntered += player => { _isInGrappleZone = true; };
                zone.OnPlayerExited += player => { _isInGrappleZone = false; };
            }
        }

        private void Update()
        {
            Movement();
            GrappleBehaviour();
            HandlePromptUI();
            HandleLineRenderer();

            // Camera & Mesh visibility
            if (brain.IsBlending)
                playerMesh.SetActive(true);
            else
                playerMesh.SetActive(brain.ActiveVirtualCamera.Name != fpsCamera.name);
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
                characterController.SimpleMove(Vector3.down);
            }
        }

        // === Grapple ===
        private void GrappleBehaviour()
        {
            if (!hasGrapple) return;

            // Fire grapple only if in zone
            if (_grappleMode && !_isGrappling && _fireGrapple && _isInGrappleZone)
            {
                _fireGrapple = false;

                if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward,
                                    out RaycastHit hit, grappleRange, rayLayerMask))
                {
                    _grappleAnchor = hit.point;
                    _isGrappling = true;
                    _targetHooked = true;
                    _grappleStart = transform.position;
                    _currentRopeLength = Vector3.Distance(_grappleStart, _grappleAnchor);

                    GameManager.Instance.GameEvents.ToggleGrappleCamera(false);
                }
            }

            // While grappling
            if (_isGrappling)
            {
                Vector3 direction = (_grappleAnchor - transform.position).normalized;

                // === 垂直输入 ===
                float verticalInput = 0f;
                if (_grappleUpAction != null && _grappleUpAction.IsPressed()) verticalInput = 1f;
                else if (_grappleDownAction != null && _grappleDownAction.IsPressed() && !_isGrounded()) verticalInput = -1f;

                _currentRopeLength -= verticalInput * Time.deltaTime *
                                      (verticalInput > 0 ? grappleAscendSpeed : grappleDescendSpeed);
                _currentRopeLength = Mathf.Clamp(_currentRopeLength, 1f, grappleRange);

                // 平滑移动
                Vector3 targetPosition = _grappleAnchor - direction * _currentRopeLength;
                characterController.Move((targetPosition - transform.position) * (grappleSpeed * Time.deltaTime));
            }
        }

        // === Grapple Prompt UI ===
        private void HandlePromptUI()
        {
            if (grapplePromptText == null) return;

            grapplePromptText.enabled = _isInGrappleZone && hasGrapple;
            if (_isInGrappleZone && hasGrapple) grapplePromptText.text = "Press X to Grapple";
        }

        // === Grapple Line Renderer ===
        private void HandleLineRenderer()
        {
            if (grappleLine == null) return;

            if (_isGrappling)
            {
                grappleLine.enabled = true;
                grappleLine.SetPosition(0, grappleTip.position);
                grappleLine.SetPosition(1, _grappleAnchor);
            }
            else
            {
                grappleLine.enabled = false;
            }
        }

        // === Input Handlers ===
        private void ToggleGrapple(InputAction.CallbackContext context)
        {
            if (context.performed && hasGrapple && !_isCurrentlyEngaged && _isInGrappleZone)
            {
                _grappleMode = !_grappleMode;
            }
        }

        private void Fire(InputAction.CallbackContext context)
        {
            if (context.performed && _grappleMode && _isInGrappleZone)
            {
                _fireGrapple = true;
            }
        }

        private void AttemptToPickupItem(InputAction.CallbackContext context)
        {
            GameManager.Instance.GameEvents.AttemptItemPickup();
        }

        private void Move(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        private void CancelMove(InputAction.CallbackContext context)
        {
            _moveInput = Vector2.zero;
        }

        private void CancelGrapple()
        {
            if (_isGrappling)
            {
                _isGrappling = false;
                _targetHooked = false;
                _grappleMode = false;
            }
        }

        // === Utility ===
        private bool _isGrounded() => characterController.isGrounded;

        public void EnableGrapple() => hasGrapple = true;

        public void DisableGrapple()
        {
            hasGrapple = false;
            _grappleMode = false;
        }

        public Quaternion GetPlayerBodyRotation() => playerBody.transform.rotation;

        public void ToggleMovement(bool isMovementEnabled)
        {
            if (isMovementEnabled)
            {
                movementSpeed = _initialMovementSpeed;
                rotationSpeed = _initialRotationSpeed;
                return;
            }

            movementSpeed = 0;
            rotationSpeed = 0;
        }
    }
}
