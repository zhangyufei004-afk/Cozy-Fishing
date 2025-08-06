using UnityEngine;

namespace FishingGame.UI.Inventory
{
    /// <summary>
    /// Controls the inventory UI panel visibility toggling.
    /// Toggles the inventory panel when the Tab key is pressed.
    /// </summary>
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private GameObject inventoryPanel;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                bool isActive = inventoryPanel.activeSelf;
                inventoryPanel.SetActive(!isActive);
            }
        }
    }
}
