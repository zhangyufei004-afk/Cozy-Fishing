using UnityEngine;

namespace FishingGame.UI.Inventory
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private GameObject _inventoryPanel;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                bool isActive = _inventoryPanel.activeSelf;
                _inventoryPanel.SetActive(!isActive);
            }
        }
    }
}
