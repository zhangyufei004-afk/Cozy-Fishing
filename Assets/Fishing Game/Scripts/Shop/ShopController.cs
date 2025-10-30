using System.Collections.Generic;
using UnityEngine;
using FishingGame.Economy;
using FishingGame.Inventory;
using FishingGame.Items;
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
        
        [Header("Camera References")]
        [SerializeField] private GameObject shopCamera;

        private bool _isOpen = false;


        /// <summary>
        /// Opens the Shop UI
        /// </summary>
        public void OpenShopUI()
        {
            shopUI?.SetActive(true);
            _isOpen = true;
            shopCamera.SetActive(true);
        }

        /// <summary>
        /// Closes the shop UI
        /// </summary>
        public void CloseShopUI()
        {
            shopUI?.SetActive(false);
            _isOpen = false;
            shopCamera.SetActive(false);
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

            ItemData itemData;
            
            if (itemScriptable is BaitScriptable)
            {
                itemData = new FishTypeBait(itemScriptable as FishTypeBaitScriptable);
            }
            else
            {
                itemData = new ItemData(itemScriptable);
            }

            playerInventory.AddItem(itemData);

            Debug.Log($"Bought {itemScriptable.ItemName}");
            return true;
        }
    }
}








