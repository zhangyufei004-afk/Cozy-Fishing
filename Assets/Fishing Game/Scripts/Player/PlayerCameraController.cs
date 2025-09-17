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
        private string _npcName;
        
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
        /// <param name="npcName">The name of the NPC we are close to.</param>
        public void SetInDialogueRange(bool isInDialogueRange, string npcName)
        {
            _isInDialogueRange = isInDialogueRange; 
            _npcName = npcName;
            if (!isInDialogueRange)
            {
                SwitchToTopDownCamera();
            }
        }

        private void OnEnable()
        {
            InputActionAsset inputActions = InputSystem.actions;
            InputActionMap playerActionMap = inputActions.FindActionMap("Player");
            playerActionMap.Enable();
            playerActionMap.FindAction("Interact").started += SwitchToDialogueCamera;
            GameManager.Instance.GameEvents.OnToggleDialogueCamera += ToggleDialogueCamera;
            GameManager.Instance.GameEvents.OnWithinDialogueRange += SetInDialogueRange;
        }

        private void SwitchToDialogueCamera(InputAction.CallbackContext context)
        {
            GameManager.Instance.GameEvents.NPCInteraction(true, _npcName);
        }

        private void ToggleDialogueCamera(bool enableCamera)
        {
            dialogCamera.gameObject.SetActive(enableCamera);
        }
    }
}
