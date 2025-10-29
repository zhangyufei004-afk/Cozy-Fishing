using FishingGame.FishSystem;
using FishingGame.Inventory;
using FishingGame.UI.Inventory;
using System;
using System.Reflection.Emit;
using FishingGame.Items;
using FishingGame.SaveGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FishingGame.UI.Inventory
{
    /// <summary>
    /// UI Entry for displaying a fish in the Inventory.
    /// </summary>
    public class InventoryUIEntry : MonoBehaviour
    {
        private IStorable _item;
        private InventoryUI _inventoryUIController;
        
        // Entry Items
        private Image _image;
        private TextMeshProUGUI _nameText;
        private TextMeshProUGUI _lengthText;
        private TextMeshProUGUI _weightText;
        private TextMeshProUGUI _caughtTimeText;
        private TextMeshProUGUI _locationText;

        // Getters / Setters
        public IStorable Item
        {
            get => _item;
            set => _item = value;
        }

        public InventoryUI InventoryUIController
        { 
            set => _inventoryUIController = value;
        }

        /// <summary>
        /// Initialises all the variables of the Entry
        /// </summary>
        public void Initialise()
        {
            _image = transform.Find("FishMask").transform.Find("FishImage").GetComponent<Image>();
            _nameText = transform.Find("FishName").GetComponent<TextMeshProUGUI>();
            _lengthText = transform.Find("FishLength").GetComponent<TextMeshProUGUI>();
            _weightText = transform.Find("FishWeight").GetComponent<TextMeshProUGUI>();
            _caughtTimeText = transform.Find("FishCaughtTime").GetComponent<TextMeshProUGUI>();
            _locationText = transform.Find("FishLocation").GetComponent<TextMeshProUGUI>();
        }

        /// <summary>
        /// Updates the visuals of the inventory entry.
        /// </summary>
        [ContextMenu("Update Visuals")]
        public void UpdateVisuals()
        {
            switch(_item.GetItemType())
            {
                case EItemType.Fish:
                case EItemType.Trash:
                    VisualiseFishable(_item as Fishable);
                    break;
                case EItemType.RodAttachment:
                    VisualRodAttatchment((ItemData)_item);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Attempted to display the details of {_item.GetName()}, but was unable. ");
            }
        }

        #region Setup UI based on item type

        private void VisualiseFishable(Fishable fishable)
        {
            EnableTextElements();

            FishableScriptable dataObject = fishable.GetBase();
            _image.sprite = dataObject.Texture;
            _nameText.text = dataObject.Name;
            _lengthText.text = ""; // missing?
            _weightText.text = $"{fishable.GetWeight():0.00}kg";
            _caughtTimeText.text = fishable.GetCaughtTime().ToString();
            _locationText.text = fishable.GetCaughtLocation();
        }

        /// <summary>
        /// Sets the required text displays for a rod attatchment
        /// </summary>
        /// <param name="itemUpdating">The item being shown</param>
        private void VisualRodAttatchment(ItemData itemUpdating)
        {
            EnableTextElements();

            _image.sprite = itemUpdating.GetTexture();
            _nameText.text = itemUpdating.GetName();
            _weightText.text = $"{itemUpdating.GetWeight():0.00}kg";
            _lengthText.text = itemUpdating.GetCurrentUseCharge().ToString();
            _caughtTimeText.text = itemUpdating.GetTooltip();
        }

        #endregion

        /// <summary>
        /// Resets the active status of text elements to true
        /// </summary>
        private void EnableTextElements()
        {
            _image.gameObject.SetActive(true);
            _nameText.gameObject.SetActive(true);
            _weightText.gameObject.SetActive(true);
        }

        /// <summary>
        /// Run when this UI element is pressed, this will cause the Inventory UI to display the clicked fish
        /// </summary>
        public void OnClick()
        {
            _inventoryUIController.InventoryEntryClicked(_item);
        }
    }
}
