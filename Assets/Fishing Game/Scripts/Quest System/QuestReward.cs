using FishingGame.Inventory;
using UnityEngine;

namespace FishingGame.QuestSystem
{
    public static class QuestReward
    {
        private static InventorySystem _inventory;
        public static void GivePlayerItem(GameObject item)
        {
            _inventory ??= Object.FindFirstObjectByType<InventorySystem>();
            
            _inventory.AddItem(item.GetComponent<IStorable>());
        }
    }
}