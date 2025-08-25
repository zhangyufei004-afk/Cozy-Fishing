using FishingGame.Inventory;
using UnityEngine;

namespace FishingGame.QuestSystem
{
    /// <summary>
    /// Quest Reward class dispenses the reward to the player and places it in their inventory. Useful for dispending rewards
    /// at the end of quest stages. 
    /// </summary>
    public static class QuestReward
    {
        private static InventorySystem _inventory;
        
        /// <summary>
        /// Give the player the Specified Item and place it in their inventory
        /// </summary>
        /// <param name="item">The item to give the player</param>
        public static void GivePlayerItem(GameObject item)
        {
            _inventory ??= Object.FindFirstObjectByType<InventorySystem>();
            
            _inventory.AddItem(item.GetComponent<IStorable>());
        }
    }
}