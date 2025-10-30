using FishingGame.GameManagement;
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
        private bool _isBusy = false;
        private InputAction _triggerMenuAction;

        private void OnEnable()
        {
            _triggerMenuAction = InputSystem.actions.FindAction("Player/Inventory");
            _triggerMenuAction.Enable();
            _triggerMenuAction.performed += ToggleMenu;

            InputSystem.actions.FindAction("UI/Back").performed += ToggleMenu;
            InputSystem.actions.FindAction("UI/CloseUI").performed += ToggleMenu;


            _isMenuActive = false;
            SetMenu(_isMenuActive);

            GameManager.Instance.GameEvents.OnBecomeOccupied +=
               isCurrentlyEngaged => _isBusy = isCurrentlyEngaged;
        }

        private void OnDisable()
        {
            _triggerMenuAction.Disable();
        }
        
        public void ToggleMenu()
        {
            // if menu is active
            if (_isMenuActive)
            {
                DisableMenu();
            }
            // if not busy
            else if (!_isBusy)
            {
                EnableMenu();
            }
            SetMenu(_isMenuActive);

        }

        private void DisableMenu()
        {
            GameManager.Instance.GameEvents.SetPlayerOccupied(false);
            _isMenuActive = false;
        }

        private void EnableMenu()
        {
            GameManager.Instance.GameEvents.SetPlayerOccupied(true);
            _isMenuActive = true;
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

        private void ToggleMenu(InputAction.CallbackContext context)
        {
            ToggleMenu();
        }
    }
}
