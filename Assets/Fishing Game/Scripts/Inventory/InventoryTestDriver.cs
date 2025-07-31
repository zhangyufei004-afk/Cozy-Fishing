using UnityEngine;
using FishingGame.Inventory;
using FishingGame.UI.Inventory;

namespace FishingGame.Inventory
{
    /// <summary>
    /// Test script to simulate adding fish and showing inventory UI.
    /// </summary>
    public class InventoryTestDriver : MonoBehaviour
    {
        [SerializeField] private InventorySystem _inventorySystem;
        [SerializeField] private InventoryUI _inventoryUI;
        [SerializeField] private FishScritableObject[] _testFishSOs;

        private int _testIndex = 0;

        private void Update()
        {
            // Press T to test
            if (Input.GetKeyDown(KeyCode.T))
            {
                Fish newFish = new Fish(_testFishSOs[_testIndex]);
                _inventorySystem.AddFish(newFish);

                _inventoryUI.RefreshInventoryUI(_inventorySystem.GetFishInventory());

                _testIndex = (_testIndex + 1) % _testFishSOs.Length;
            }
        }
    }
}
