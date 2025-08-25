using FishingGame.GameManagement;
using FishingGame.NPC;
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

        private bool _isInDialogueRange;
        private QuestGiver _questGiver;
        
        /// <summary>
        /// Switches from the current camera back to the top-down camera. 
        /// </summary>
        public void SwitchToTopDownCamera()
        {
            grappleCamera?.gameObject.SetActive(false);
            dialogCamera.gameObject.SetActive(false);
            GameManager.Instance.GameEvents.TogglePlayerMovement(true);
            _isInDialogueRange = false;
        }

        /// <summary>
        /// Sets the in range of NPC with dialogue boolen.
        /// </summary>
        /// <param name="isInDialogueRange">Whether we are in range of an NPC.</param>
        /// <param name="questGiver">The quest giver NPC we are close to.</param>
        public void SetInDialogueRange(bool isInDialogueRange, QuestGiver questGiver)
        {
            _isInDialogueRange = isInDialogueRange;
            _questGiver = questGiver;
        }

        private void OnEnable()
        {
            InputActionAsset inputActions = InputSystem.actions;
            InputActionMap playerActionMap = inputActions.FindActionMap("Player");
            playerActionMap.Enable();
            playerActionMap.FindAction("Interact").started += SwitchToDialogueCamera;
            GameManager.Instance.GameEvents.OnToggleDialogueCamera += ToggleDialogueCamera;
        }

        private void SwitchToDialogueCamera(InputAction.CallbackContext context)
        {
            bool switchToCamera = context.ReadValueAsButton();
            if (_isInDialogueRange)
            {
                dialogCamera.gameObject.SetActive(switchToCamera);
                GameManager.Instance.GameEvents.TogglePlayerMovement(false);
                _questGiver.InteractWithNPC();
            }
        }

        private void ToggleDialogueCamera(bool enableCamera)
        {
            dialogCamera.gameObject.SetActive(enableCamera);
        }
    }
}
