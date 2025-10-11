using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using FishingGame.GameManagement;
using Unity.Cinemachine;
using FishingGame.Reeling;

namespace FishingGame.Player
{
    /// <summary>
    /// Player Movement Controller. Moves the character using a Character Controller. 
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        private readonly int _speed = Animator.StringToHash("Speed");

        public bool HasGrappleHook => hasGrapple;

        [SerializeField] private GameObject playerBody;
        [SerializeField] private GameObject playerMesh;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private float movementSpeed; // NOTE: BEST VALUE SEEMED LIKE 6
        [SerializeField] private float rotationSpeed; // NOTE BEST VALUE SEEMED LIKE 20    
        [SerializeField] private Animator animator;
        [SerializeField] private CinemachineBrain brain;
        [SerializeField] private GameObject fpsCamera;
        [SerializeField] private float grappleRange;
        [SerializeField] private float grappleSpeed;
        [SerializeField] private LayerMask grappleLayer;
        [SerializeField] private LayerMask waterLayerMask;
        [SerializeField] private LayerMask terrainLayerMask;

        [SerializeField] private bool hasGrapple;

        private Vector2 _moveInput;
        private float _initialMovementSpeed;
        private float _initialRotationSpeed;

        // Grapple Variables
        private bool _grappleMode;
        private bool _fireGrapple;
        private bool _targetHooked;
        private float _grappleTime;
        private float _currentGrappleTime;
        private Vector3 _grappleStart;
        private Vector3 _grappleTarget;
        private bool _isCurrentlyEngaged;

        [Tooltip("A reference to the fishingRod script")] [SerializeField]
        private FishingRod currentFishingRod;

        public FishingRod CurrentFishingRod => currentFishingRod;

        // Anti Water Walking Properties
        private int _raycastLayerMask;
        private Vector3 _previousSafePlace;
        private bool _isRespawnRoutineRunning;
        private bool _isDead;
        
        /// <summary>
        /// Enables or disables the characters movement
        /// </summary>
        /// <param name="isMovementEnabled">Sets the movement enabled parameter</param>
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

            _initialMovementSpeed = movementSpeed;
            _initialRotationSpeed = rotationSpeed;

            GameManager.Instance.GameEvents.OnTogglePlayerMovement += ToggleMovement;
            GameManager.Instance.GameEvents.OnBecomeOccupied +=
                isCurrentlyEngaged => _isCurrentlyEngaged = isCurrentlyEngaged;
            GameManager.Instance.GameEvents.OnPlayerDeathScreenActive += RespawnPlayer;

            _raycastLayerMask = waterLayerMask | terrainLayerMask;
            _previousSafePlace = Vector3.zero;
        }

        private void Update()
        {
            if (IsInWater() && !_isDead)
            {
                _isDead = true;
                GameManager.Instance.GameEvents.PlayerDied();
                characterController.SimpleMove(Vector3.zero);
                animator.SetFloat(_speed, 0);
            }
            if (_isDead)
            {
                return;
            }
            
            Movement();
            Grappling();

            if (brain.IsBlending)
            {
                playerMesh.SetActive(true);
            }
            else
            {
                ICinemachineCamera activeCam = brain.ActiveVirtualCamera;

                if (activeCam.Name == fpsCamera.name)
                {
                    playerMesh.SetActive(false);
                }
                else playerMesh.SetActive(true);
            }
        }

        /// <summary>
        /// Enables the Grapple hook on the Player.
        /// Typically called after the player has purchased the grapple hook.
        /// </summary>
        public void EnableGrapple()
        {
            hasGrapple = true;
        }

        /// <summary>
        /// Disables the grapple hook on the player.
        /// Typically called if the player sells or remvoves the grapple hook.
        /// </summary>
        public void DisableGrapple()
        {
            hasGrapple = false;
            _grappleMode = false;
        }

        public Quaternion GetPlayerBodyRotation()
        {
            return playerBody.transform.rotation;
        }
        
        private void RespawnPlayer(bool deathScreenActive)
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
                    Debug.LogError($"Attempted to Respawn player while the player is not dead!");
                    break;
            }
        }

        /// <summary>
        /// Reads  input to switch between cameras
        /// </summary>
        /// <param name="context">The InputAction context</param>
        private void ToggleGrapple(InputAction.CallbackContext context)
        {
            if (context.performed && hasGrapple && !_isCurrentlyEngaged)
            {
                _grappleMode = !_grappleMode;
            }
        }

        /// <summary>
        /// Reads attack input to fire grappling hook
        /// </summary>
        /// <param name="context">The InputAction context</param>
        private void Fire(InputAction.CallbackContext context)
        {
            if (context.performed && _grappleMode)
            {
                _fireGrapple = true;
            }
        }
        
        /// <summary>
        /// Handles the player movement
        /// </summary>
        private void Movement()
        {
            if (!characterController.enabled)
            {
                return;
            }
            if (!_grappleMode)
            {
                UnityEngine.Cursor.lockState = CursorLockMode.None;

                Vector3 directionNormalized = Vector3.ClampMagnitude(new Vector3(_moveInput.x, 0, _moveInput.y), 1);

                float animationSpeed = Mathf.Clamp(characterController.velocity.magnitude / 2f, min: 0, max: 2f);

                animator.SetFloat(_speed, animationSpeed);

                if (!CanWalkInDirection() && _previousSafePlace == Vector3.zero)
                {
                    _previousSafePlace = transform.position;
                }

                characterController.SimpleMove(directionNormalized * movementSpeed);
                
                if (_moveInput != Vector2.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionNormalized, Vector3.up);
                    playerBody.transform.rotation = Quaternion.Slerp(playerBody.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
            }
            else
            {
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;

                animator.SetFloat(_speed, 0);

                characterController.SimpleMove(Vector3.down);
            }
        }

        /// <summary>
        /// Handles the grappling hook features
        /// </summary>
        private void Grappling()
        {
            RaycastHit hit;
            if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, grappleRange, grappleLayer) 
                && _grappleMode)
            {
                if (_fireGrapple && !_targetHooked)
                {
                    _fireGrapple = false;
                    _targetHooked = true;

                    Vector3 directionToTarget = (hit.transform.position - transform.position).normalized;

                    _grappleStart = transform.position;
                    _grappleTarget = hit.transform.position - (directionToTarget * 1f);

                    float distance = Vector3.Distance(_grappleStart, _grappleTarget);
                    _grappleTime = distance / grappleSpeed;
                    _currentGrappleTime = 0f;

                }

            }
            else _fireGrapple = false;
            Debug.DrawRay(fpsCamera.transform.position, fpsCamera.transform.forward * grappleRange, Color.red);

            if (_targetHooked)
            {
                _currentGrappleTime += Time.deltaTime;
                float time = _currentGrappleTime / _grappleTime;
                time = Mathf.Clamp01(time);

                transform.position = Vector3.Lerp(_grappleStart, _grappleTarget, time);

                GameManager.Instance.GameEvents.ToggleGrappleCamera(false);
                Quaternion targetRotation = fpsCamera.transform.rotation;
                targetRotation.x = 0;
                targetRotation.z = 0;
                playerBody.transform.rotation = targetRotation;
                if (time >= 1f)
                {
                    _targetHooked = false;
                    _grappleMode = false;
                }

            }
        }

        /// <summary>
        /// Runs when the player uses the pickup item input
        /// Calls the AttemptToPickUp item event
        /// </summary>
        /// <param name="context"></param>
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


        private bool CanWalkInDirection()
        {
            Vector3 startPositionOffset = transform.position + playerBody.transform.forward * 2f + Vector3.up * 2f;
            Debug.DrawRay(startPositionOffset, Vector3.down * 10f, Color.green, Time.deltaTime);

            if (Physics.Raycast(startPositionOffset, Vector3.down, out RaycastHit hitInfo, 20f, _raycastLayerMask))
            {
                if (1 << hitInfo.transform.gameObject.layer == waterLayerMask)
                {   
                    return false;
                }
                _previousSafePlace = Vector3.zero;
            }

            return true;
        }

        private bool IsInWater()
        {
            Debug.DrawRay(transform.position + Vector3.up * 1.5f, Vector3.down*0.2f, Color.green);
            return Physics.Raycast(transform.position + Vector3.up * 1.5f, Vector3.down, 0.2f, waterLayerMask);
        }
    }
}
