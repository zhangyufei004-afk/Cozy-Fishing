using System.Collections.Generic;
using UnityEngine;
using FishingGame.FishSystem;
using FishingGame.FishLog;
using FishingGame.GameManagement;

namespace FishingGame.Inventory
{
    /// <summary>
    /// Inventory System - Stores caught fish and updates FishLog.
    /// </summary>
    public class InventorySystem : MonoBehaviour
    {
        [SerializeField] private FishLogSystem fishLogSystem;
        private List<IStorable> _fishInventory = new List<IStorable>();

        private void OnEnable()
        {
            GameManager.Instance.GameEvents.OnItemReceived += AddItem;
            GameManager.Instance.GameEvents.OnItemUsedUp += RemoveItem;
        }

        /// <summary>
        /// Adds a new item to the inventory. If the item is a fish, it marks it as caught in FishLogSystem.
        /// </summary>
        /// <param name="newItem">The new item instance to add</param>
        public void AddItem(IStorable newItem)
        {
            _fishInventory.Add(newItem);
            GameManager.Instance.GameEvents.InventoryUpdated(_fishInventory);
            if (fishLogSystem is not null && newItem.GetItemType() == EItemType.Fish)
            {
                Fish newFish = newItem as Fish;
                fishLogSystem.RegisterFishCaught(newFish?.GetFishBase());
            }
        }

        /// <summary>
        /// Returns the full list of items in the inventory.
        /// </summary>
        public List<IStorable> GetFishInventory()
        {
            return _fishInventory;
        }

        /// <summary>
        /// Clears the inventory of all items. 
        /// </summary>
        public void ClearInventory()
        {
            _fishInventory.Clear();
        }

        private void RemoveItem(IStorable itemToRemove)
        {
            _fishInventory.Remove(itemToRemove);
            GameManager.Instance.GameEvents.InventoryUpdated(_fishInventory);
        }
    }
}
