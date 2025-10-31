using FishingGame.Items;
using FishingGame.Inventory;
using FishingGame.FishSystem;
using FishingGame.GameTime;

namespace FishingGame.Shop
{
    /// <summary>
    /// Converts ScriptableObjects into runtime IStorable objects.
    /// </summary>
    public static class ShopItemFactory
    {
        public static IStorable CreateItem(ItemScriptable item)
        {
            if (item == null) return null;
            return new ItemData(item);
        }

        public static IStorable CreateItem(FishScriptableObject fish)
        {
            if (fish == null) return null;
            return new Fish(fish, ETimeOfDay.Morning, "Shop");
        }
    }
}

