using System;
using System.Collections;
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
        private const float RotationSpeed = 10f;
        private const float RotationComparisonThreshold = 0.1f;
        
        [Header("Cameras")]
        [SerializeField] private CinemachineCamera dialogCamera;
        [SerializeField] private CinemachineCamera grappleCamera;

        [Header("Player")]
        [SerializeField] private PlayerController player;
        [SerializeField] private Transform characterBody;
        
        [Header("UI")]
        [SerializeField] private GameObject grappleTargetUI;

        private bool _isInDialogueRange;
        private string _npcName;
        private bool _isCurrentlyEngaged;
        private Vector3 _npcPosition;
        private bool _changePlayerRotation;
        
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

            _npcName = "";

            GameManager.Instance.GameEvents.OnNPCFocus += SetNpcFocus;
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
            if (!isInDialogueRange)
            {
                _npcName = "";
            }
            else
            {
                _npcName = npcName;
            }

            if (!isInDialogueRange)
            {
                SwitchToTopDownCamera();
            }
        }

        private void SetNpcFocus(string npcName, Vector3 npcPosition)
        {
            if (npcName.Equals(_npcName))
            {
                _npcPosition = npcPosition;
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
            if (_npcName != "" && _npcPosition != Vector3.zero)
            {
                StartCoroutine(RotateTowardsNPC());
            }
        }

        private void ToggleDialogueCamera(bool enableCamera)
        {
            dialogCamera.gameObject.SetActive(enableCamera);
        }

        private IEnumerator RotateTowardsNPC()
        {
            Vector3 direction = _npcPosition - transform.position;
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            while (!IsAtRotation(characterBody.rotation, toRotation))
            {
                yield return null;
                characterBody.rotation = Quaternion.Lerp(characterBody.rotation, toRotation, Time.deltaTime * RotationSpeed);
            }
        }

        private bool IsAtRotation(Quaternion currentRotation, Quaternion targetRotation)
        {
            return Mathf.Abs(currentRotation.eulerAngles.x - targetRotation.eulerAngles.x) < RotationComparisonThreshold 
                    && Mathf.Abs(currentRotation.eulerAngles.z - targetRotation.eulerAngles.z) < RotationComparisonThreshold;
        }
    }
}
