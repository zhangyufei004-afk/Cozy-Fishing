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

        private readonly List<GameObject> _currentFishCards = new List<GameObject>();

        private void Start()
        {
            GameManager.Instance.GameEvents.OnInventoryUpdated += RefreshInventoryUI;
        }

        private void OnDisable()
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        /// <summary>
        /// Adds a fish to the inventory UI as a new card.
        /// </summary>
        /// <param name="fish">Fish object to be displayed.</param>
        // TODO: REFACTOR FOR EFFICIENCY
        public void AddFishToUI(Fish fish)
        {
            GameObject card = Instantiate(fishCardPrefab, fishListContainer);
            _currentFishCards.Add(card);

            Image image = card.transform.Find("FishMask").transform.Find("FishImage").GetComponent<Image>();
            TextMeshProUGUI nameText = card.transform.Find("FishName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI lengthText = card.transform.Find("FishLength").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI weightText = card.transform.Find("FishWeight").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI caughtTimeText = card.transform.Find("FishCaughtTime").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI locationText = card.transform.Find("FishLocation").GetComponent<TextMeshProUGUI>();

            image.sprite = fish.GetFishBase().Texture;
            nameText.text = fish.GetFishBase().SpeciesName;
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
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}


