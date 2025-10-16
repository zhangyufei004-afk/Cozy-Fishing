using System.Collections.Generic;
using UnityEngine;
using FishingGame.Items;
using FishingGame.FishSystem;

namespace FishingGame.Shop
{
    /// <summary>
    /// Stores items and fish available for sale in this shop.
    /// Inspector configurable.
    /// </summary>
    public class ShopInventory : MonoBehaviour
    {
        [Header("Normal Items")]
        [SerializeField] private List<ItemScriptable> itemsForSale;

        [Header("Fish Items")]
        [SerializeField] private List<FishScriptableObject> fishForSale;

        public List<ItemScriptable> GetItemsForSale() => itemsForSale;
        public List<FishScriptableObject> GetFishForSale() => fishForSale;
    }
}
