using FishingGame.FishSystem;
using FishingGame.Inventory;
using FishingGame.UI.Inventory;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FishingGame.UI.Inventory
{
    /// <summary>
    /// UI Entry for displaying a fish in the Inventory.
    /// TODO: update this to be IStorable? probably?
    /// </summary>
    public class InventoryUIEntry : MonoBehaviour
    {
        private IStorable _item;
        private InventoryUI _inventoryUIController;

        // Getters / Setters
        public IStorable Item
        {
            get { return _item; }
            set { _item = value; }
        }

        public InventoryUI InventoryUIController
        {
            set {_inventoryUIController = value; }
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
                    VisualIsFish((Fish)_item);
                    break;
                case EItemType.Rod:
                    Debug.Log("TODO: Tried to add a rod to the inventory UI, but we don't have logic for that yet. ");
                    break;
                case EItemType.RodAttachment:
                    VisualRodAttatchment((ItemData)_item);
                    break;
                case EItemType.Money:
                    Debug.Log("TODO: Tried to add money to the inventory UI, but we don't have logic for that yet. ");
                    break;
                case EItemType.Trash:
                    VisualTrash((Trash)_item);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region Setup UI based on item type

        /// <summary>
        /// Run when the visual item is a fish
        /// </summary>
        /// <param name="fishUpdating">The fish data being used</param>
        private void VisualIsFish(Fish fishUpdating)
        {
            ResetTextElements();
            Image image = transform.Find("FishMask").transform.Find("FishImage").GetComponent<Image>();
            TextMeshProUGUI nameText = transform.Find("FishName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI lengthText = transform.Find("FishLength").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI weightText = transform.Find("FishWeight").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI caughtTimeText = transform.Find("FishCaughtTime").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI locationText = transform.Find("FishLocation").GetComponent<TextMeshProUGUI>();

            image.sprite = fishUpdating.GetFishBase().Texture;
            nameText.text = fishUpdating.GetFishBase().SpeciesName;
            lengthText.text = "temp"; // missing?
            weightText.text = $"{fishUpdating.GetWeight():0.00}kg";
            caughtTimeText.text = fishUpdating.GetCaughtTime().ToString();
            locationText.text = fishUpdating.GetCaughtLocation();
        }

        /// <summary>
        /// Run when the visual item is a trash
        /// </summary>
        /// <param name="trashUpdating">Trash data being used</param>
        private void VisualTrash(Trash trashUpdating)
        {
            ResetTextElements();
            Image image = transform.Find("FishMask").transform.Find("FishImage").GetComponent<Image>();
            TextMeshProUGUI nameText = transform.Find("FishName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI lengthText = transform.Find("FishLength").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI weightText = transform.Find("FishWeight").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI caughtTimeText = transform.Find("FishCaughtTime").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI locationText = transform.Find("FishLocation").GetComponent<TextMeshProUGUI>();

            image.sprite = trashUpdating.GetTrashBase().Texture;
            nameText.text = trashUpdating.GetTrashBase().TrashName;
            lengthText.text = "temp" ; // missing?
            weightText.text = $"{trashUpdating.GetWeight():0.00}kg";
            caughtTimeText.text = trashUpdating.GetCaughtTime().ToString();
            locationText.text = trashUpdating.GetCaughtLocation();
        }

        /// <summary>
        /// Sets the required text displays for a rod attatchment
        /// </summary>
        /// <param name="itemUpdating">The item being shown</param>
        private void VisualRodAttatchment(ItemData itemUpdating)
        {
            ResetTextElements();
            Image image = transform.Find("FishMask").transform.Find("FishImage").GetComponent<Image>();
            TextMeshProUGUI nameText = transform.Find("FishName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI lengthText = transform.Find("FishLength").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI weightText = transform.Find("FishWeight").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI caughtTimeText = transform.Find("FishCaughtTime").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI locationText = transform.Find("FishLocation").GetComponent<TextMeshProUGUI>();

            image.sprite = itemUpdating.GetTexture();
            nameText.text = itemUpdating.GetItemName();
            weightText.text = $"{itemUpdating.GetWeight():0.00}kg";
            lengthText.text = itemUpdating.GetCurrentUseCharge().ToString();
            caughtTimeText.text = itemUpdating.GetTooltip();
            caughtTimeText.gameObject.SetActive(false);
            locationText.gameObject.SetActive(false);
        }

        #endregion

        /// <summary>
        /// Resets the active status of text elements to true
        /// </summary>
        private void ResetTextElements()
        {
            Image image = transform.Find("FishMask").transform.Find("FishImage").GetComponent<Image>();
            TextMeshProUGUI nameText = transform.Find("FishName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI lengthText = transform.Find("FishLength").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI weightText = transform.Find("FishWeight").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI caughtTimeText = transform.Find("FishCaughtTime").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI locationText = transform.Find("FishLocation").GetComponent<TextMeshProUGUI>();

            image.gameObject.SetActive(true);
            nameText.gameObject.SetActive(true);
            lengthText.gameObject.SetActive(true);
            weightText.gameObject.SetActive(true);
            caughtTimeText.gameObject.SetActive(true);
            locationText.gameObject.SetActive(true);
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
