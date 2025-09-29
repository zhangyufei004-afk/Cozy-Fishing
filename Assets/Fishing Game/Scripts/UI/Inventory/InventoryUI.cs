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
        [SerializeField] [Tooltip("The image that shows what item is being looked at")] private Image itemImage;
        [SerializeField] [Tooltip("The textbox that says the items name")]  private TextMeshProUGUI itemNameText;
        [SerializeField] [Tooltip("The textbox that says the weight of the item")] private TextMeshProUGUI weight;
        [SerializeField] [Tooltip("The textbox that shows the location this was found from")] private TextMeshProUGUI location;
        [SerializeField] [Tooltip("The textbox that shows the time this was found")] private TextMeshProUGUI timeText;


        private readonly List<GameObject> _currentItemCards = new List<GameObject>();

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
                inventoryUIEntry.Item = fish;
                inventoryUIEntry.InventoryUIController = this;
                inventoryUIEntry.UpdateVisuals();
            }
            _currentItemCards.Add(card);
        }

        /// <summary>
        /// Adds a trash item to the inventory UI as a new card.
        /// </summary>
        /// <param name="trash">The item to be displayed</param>
        public void AddTrashToUI(Trash trash)
        {
            GameObject card = Instantiate(fishCardPrefab, fishListContainer);
            InventoryUIEntry inventoryUIEntry = card.GetComponent<InventoryUIEntry>();
            if (inventoryUIEntry)
            {
                inventoryUIEntry.Item = trash;
                inventoryUIEntry.InventoryUIController = this;
                inventoryUIEntry.UpdateVisuals();
            }
            _currentItemCards.Add(card);
        }

        /// <summary>
        /// Adds a trash item to the inventory UI as a new card.
        /// </summary>
        /// <param name="trash">The item to be displayed</param>
        public void AddAttatchmentToUI(ItemData itemToAdd)
        {
            GameObject card = Instantiate(fishCardPrefab, fishListContainer);
            InventoryUIEntry inventoryUIEntry = card.GetComponent<InventoryUIEntry>();
            if (inventoryUIEntry)
            {
                inventoryUIEntry.Item = itemToAdd;
                inventoryUIEntry.InventoryUIController = this;
                inventoryUIEntry.UpdateVisuals();
            }
            _currentItemCards.Add(card);
        }

        /// <summary>
        /// Clears all fish cards from the UI.
        /// </summary>
        public void ClearInventoryUI()
        {
            foreach (GameObject card in _currentItemCards)
            {
                Destroy(card);
            }
            _currentItemCards.Clear();
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
                        ItemData item = storable as ItemData;
                        AddAttatchmentToUI(item);
                        break;
                    case EItemType.Money:
                        Debug.Log("TODO: Tried to add money to the inventory UI, but we don't have logic for that yet. ");
                        break;
                    case EItemType.Trash:
                        Trash trash = storable as Trash;
                        AddTrashToUI(trash);
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
        public void InventoryEntryClicked(IStorable item)
        {
            switch (item.GetItemType())
            {
                case EItemType.Fish:
                    FishEntryClicked((Fish)item);
                    break;
                case EItemType.Rod:
                    Debug.Log("TODO: Tried to add a rod to the inventory UI, but we don't have logic for that yet. ");
                    break;
                case EItemType.RodAttachment:
                    AttatchmentEntryClicked((ItemData)item);
                    break;
                case EItemType.Money:
                    Debug.Log("TODO: Tried to add money to the inventory UI, but we don't have logic for that yet. ");
                    break;
                case EItemType.Trash:
                    TrashEntryClicked((Trash)item);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            
        }

        /// <summary>
        /// Run when a fish entry is clicked, sets the required display variables
        /// </summary>
        /// <param name="entryClicked">The fish that has been clicked</param>
        private void FishEntryClicked(Fish entryClicked)
        {
            if (itemImage) itemImage.sprite = entryClicked.GetTexture();
            if (itemNameText) itemNameText.text = entryClicked.GetName();
            if (weight) weight.text = entryClicked.GetWeight() + "kg";
            if (location) location.text = entryClicked.GetCaughtLocation();
            if (timeText) timeText.text = entryClicked.GetCaughtTime().ToString();
        }

        /// <summary>
        /// Run when a trash entry is clicked, sets the required display variables
        /// </summary>
        /// <param name="entryClicked">The trash that has been clicked</param>
        private void TrashEntryClicked(Trash entryClicked)
        {
            if (itemImage) itemImage.sprite = entryClicked.GetTexture();
            if (itemNameText) itemNameText.text = entryClicked.GetName();
            if (weight) weight.text = entryClicked.GetWeight() + "kg";
            if (location) location.text = entryClicked.GetCaughtLocation();
            if (timeText) timeText.text = entryClicked.GetCaughtTime().ToString();
        }

        /// <summary>
        /// Run when a Rod Attatchment entry is clicked, sets the required display variables
        /// </summary>
        /// <param name="entryClicked">The trash that has been clicked</param>
        private void AttatchmentEntryClicked(ItemData entryClicked)
        {
            if (itemImage) itemImage.sprite = entryClicked.GetTexture();
            if (itemNameText) itemNameText.text = entryClicked.GetItemName();
            if (weight) weight.text = entryClicked.GetWeight() + "kg";
        }
    }
}


