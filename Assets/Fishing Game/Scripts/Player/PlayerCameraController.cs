using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingGame.Player
{
    /// <summary>
    /// Class containing methods which control the cameras used by the player. Useful for switching to a close up camera
    /// or a fist person camera for looking up. 
    /// </summary>
    public class PlayerCameraController : MonoBehaviour
    {
        [Header("Cameras")]
        [SerializeField] private CinemachineCamera dialogCamera;
        [SerializeField] private CinemachineCamera grappleCamera;
        
        [Header("Movement Components")]
        [SerializeField] private PlayerController playerController;

        private bool _isInDialogueRange;
        
        /// <summary>
        /// Switches from the current camera back to the top-down camera. 
        /// </summary>
        public void SwitchToTopDownCamera()
        {
            grappleCamera?.gameObject.SetActive(false);
            dialogCamera.gameObject.SetActive(false);
            playerController.SetMovementEnabled(true);
            _isInDialogueRange = false;
        }
        
        /// <summary>
        /// Sets the in range of NPC with dialogue boolen.
        /// </summary>
        /// <param name="isInDialogueRange">Whether we are in range of an NPC.</param>
        public void SetInDialogueRange(bool isInDialogueRange)
        {
            _isInDialogueRange = isInDialogueRange;
        }

        private void OnEnable()
        {
            InputActionAsset inputActions = InputSystem.actions;
            InputActionMap playerActionMap = inputActions.FindActionMap("Player");
            playerActionMap.Enable();
            playerActionMap.FindAction("Interact").started += SwitchToDialogueCamera;
        }

        private void SwitchToDialogueCamera(InputAction.CallbackContext context)
        {
            bool switchToCamera = context.ReadValueAsButton();
            if (_isInDialogueRange)
            {
                dialogCamera.gameObject.SetActive(switchToCamera);
                playerController.SetMovementEnabled(false);

            }
        }

        
    }
}
