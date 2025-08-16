using System;
using FishingGame.Player;
using UnityEngine;

namespace FishingGame.Dialogue
{
    /// <summary>
    /// Enables the ability to switch to the dialogue camera when inside the trigger.
    /// NOTE: The trigger position should reflect the position you want the character to stand in during the dialogue,
    /// typically 1 meter away."
    /// </summary>
    public class EnableDialogueCamera : MonoBehaviour
    {
        PlayerCameraController _playerCameraController;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerCameraController = other.GetComponent<PlayerCameraController>();
                _playerCameraController.SetInDialogueRange(true);
            }
        }
#if UNITY_EDITOR
        // THIS IS A TESTING FUNCTION, IN ACTUAL GAMEPLAY THE INK SCRIPT WILL RETURN THE CAMERA BACK TO NORMAL
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerCameraController.SwitchToTopDownCamera();
                _playerCameraController = null;
            }
        }
#endif
    }
}
