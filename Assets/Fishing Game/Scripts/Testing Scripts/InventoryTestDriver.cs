using System;
using UnityEngine;
using FishingGame.Inventory;
using FishingGame.UI.Inventory;

namespace FishingGame.Inventory
{
    /// <summary>
    /// Test script to simulate adding fish and showing inventory UI.
    /// </summary>
    [Obsolete("This class is just for testing the UI, and will be removed in future versions.")]
    public class InventoryTestDriver : MonoBehaviour
    {
        [SerializeField] private InventorySystem inventorySystem;
        [SerializeField] private InventoryUI inventoryUI;
        [SerializeField] private FishScritableObject[] testFishScriptableObjects;

        private int _testIndex = 0;

        private void Update()
        {
            // Press T to test
            if (UnityEngine.Input.GetKeyDown(KeyCode.T))
            {
                Fish newFish = new Fish(testFishScriptableObjects[_testIndex]);
                inventorySystem.AddFish(newFish);

                inventoryUI.RefreshInventoryUI(inventorySystem.GetFishInventory());

                _testIndex = (_testIndex + 1) % testFishScriptableObjects.Length;
            }
        }
    }
}
