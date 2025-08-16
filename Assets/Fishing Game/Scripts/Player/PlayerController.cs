using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingGame.Player
{
    /// <summary>
    /// Player Movement Controller. Moves the character using a Character Controller. 
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private GameObject playerBody;
        [SerializeField]
        private CharacterController characterController;
        [SerializeField]
        private float movementSpeed; // NOTE: BEST VALUE SEEMED LIKE 6
        [SerializeField]
        private float rotationSpeed; // NOTE BEST VALUE SEEMED LIKE 20
        
        private Vector2 _moveInput;
        
        private float _initialMovementSpeed;
        private float _initialRotationSpeed;

        /// <summary>
        /// Enables or disables the characters movement
        /// </summary>
        /// <param name="isMovementEnabled">Sets the movement enabled parameter</param>
        public void SetMovementEnabled(bool isMovementEnabled)
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
            _initialMovementSpeed = movementSpeed;
            _initialRotationSpeed = rotationSpeed;
        }

        void Update()
        {
            Vector3 directionNormalized = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;
            
            characterController.SimpleMove(directionNormalized * movementSpeed);

            if (_moveInput != Vector2.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionNormalized, Vector3.up);
                playerBody.transform.rotation = Quaternion.Slerp(playerBody.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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
