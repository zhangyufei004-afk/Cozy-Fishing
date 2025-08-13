using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingGame.Player
{
    public class PlayerController : MonoBehaviour
    {
        public GameObject PlayerBody;
        public CharacterController CharacterController;
        public float MovementSpeed;
        public float RotationSpeed;
        Vector2 moveInput;

        void Update()
        {
            Vector3 direction = new Vector3(moveInput.y, 0, -moveInput.x).normalized;

            //see wether the weight of x or z movement is higher and store it
            float movementWeight = Mathf.Abs(moveInput.y) > Mathf.Abs(moveInput.x) ? moveInput.y : moveInput.x;

            //movement speed is scaled by the weight of input
            CharacterController.SimpleMove(direction * (MovementSpeed * Mathf.Abs(movementWeight)));

            if (moveInput != Vector2.zero)
            {
                //PlayerBody.transform.rotation = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(0, -90f, 0);
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(0, -90f, 0);
                PlayerBody.transform.rotation = Quaternion.Slerp(PlayerBody.transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// Reads movement input
        /// </summary>
        /// <param name="context"></param>
        public void Move(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

    }
}
