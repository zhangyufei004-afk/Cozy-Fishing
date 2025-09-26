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
                    Debug.Log("TODO: Tried to add a rod attachment to the inventory UI, but we don't have logic for that yet. ");
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

        /// <summary>
        /// Run when the visual item is a fish
        /// </summary>
        /// <param name="fishUpdating">The fish data being used</param>
        private void VisualIsFish(Fish fishUpdating)
        {
            Image image = transform.Find("FishMask").transform.Find("FishImage").GetComponent<Image>();
            TextMeshProUGUI nameText = transform.Find("FishName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI lengthText = transform.Find("FishLength").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI weightText = transform.Find("FishWeight").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI caughtTimeText = transform.Find("FishCaughtTime").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI locationText = transform.Find("FishLocation").GetComponent<TextMeshProUGUI>();

            image.sprite = fishUpdating.GetFishBase().Texture;
            nameText.text = fishUpdating.GetFishBase().SpeciesName;
            // lengthText = fish.GetSize(); // missing?
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
            Image image = transform.Find("FishMask").transform.Find("FishImage").GetComponent<Image>();
            TextMeshProUGUI nameText = transform.Find("FishName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI lengthText = transform.Find("FishLength").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI weightText = transform.Find("FishWeight").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI caughtTimeText = transform.Find("FishCaughtTime").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI locationText = transform.Find("FishLocation").GetComponent<TextMeshProUGUI>();

            image.sprite = trashUpdating.GetTrashBase().Texture;
            nameText.text = trashUpdating.GetTrashBase().TrashName;
            // lengthText = fish.GetSize(); // missing?
            weightText.text = $"{trashUpdating.GetWeight():0.00}kg";
            caughtTimeText.text = trashUpdating.GetCaughtTime().ToString();
            locationText.text = trashUpdating.GetCaughtLocation();
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
