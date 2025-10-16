using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using FishingGame.Items;

namespace FishingGame.UI.Shop
{
    /// <summary>
    /// UI entry for a single shop item (buyable).
    /// </summary>
    public class ShopItemUI : MonoBehaviour
    {
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private Button buyButton;

        private ItemScriptable _currentItem;
        private Action<ItemScriptable> _onBuyCallback;

        /// <summary>
        /// Setup the UI entry and hook the buy callback.
        /// </summary>
        public void Setup(ItemScriptable item, Action<ItemScriptable> buyCallback)
        {
            _currentItem = item;
            _onBuyCallback = buyCallback;

            if (item == null) return;

            // Set UI display
            itemNameText.text = item.ItemName;
            priceText.text = item.Price.ToString();
            itemIcon.sprite = item.Item2DTexture;

            // Button callback
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => _onBuyCallback?.Invoke(_currentItem));
        }
    }
}


