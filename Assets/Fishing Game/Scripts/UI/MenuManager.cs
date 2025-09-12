using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingGame
{
    public class MenuManager : MonoBehaviour
    {
        // Private Readable Variables
        [Header("References")]
        [SerializeField]
        private GameObject menuObject;

        // Private Variables
        private bool _isMenuActive = false;
        private InputAction _triggerMenuAction;

        private void OnEnable()
        {
            _triggerMenuAction = InputSystem.actions.FindAction("Player/Inventory");
            _triggerMenuAction.Enable();
            _triggerMenuAction.performed += ToggleMenu;

            _isMenuActive = false;
            SetMenu(_isMenuActive);
        }

        private void OnDisable()
        {
            _triggerMenuAction.Disable();
        }

        private void ToggleMenu(InputAction.CallbackContext context)
        {
            ToggleMenu();
        }

        public void ToggleMenu()
        {
            _isMenuActive = !_isMenuActive;
            SetMenu(_isMenuActive);
        }

        public void SetMenu(bool isActive)
        {
            menuObject.SetActive(isActive);
        }
    }
}
