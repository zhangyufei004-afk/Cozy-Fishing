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
        private InputActionAsset _inputActions;
        private bool _isInventoryOpen;
        private InputAction _triggerInventoryAction;

        private void OnEnable()
        {
            _inputActions = InputSystem.actions;
            _inputActions.FindActionMap("Player").Enable();
            _inputActions.FindActionMap("UI").Enable();
            _isInventoryOpen = false;
            _triggerInventoryAction = _inputActions.FindAction("Player/Inventory");
        }

        private void Update()
        {
            if (_triggerInventoryAction.WasPressedThisFrame())
            {
                _isInventoryOpen = !_isInventoryOpen;
                inventoryPanel.SetActive(_isInventoryOpen);
            }
        }
    }
}
