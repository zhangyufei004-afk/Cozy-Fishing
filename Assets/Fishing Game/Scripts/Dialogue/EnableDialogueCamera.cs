using System;
using FishingGame.Player;
using UnityEngine;

namespace FishingGame.Dialogue
{
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
