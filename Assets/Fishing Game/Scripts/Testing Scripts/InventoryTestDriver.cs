using System;
using UnityEngine;
using FishingGame.Inventory;
using FishingGame.UI.Inventory;
using FishingGame.FishSystem;
using FishingGame.GameTime;

namespace FishingGame.Inventory
{
    /// <summary>
    /// Test script to simulate adding fish and showing inventory UI.
    /// </summary>
    [Obsolete("This class is just for testing the UI, and will be removed in future versions.")]
    public class InventoryTestDriver : MonoBehaviour
    {
        [SerializeField] private InventorySystem _inventorySystem;
        [SerializeField] private InventoryUI _inventoryUI;
        [SerializeField] private FishScriptableObject[] _testFishScriptableObjects;

        private int _testIndex = 0;

        private void Update()
        {
            // Press T to test
            if (UnityEngine.Input.GetKeyDown(KeyCode.T))
            {
                Fish newFish = new Fish(
                    _testFishScriptableObjects[_testIndex], 
                    ETimeOfDay.Morning, 
                    "Lake"
                );
                _inventorySystem.AddItem(newFish);

                _inventoryUI.RefreshInventoryUI(_inventorySystem.GetFishInventory());

                _testIndex = (_testIndex + 1) % _testFishScriptableObjects.Length;
            }
        }
    }
}
