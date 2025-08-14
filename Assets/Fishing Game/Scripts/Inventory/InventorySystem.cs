using System.Collections.Generic;
using UnityEngine;
using FishingGame.FishSystem;
using FishingGame.FishLog;

namespace FishingGame.Inventory
{
    /// <summary>
    /// Inventory System - Stores caught fish and updates FishLog.
    /// </summary>
    public class InventorySystem : MonoBehaviour
    {
        [SerializeField] private FishLogSystem fishLogSystem;
        private List<Fish> _fishInventory = new List<Fish>();

        /// <summary>
        /// Adds a new fish to the inventory and marks it as caught in FishLogSystem.
        /// </summary>
        /// <param name="newFish">Fish instance</param>
        public void AddFish(Fish newFish)
        {
            _fishInventory.Add(newFish);

            if (fishLogSystem != null)
            {
                fishLogSystem.RegisterFishCaught(newFish.GetFishBase());
            }
        }

        /// <summary>
        /// Returns the full list of caught fish.
        /// </summary>
        public List<Fish> GetFishInventory()
        {
            return _fishInventory;
        }

        /// <summary>
        /// Clears all caught fish.
        /// </summary>
        public void ClearInventory()
        {
            _fishInventory.Clear();
        }
    }
}
