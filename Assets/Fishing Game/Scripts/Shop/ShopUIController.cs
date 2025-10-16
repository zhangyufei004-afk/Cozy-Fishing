using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FishingGame.FishSystem;
using FishingGame.Inventory;
using FishingGame.Economy;
using FishingGame.Items;
using FishingGame.Shop;
using TMPro;

namespace FishingGame.UI.Shop
{
    /// <summary>
    /// Controls the shop UI for both buying items and selling fish.
    /// </summary>
    public class ShopUIController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject shopPanel;
        [SerializeField] private Transform buyItemsContainer;
        [SerializeField] private Transform sellItemsContainer;
        [SerializeField] private GameObject shopItemPrefab;
        [SerializeField] private GameObject sellFishPrefab;
        [SerializeField] private TextMeshProUGUI playerMoneyText;

        [Header("Dependencies")]
        [SerializeField] private InventorySystem playerInventory;
        [SerializeField] private ShopController shopController;

        private void OnEnable()
        {
            PopulateBuyItems();
            PopulateSellItems();
            UpdatePlayerMoney();

            if (EconomySystem.Instance != null)
                EconomySystem.Instance.MoneyChanged += OnMoneyChanged;
        }

        private void OnDisable()
        {
            if (EconomySystem.Instance != null)
                EconomySystem.Instance.MoneyChanged -= OnMoneyChanged;
        }

        #region Buy Items

        private void PopulateBuyItems()
        {
            foreach (Transform child in buyItemsContainer)
                Destroy(child.gameObject);

            List<ItemScriptable> items = shopController.GetShopItemsAsScriptables();

            foreach (var item in items)
            {
                GameObject itemGO = Instantiate(shopItemPrefab, buyItemsContainer);
                ShopItemUI itemUI = itemGO.GetComponent<ShopItemUI>();
                if (itemUI != null)
                    itemUI.Setup(item, OnBuyItemClicked);
            }
        }

        private void OnBuyItemClicked(ItemScriptable item)
        {
            int price = item.Price;
            shopController.BuyItem(item, playerInventory, price);
        }

        #endregion

        #region Sell Fish

        private void PopulateSellItems()
        {
            foreach (Transform child in sellItemsContainer)
                Destroy(child.gameObject);

            // Convert IStorable list to Fish list safely using LINQ
            List<Fish> fishInventory = playerInventory.GetFishInventory()
                                                     .OfType<Fish>()
                                                     .ToList();

            foreach (var fish in fishInventory)
            {
                GameObject fishGO = Instantiate(sellFishPrefab, sellItemsContainer);
                SellFishUIEntry entry = fishGO.GetComponent<SellFishUIEntry>();
                if (entry != null)
                {
                    entry.Fish = fish;
                    entry.PlayerInventory = playerInventory;
                    entry.SetupSellButton(); // keep original sell logic
                }
            }
        }

        #endregion

        #region Money UI

        private void OnMoneyChanged(int newMoney)
        {
            UpdatePlayerMoney(newMoney);
        }

        private void UpdatePlayerMoney(int money = -1)
        {
            int currentMoney = (money >= 0) ? money : EconomySystem.Instance.PlayerMoney;
            if (playerMoneyText != null)
                playerMoneyText.text = $"Money: {currentMoney}";
        }

        #endregion

        #region UI Controls

        public void OpenShopUI() => shopPanel.SetActive(true);
        public void CloseShopUI() => shopPanel.SetActive(false);

        #endregion
    }
}

