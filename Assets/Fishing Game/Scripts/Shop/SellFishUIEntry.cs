using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FishingGame.FishSystem;
using FishingGame.Inventory;
using FishingGame.Economy;
using FishingGame.GameManagement;

namespace FishingGame.UI.Shop
{
    /// <summary>
    /// UI entry for displaying a single fish in the Sell Fish panel.
    /// Shows fish details, sell price, and handles selling the fish.
    /// </summary>
    public class SellFishUIEntry : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image fishImage;
        [SerializeField] private TextMeshProUGUI speciesNameText;
        [SerializeField] private TextMeshProUGUI weightText;
        [SerializeField] private TextMeshProUGUI locationText;
        [SerializeField] private TextMeshProUGUI sellPriceText;
        [SerializeField] private Button sellButton;

        private Fish _fish;
        private InventorySystem _playerInventory;

        /// <summary>
        /// The Fish object this entry represents.
        /// </summary>
        public Fish Fish
        {
            get => _fish;
            set
            {
                _fish = value;
                UpdateVisuals();
            }
        }

        /// <summary>
        /// Reference to the player's inventory.
        /// Needed to remove fish when sold.
        /// </summary>
        public InventorySystem PlayerInventory
        {
            set => _playerInventory = value;
        }

        /// <summary>
        /// Updates the UI visuals for this entry.
        /// </summary>
        public void UpdateVisuals()
        {
            if (_fish == null) return;

            if (fishImage) fishImage.sprite = _fish.GetFishBase().Texture;
            if (speciesNameText) speciesNameText.text = _fish.GetName(); // ✅ 用 GetName()
            if (weightText) weightText.text = $"{_fish.GetWeight():0.00}kg";
            if (locationText) locationText.text = _fish.GetCaughtLocation();
            if (sellPriceText) sellPriceText.text = $"Price: {_fish.GetSellPrice()}";
        }

        /// <summary>
        /// Setup the sell button callback.
        /// Automatically removes the fish from inventory and updates the economy.
        /// </summary>
        /// <param name="sellCallback">Optional external callback when selling.</param>
        public void SetupSellButton(Action<Fish> sellCallback = null)
        {
            if (sellButton == null) return;

            sellButton.onClick.RemoveAllListeners();
            sellButton.onClick.AddListener(() =>
            {
                if (_fish == null || _playerInventory == null) return;

                // Remove fish from inventory list
                _playerInventory.GetFishInventory().Remove(_fish);

                // Fire the OnItemUsedUp event via GameEvents
                GameManager.Instance.GameEvents.ItemUsedUp(_fish);

                // Add money to player
                EconomySystem.Instance?.AddMoney(_fish.GetSellPrice());

                // Invoke external callback if provided
                sellCallback?.Invoke(_fish);

                // Destroy the UI entry
                Destroy(gameObject);
            });
        }
    }
}

