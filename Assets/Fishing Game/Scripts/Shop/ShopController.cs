using System.Collections.Generic;
using UnityEngine;
using FishingGame.Economy;
using FishingGame.Inventory;
using FishingGame.Items;
using FishingGame.FishSystem;

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

        private bool isOpen = false;

        public void OpenShopUI()
        {
            shopUI?.SetActive(true);
            isOpen = true;
        }

        public void CloseShopUI()
        {
            shopUI?.SetActive(false);
            isOpen = false;
        }

        public bool IsShopOpen() => isOpen;

        /// <summary>
        /// Returns all ItemScriptables available for sale.
        /// </summary>
        public List<ItemScriptable> GetShopItemsAsScriptables()
        {
            return shopInventory.GetItemsForSale();
        }

        /// <summary>
        /// Attempts to buy an item and add it to the player's inventory.
        /// </summary>
        public bool BuyItem(ItemScriptable itemScriptable, InventorySystem playerInventory, int price)
        {
            if (itemScriptable == null || playerInventory == null)
                return false;

            if (!EconomySystem.Instance.SpendMoney(price))
                return false;

            // Create runtime ItemData from ItemScriptable
            ItemData itemData = new ItemData(itemScriptable);
            playerInventory.AddItem(itemData);
            Debug.Log($"Bought {itemScriptable.ItemName}");
            return true;
        }
    }
}








