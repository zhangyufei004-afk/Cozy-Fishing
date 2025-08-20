using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingGame.Player
{
    /// <summary>
    /// Player Movement Controller. Moves the character using a Character Controller. 
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        private readonly int _speed = Animator.StringToHash("Speed");

        [SerializeField]
        private GameObject playerBody;
        [SerializeField]
        private CharacterController characterController;
        [SerializeField]
        private float movementSpeed; // NOTE: BEST VALUE SEEMED LIKE 6
        [SerializeField]
        private float rotationSpeed; // NOTE BEST VALUE SEEMED LIKE 20
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Animator animator;
        
        private float _initialMovementSpeed;
        private float _initialRotationSpeed;
        private Vector2 _moveInput;
        

        private void OnEnable()
        {
            InputActionAsset inputActions = InputSystem.actions;
            InputActionMap playerActionMap = inputActions.FindActionMap("Player");
            playerActionMap.Enable();
            playerActionMap.FindAction("Move").performed += Move;
            playerActionMap.FindAction("Move").canceled += CancelMove;
            _initialMovementSpeed = movementSpeed;
            _initialRotationSpeed = rotationSpeed;
        }

        void Update()
        {
            Vector3 directionNormalized = Vector3.ClampMagnitude(new Vector3(_moveInput.x, 0, _moveInput.y), 1);
            
            characterController.SimpleMove(directionNormalized * movementSpeed);

            float animationSpeed = Mathf.Clamp(characterController.velocity.magnitude / 2f, min: 0, max: 2f);
            
            animator.SetFloat(_speed, animationSpeed);
            
            if (_moveInput != Vector2.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionNormalized, Vector3.up);
                playerBody.transform.rotation = Quaternion.Slerp(playerBody.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        
        /// <summary>
        /// Enables or Disables Player Movement
        /// </summary>
        /// <param name="isMovementEnabled">Is the movement enabled or disabled</param>
        public void ToggleMovement(bool isMovementEnabled)
        {
            if (isMovementEnabled)
            {
                movementSpeed = 0;
                rotationSpeed = 0;
            }
            else
            {
                movementSpeed = _initialMovementSpeed;
                rotationSpeed = _initialRotationSpeed;
            }
        }
        
        private void Move(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        private void CancelMove(InputAction.CallbackContext context)
        {
            _moveInput = Vector2.zero;
        }

        

    }
}
