using FishingGame.FishSystem;
using FishingGame.UI.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FishingGame
{
    public class InventoryUIEntry : MonoBehaviour
    {
        private Fish _fish;
        private InventoryUI _inventoryUIController;

        // Getters / Setters
        public Fish GetFish() => _fish;
        public void SetFish(Fish fish) => _fish = fish;

        public void SetController(InventoryUI _inventoryUIController) => this._inventoryUIController = _inventoryUIController;

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
