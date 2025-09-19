using FishingGame.FishSystem;
using FishingGame.UI.Inventory;
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
        private Fish _fish;
        private InventoryUI _inventoryUIController;

        // Getters / Setters
        public Fish Fish
        {
            get { return _fish; }
            set { _fish = value; }
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
            Image image = transform.Find("FishMask").transform.Find("FishImage").GetComponent<Image>();
            TextMeshProUGUI nameText = transform.Find("FishName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI lengthText = transform.Find("FishLength").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI weightText = transform.Find("FishWeight").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI caughtTimeText = transform.Find("FishCaughtTime").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI locationText = transform.Find("FishLocation").GetComponent<TextMeshProUGUI>();

            image.sprite = _fish.GetFishBase().Texture;
            nameText.text = _fish.GetFishBase().SpeciesName;
            // lengthText = fish.GetSize(); // missing?
            weightText.text = $"{_fish.GetWeight():0.00}kg";
            caughtTimeText.text = _fish.GetCaughtTime().ToString();
            locationText.text = _fish.GetCaughtLocation();
        }

        /// <summary>
        /// Run when this UI element is pressed, this will cause the Inventory UI to display the clicked fish
        /// </summary>
        public void OnClick()
        {
            _inventoryUIController.FishEntryClicked(_fish);
        }
    }
}
