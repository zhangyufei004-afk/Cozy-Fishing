using System.Collections.Generic;
using UnityEngine;
using FishingGame.Economy;
using FishingGame.Inventory;
using FishingGame.Items;
using FishingGame.FishSystem;
using FishingGame.Items.Bait;

namespace FishingGame.Shop
{
    /// <summary>
    /// Controls shop opening/closing, listing items, and handling purchases.
    /// </summary>
    public class ShopController : MonoBehaviour
    {
        [Header("UI Reference")]
        [SerializeField] private GameObject shopUI;

        [Header("Inventory Reference")]
        [SerializeField] private ShopInventory shopInventory;

        private bool _isOpen = false;

        public void OpenShopUI()
        {
            shopUI?.SetActive(true);
            _isOpen = true;
        }

        public void CloseShopUI()
        {
            shopUI?.SetActive(false);
            _isOpen = false;
        }

        public bool IsShopOpen() => _isOpen;

        /// <summary>
        /// Returns all ItemScriptables available for sale.
        /// </summary>
        public List<ItemScriptable> GetShopItemsAsScriptables()
        {
            return shopInventory.GetItemsForSale();
        }

        /// <summary>
        /// Attempts to buy an item and add it to the player's inventory.
        /// If the item is a bait, automatically adds +1 charge.
        /// </summary>
        public bool BuyItem(ItemScriptable itemScriptable, InventorySystem playerInventory, int price)
        {
            if (itemScriptable == null || playerInventory == null)
                return false;

            if (!EconomySystem.Instance.SpendMoney(price))
                return false;

            // Create runtime ItemData from ItemScriptable
            ItemData itemData = new ItemData(itemScriptable);

            if (itemScriptable is BaitScriptable)
            {
                int currentCharge = itemData.GetCurrentUseCharge();
                var field = typeof(ItemData).GetField("_itemCharge", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                    field.SetValue(itemData, currentCharge + 1);
            }

            playerInventory.AddItem(itemData);

            Debug.Log($"Bought {itemScriptable.ItemName}");
            return true;
        }
    }
}








