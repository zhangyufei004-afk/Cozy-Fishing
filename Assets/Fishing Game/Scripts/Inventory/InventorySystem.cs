using System.Collections.Generic;
using UnityEngine;

namespace FishingGame.Inventory
{
    /// <summary>
    /// Inventory System - Stores caught fish.
    /// </summary>
    public class InventorySystem : MonoBehaviour
    {
        private List<Fish> fishInventory = new List<Fish>();

        /// <summary>
        /// Adds a new fish to the inventory.
        /// </summary>
        /// <param name="newFish">Fish instance</param>
        public void AddFish(Fish newFish)
        {
            fishInventory.Add(newFish);
        }

        /// <summary>
        /// Returns the full list of caught fish.
        /// </summary>
        public List<Fish> GetFishInventory()
        {
            return fishInventory;
        }

        /// <summary>
        /// Clears all caught fish.
        /// </summary>
        public void ClearInventory()
        {
            fishInventory.Clear();
        }
    }
}
