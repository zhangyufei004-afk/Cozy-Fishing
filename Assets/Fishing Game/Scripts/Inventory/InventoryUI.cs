using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FishingGame.FishSystem;

namespace FishingGame.UI.Inventory
{
    /// <summary>
    /// Manages the visual display of the player's inventory of caught fish.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private Transform fishListContainer;
        [SerializeField] private GameObject fishCardPrefab;
        [SerializeField] private GameObject fishInventoryPanel;
        [SerializeField] private GameObject fishlogPanel;
        [SerializeField] private TMP_Dropdown panelDropdown;

        private readonly List<GameObject> _currentFishCards = new List<GameObject>();

        // Inventory panel GameObject (assigned in inspector or initialized on Start)
        private GameObject _inventoryPanel;

        private void Awake()
        {
            _inventoryPanel = gameObject;
        }

        private void Start()
        {
            ShowFishInventory();
            panelDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
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

        private void OnDropdownValueChanged(int index)
        {
            switch (index)
            {
                case 0:
                    ShowFishInventory();
                    break;
                case 1:
                    ShowFishlog();
                    break;
            }
        }

        private void ShowFishInventory()
        {
            fishInventoryPanel.SetActive(true);
            fishlogPanel.SetActive(false);
        }

        private void ShowFishlog()
        {
            fishInventoryPanel.SetActive(false);
            fishlogPanel.SetActive(true);
        }

        /// <summary>
        /// Adds a fish to the inventory UI as a new card.
        /// </summary>
        /// <param name="fish">Fish object to be displayed.</param>
        public void AddFishToUI(Fish fish)
        {
            GameObject card = Instantiate(fishCardPrefab, fishListContainer);
            _currentFishCards.Add(card);

            Image image = card.transform.Find("FishImage").GetComponent<Image>();
            TextMeshProUGUI nameText = card.transform.Find("FishName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI lengthText = card.transform.Find("FishLength").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI weightText = card.transform.Find("FishWeight").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI caughtTimeText = card.transform.Find("FishCaughtTime").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI locationText = card.transform.Find("FishLocation").GetComponent<TextMeshProUGUI>();

            image.sprite = fish.GetFishBase().Texture;
            nameText.text = fish.GetFishBase().SpeciesName;
            lengthText.text = $"{fish.GetLength():0.0} cm";
            weightText.text = $"{fish.GetWeight():0.00} kg";
            caughtTimeText.text = fish.GetCaughtTime().ToString();
            locationText.text = fish.GetCaughtLocation();
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

