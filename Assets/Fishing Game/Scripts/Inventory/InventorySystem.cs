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
        [SerializeField] private FishLogSystem _fishLogSystem;

        private List<Fish> fishInventory = new();

        /// <summary>
        /// Adds a new fish to the inventory and marks it as caught in FishLogSystem.
        /// </summary>
        /// <param name="newFish">Fish instance</param>
        public void AddFish(Fish newFish)
        {
            fishInventory.Add(newFish);

            if (_fishLogSystem != null)
            {
                _fishLogSystem.RegisterFishCaught(newFish.GetFishBase());
            }
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
