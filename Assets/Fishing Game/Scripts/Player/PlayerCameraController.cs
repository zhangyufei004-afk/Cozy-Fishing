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

        [Header("Player")]
        [SerializeField] private PlayerController player;
        
        [Header("UI")]
        [SerializeField] private GameObject grappleTargetUI;

        private bool _isInDialogueRange;
        private string _npcName;
        private bool _isCurrentlyEngaged;
        
        private void OnEnable()
        {
            InputActionAsset inputActions = InputSystem.actions;
            InputActionMap playerActionMap = inputActions.FindActionMap("Player");
            playerActionMap.Enable();
            playerActionMap.FindAction("Interact").started += SwitchToDialogueCamera;
            playerActionMap.FindAction("Crouch").started += ToggleGrappleCamera;
            

            GameManager.Instance.GameEvents.OnToggleDialogueCamera += ToggleDialogueCamera;
            GameManager.Instance.GameEvents.OnToggleGrappleCamera += ToggleGrappleCamera;
            GameManager.Instance.GameEvents.OnBecomeOccupied += isCurrentlyEngaged => _isCurrentlyEngaged = isCurrentlyEngaged;
            GameManager.Instance.GameEvents.OnWithinDialogueRange += SetInDialogueRange;


        }
        
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
        
        private void ToggleGrappleCamera(InputAction.CallbackContext context)
        {
            ToggleGrappleCamera(!grappleCamera.gameObject.activeSelf);
        }

        private void ToggleGrappleCamera(bool isCameraEnabled)
        {
            if (player.HasGrappleHook && !_isCurrentlyEngaged)
            {
                // grappleCamera.transform.rotation = player.GetPlayerBodyRotation();
                grappleCamera.gameObject.SetActive(isCameraEnabled);
                if (isCameraEnabled)
                {
                    grappleCamera.GetComponent<CinemachinePanTilt>()
                        .ForceCameraPosition(grappleCamera.transform.position, player.GetPlayerBodyRotation());

                }

                dialogCamera.gameObject.SetActive(false);
                grappleTargetUI.SetActive(isCameraEnabled);
            }
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
