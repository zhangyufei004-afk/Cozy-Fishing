using UnityEngine;

namespace PrototypeFishingMechanics
{
    // NOTE: THIS CLASS IS JUST FOR QUICK AND DIRTY MOVEMENT
    public class CharacterMovement : MonoBehaviour
    {
        #region Constants

        private const int SPEED = 5;

        #endregion
        
        #region Private Fields

        private CharacterController _characterController;
        private Vector3 _moveSpeed;

        #endregion

        #region Public Variables
        public bool AllowMovement = true;
        #endregion 

        /// <summary>
        /// Start is called once before the first execution of Update after the MonoBehaviour is created
        /// </summary>
        void Start()
        {
            _characterController = GetComponent<CharacterController>();
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        void Update()
        {
            if (AllowMovement)
            {
                // Moves the character forward and back based off the direction they are facing
                _moveSpeed = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
                _characterController.SimpleMove(_moveSpeed * /*(Time.deltaTime */ SPEED)/*)*/;
            }
            
        }
    }

}
