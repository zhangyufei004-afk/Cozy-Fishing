using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingGame.UI
{
    /// <summary>
    /// Controls the UI panel visibility of all Menus.
    /// Toggles the menus when the Tab key is pressed.
    /// </summary>
    public class MenuManager : MonoBehaviour
    {
        // Private Readable Variables
        [Header("References")]
        [SerializeField]
        private GameObject menuObject;

        // Private Variables
        private bool _isMenuActive = false;
        private InputAction _triggerMenuAction;

        public void ToggleMenu()
        {
            _isMenuActive = !_isMenuActive;
            SetMenu(_isMenuActive);
        }

        public void SetMenu(bool isActive)
        {
            menuObject.SetActive(isActive);
            Time.timeScale = isActive ? 0.0f : 1.0f;
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

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
    }
}
