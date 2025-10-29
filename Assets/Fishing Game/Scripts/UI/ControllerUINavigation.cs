using System;
using FishingGame.GameManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace FishingGame.UI
{
    /// <summary>
    /// This class when attatched to a UI Element will automatically select the defined element for controller
    /// navigation. It will do this OnEnable and only if the current active control scheme is a gamepad.
    /// </summary>
    public class ControllerUINavigation : MonoBehaviour
    {
        [SerializeField] private GameObject firstItemSelected;
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private GameManager gameManager;

        private void OnEnable()
        {
            if (gameManager?.GetCurrentControlScheme() is Gamepad)
            {
                eventSystem.SetSelectedGameObject(firstItemSelected);
            }
        }
    }
}