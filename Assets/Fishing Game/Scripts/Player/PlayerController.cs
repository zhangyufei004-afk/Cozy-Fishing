using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingGame.Player
{
    /// <summary>
    /// player controller
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private GameObject playerBody;
        [SerializeField]
        private CharacterController characterController;
        [SerializeField]
        private float movementSpeed;
        [SerializeField]
        private float rotationSpeed;
        private Vector2 _moveInput;

        private void OnEnable()
        {
            InputActionAsset inputActions = InputSystem.actions;
            InputActionMap playerActionMap = inputActions.FindActionMap("Player");
            playerActionMap.Enable();
            playerActionMap.FindAction("Move").performed += Move;
            playerActionMap.FindAction("Move").canceled += CancelMove;
        }

        void Update()
        {
            Vector3 directionNormalized = new Vector3(_moveInput.y, 0, -_moveInput.x).normalized;
            
            characterController.SimpleMove(directionNormalized * movementSpeed);

            if (_moveInput != Vector2.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionNormalized, Vector3.up) * Quaternion.Euler(0, -90f, 0);
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
