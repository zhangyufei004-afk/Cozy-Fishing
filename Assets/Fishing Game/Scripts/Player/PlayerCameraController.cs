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
    /// Uses Cinemachine priority rather than enabling/disabling cameras.
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
            if (topDownCamera != null)
                topDownCamera.Priority = 15;

            if (dialogCamera != null)
                dialogCamera.Priority = 5;

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

        /// <summary>
        /// Switches from the default top-down camera to the dialogue camera when interacting with an NPC.
        /// </summary>
        private void SwitchToDialogueCamera(InputAction.CallbackContext context)
        {
            if (!_isInDialogueRange || _isCurrentlyEngaged)
                return;

            GameManager.Instance.GameEvents.NPCInteraction(true, _npcName);

            if (!string.IsNullOrEmpty(_npcName) && _npcPosition != Vector3.zero)
                StartCoroutine(RotateTowardsNPC());

            ToggleDialogueCamera(true);
        }

        /// <summary>
        /// Toggles between dialogue and top-down cameras using Cinemachine priority.
        /// </summary>
        private void ToggleDialogueCamera(bool enableCamera)
        {
            GameManager.Instance.GameEvents.SetPlayerOccupied(enableCamera);

            if (dialogCamera != null)
                dialogCamera.Priority = enableCamera ? 20 : 5;

            if (topDownCamera != null)
                topDownCamera.Priority = enableCamera ? 5 : 15;
        }

        /// <summary>
        /// Smoothly rotates the player toward the NPC during dialogue.
        /// </summary>
        private IEnumerator RotateTowardsNPC()
        {
            Vector3 direction = _npcPosition - transform.position;
            direction.y = 0f;
            Quaternion toRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);

            while (!IsAtRotation(characterBody.rotation, toRotation))
            {
                yield return null;
                characterBody.rotation = Quaternion.Lerp(characterBody.rotation, toRotation, Time.deltaTime * RotationSpeed);
            }
        }

        private bool IsAtRotation(Quaternion currentRotation, Quaternion targetRotation)
        {
            return Quaternion.Angle(currentRotation, targetRotation) < RotationComparisonThreshold;
        }
    }
}

