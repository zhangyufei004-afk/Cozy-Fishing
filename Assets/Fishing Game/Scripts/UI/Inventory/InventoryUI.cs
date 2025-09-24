using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FishingGame.FishSystem;
using FishingGame.GameManagement;
using FishingGame.Inventory;
using UnityEngine.EventSystems;

namespace FishingGame.UI.Inventory
{
    /// <summary>
    /// Manages the visual display of the player's inventory of caught fish.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private Transform fishListContainer;
        [SerializeField] private GameObject fishCardPrefab;

        [Header("Inventory Display References")]
        [SerializeField] [Tooltip("The image that shows what fish is being looked at")] private Image fishImage;
        [SerializeField] [Tooltip("The textbox that says the species name")]  private TextMeshProUGUI speciesNameText;
        [SerializeField] [Tooltip("The textbox that says the weight")] private TextMeshProUGUI weight;
        [SerializeField] [Tooltip("The textbox that shows the location text")] private TextMeshProUGUI location;
        [SerializeField] [Tooltip("The textbox that shows the time text")] private TextMeshProUGUI timeText;


        private readonly List<GameObject> _currentFishCards = new List<GameObject>();

        private void Start()
        {
            GameManager.Instance.GameEvents.OnInventoryUpdated += RefreshInventoryUI;
        }

        private void OnDisable()
        {
            if (EventSystem.current)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

        /// <summary>
        /// Adds a fish to the inventory UI as a new card.
        /// </summary>
        /// <param name="fish">Fish object to be displayed.</param>
        public void AddFishToUI(Fish fish)
        {
            GameObject card = Instantiate(fishCardPrefab, fishListContainer);
            InventoryUIEntry inventoryUIEntry = card.GetComponent<InventoryUIEntry>();
            if (inventoryUIEntry)
            {
                inventoryUIEntry.Fish = fish;
                inventoryUIEntry.InventoryUIController = this;
                inventoryUIEntry.UpdateVisuals();
            }
            _currentFishCards.Add(card);
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
        /// Refreshes the inventory UI with the latest list of items.
        /// </summary>
        /// <param name="itemList">The list of items to display.</param>
        public void RefreshInventoryUI(List<IStorable> itemList)
        {
            ClearInventoryUI();

            foreach (var storable in itemList)
            {
                switch (storable.GetItemType())
                {
                    case EItemType.Fish:
                        Fish fish = storable as Fish;
                        AddFishToUI(fish);
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
                        Debug.Log("TODO: Tried to add trash to the inventory UI, but we don't have logic for that yet");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Updates Inventory Info display to show whatever fish was clicked
        /// </summary>
        /// <param name="fish">The fish clicked</param>
        public void FishEntryClicked(Fish fish)
        {
            if (fishImage) fishImage.sprite = fish.GetTexture();
            if (speciesNameText) speciesNameText.text = fish.GetName();
            if (weight) weight.text = fish.GetWeight() + "kg";
            if (location) location.text = fish.GetCaughtLocation();
            if (timeText) timeText.text = fish.GetCaughtTime().ToString();
        }
    }
}


