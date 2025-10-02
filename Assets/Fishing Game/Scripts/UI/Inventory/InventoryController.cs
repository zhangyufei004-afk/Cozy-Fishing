using FishingGame.GameManagement;
using FishingGame.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingGame.UI.Inventory
{
    /// <summary>
    /// Controls the inventory UI panel visibility toggling.
    /// Toggles the inventory panel when the Tab key is pressed.
    /// </summary>
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private PlayerController playerController;
        private InputActionAsset _inputActions;
        private bool _isInventoryOpen;
        private InputAction _triggerInventoryAction;
        private bool _isBusy;

        private void OnEnable()
        {
            _inputActions = InputSystem.actions;
            _inputActions.FindActionMap("Player").Enable();
            _inputActions.FindActionMap("UI").Enable();
            _isInventoryOpen = false;
            _triggerInventoryAction = _inputActions.FindAction("Player/Inventory");

            GameManager.Instance.GameEvents.OnBecomeOccupied +=
               isCurrentlyEngaged => _isBusy = isCurrentlyEngaged;
        }

        private void Update()
        {
            if (_triggerInventoryAction.WasPressedThisFrame())
            {
                ToggleInventoryVisibility();
            }
        }

        private void ToggleInventoryVisibility()
        {
            Debug.Log("This is temporarily here incase I've brocken something, modification were made to this script thinking it controlled the UI menu");
            if (!_isBusy)
            {
                _isInventoryOpen = !_isInventoryOpen;
                GameManager.Instance.GameEvents.SetPlayerOccupied(true);
            }
            else
            {
                _isInventoryOpen = false;
                GameManager.Instance.GameEvents.SetPlayerOccupied(false);
            }
            inventoryPanel.SetActive(_isInventoryOpen);
            playerController.ToggleMovement(!_isInventoryOpen);
        }
    }
}
