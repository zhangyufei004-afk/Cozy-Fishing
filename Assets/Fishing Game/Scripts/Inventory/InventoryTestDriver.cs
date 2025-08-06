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
        [SerializeField] private InventorySystem inventorySystem;
        [SerializeField] private InventoryUI inventoryUI;
        [SerializeField] private FishScritableObject[] testFishScriptableObjects;

        private int testIndex = 0;

        private void Update()
        {
            // Press T to test
            if (UnityEngine.Input.GetKeyDown(KeyCode.T))
            {
                Fish newFish = new Fish(testFishScriptableObjects[testIndex]);
                inventorySystem.AddFish(newFish);

                inventoryUI.RefreshInventoryUI(inventorySystem.GetFishInventory());

                testIndex = (testIndex + 1) % testFishScriptableObjects.Length;
            }
        }
    }
}
