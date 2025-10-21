using System.Collections;
using FishingGame.GameManagement;
using FishingGame.NPC;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingGame.Player
{
    /// <summary>
    /// Handles dialogue and general camera transitions for the player.
    /// Grapple camera support is disabled since grappling now uses predefined anchors.
    /// </summary>
    public class PlayerCameraController : MonoBehaviour
    {
        private const float RotationSpeed = 10f;
        private const float RotationComparisonThreshold = 0.1f;
        
        [Header("Cameras")]
        [SerializeField] private CinemachineCamera dialogCamera;
        [SerializeField] private CinemachineCamera topDownCamera;

        [Header("Player")]
        [SerializeField] private PlayerController player;
        [SerializeField] private Transform characterBody;
        
        [Header("UI")]
        [SerializeField] private GameObject grappleTargetUI;

        private bool _isInDialogueRange;
        private string _npcName;
        private bool _isCurrentlyEngaged;
        private Vector3 _npcPosition;

        private void OnEnable()
        {
            InputActionAsset inputActions = InputSystem.actions;
            InputActionMap playerActionMap = inputActions.FindActionMap("Player");
            playerActionMap.Enable();
            playerActionMap.FindAction("Interact").started += SwitchToDialogueCamera;

            GameManager.Instance.GameEvents.OnToggleDialogueCamera += ToggleDialogueCamera;
            GameManager.Instance.GameEvents.OnBecomeOccupied += isEngaged => _isCurrentlyEngaged = isEngaged;
            GameManager.Instance.GameEvents.OnWithinDialogueRange += SetInDialogueRange;
            GameManager.Instance.GameEvents.OnNPCFocus += SetNpcFocus;

            _npcName = "";
        }

        /// <summary>
        /// Resets to the default camera view (top-down).
        /// </summary>
        public void SwitchToTopDownCamera()
        {
            dialogCamera?.gameObject.SetActive(false);
            topDownCamera?.gameObject.SetActive(true);
            GameManager.Instance.GameEvents.TogglePlayerMovement(true);
            _isInDialogueRange = false;
        }

        /// <summary>
        /// Updates whether the player is within NPC dialogue range.
        /// </summary>
        public void SetInDialogueRange(bool isInDialogueRange, string npcName)
        {
            _isInDialogueRange = isInDialogueRange;
            _npcName = isInDialogueRange ? npcName : "";

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

        private void SwitchToDialogueCamera(InputAction.CallbackContext context)
        {
            GameManager.Instance.GameEvents.NPCInteraction(true, _npcName);
            if (!string.IsNullOrEmpty(_npcName) && _npcPosition != Vector3.zero)
            {
                StartCoroutine(RotateTowardsNPC());
            }
        }

        private void ToggleDialogueCamera(bool enableCamera)
        {
            GameManager.Instance.GameEvents.SetPlayerOccupied(enableCamera);
            dialogCamera.gameObject.SetActive(enableCamera);
            if (topDownCamera != null)
                topDownCamera.gameObject.SetActive(!enableCamera);
        }

        /// <summary>
        /// Smoothly rotates the player toward the NPC during dialogue.
        /// </summary>
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
