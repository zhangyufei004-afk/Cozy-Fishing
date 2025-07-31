using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FishingGame.UI.Inventory
{
    /// <summary>
    /// Manages the visual display of the player's inventory of caught fish.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private Transform _fishListContainer;
        [SerializeField] private GameObject _fishCardPrefab;

        private readonly List<GameObject> _currentFishCards = new List<GameObject>();

        // Inventory panel GameObject (assigned in inspector or initialized on Start)
        private GameObject _inventoryPanel;

        private void Awake()
        {
            _inventoryPanel = gameObject;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ToggleInventoryVisibility();
            }
        }

        private void ToggleInventoryVisibility()
        {
            if (_inventoryPanel != null)
            {
                _inventoryPanel.SetActive(!_inventoryPanel.activeSelf);
            }
        }

        /// <summary>
        /// Adds a fish to the inventory UI as a new card.
        /// </summary>
        /// <param name="fish">Fish object to be displayed.</param>
        public void AddFishToUI(Fish fish)
        {
            GameObject card = Instantiate(_fishCardPrefab, _fishListContainer);
            _currentFishCards.Add(card);

            Image image = card.transform.Find("FishImage").GetComponent<Image>();
            TextMeshProUGUI nameText = card.transform.Find("FishName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI lengthText = card.transform.Find("FishLength").GetComponent<TextMeshProUGUI>();

            image.sprite = fish.GetFishBase().Texture;
            nameText.text = fish.GetFishBase().SpeciesName;
            lengthText.text = $"{fish.GetLength():0.0} cm";
        }

        /// <summary>
        /// Clears all fish cards from the UI.
        /// </summary>
        public void ClearInventoryUI()
        {
            foreach (GameObject card in _currentFishCards)
            {
                Destroy(card);
            }
            _currentFishCards.Clear();
        }

        /// <summary>
        /// Refreshes the inventory UI with the latest list of fish.
        /// </summary>
        /// <param name="fishList">The list of fish to display.</param>
        public void RefreshInventoryUI(List<Fish> fishList)
        {
            ClearInventoryUI();

            foreach (Fish fish in fishList)
            {
                AddFishToUI(fish);
            }
        }
    }
}
