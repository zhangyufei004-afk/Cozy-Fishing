using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace FishingGame.UI
{
    /// <summary>
    /// Class which hides UI element when the back button is pressed on the controller.
    /// Must be placed on the UI element itself (or the parent)
    /// </summary>
    public class HideUIController : MonoBehaviour
    {
        [SerializeField] private Selectable itemToSelectUponClose;
        private void OnEnable()
        {
            InputSystem.actions.FindAction("UI/Back").performed += HideUI;
        }

        private void OnDisable()
        {
            InputSystem.actions.FindAction("UI/Back").performed -= HideUI;
        }

        private void HideUI(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                EventSystem.current.SetSelectedGameObject(itemToSelectUponClose.gameObject);
                gameObject.SetActive(false);
            }
        }
    }
}
