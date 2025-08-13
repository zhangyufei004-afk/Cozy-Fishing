using UnityEditor.UI;
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

        void Update()
        {
            Vector3 directionNormalized = new Vector3(_moveInput.y, 0, -_moveInput.x).normalized;

            float movementWeight = Mathf.Abs(_moveInput.y) > Mathf.Abs(_moveInput.x) ? _moveInput.y : _moveInput.x;

            characterController.SimpleMove(directionNormalized * (movementSpeed * Mathf.Abs(movementWeight)));

            if (_moveInput != Vector2.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionNormalized, Vector3.up) * Quaternion.Euler(0, -90f, 0);
                playerBody.transform.rotation = Quaternion.Slerp(playerBody.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// Reads movement input
        /// </summary>
        /// <param name="context"></param>
        public void Move(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

    }
}
