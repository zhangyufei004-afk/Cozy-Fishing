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
        [SerializeField][Tooltip("The textbox that says the weight of the item")] private TextMeshProUGUI length;
        [SerializeField] [Tooltip("The textbox that shows the location this was found from")] private TextMeshProUGUI location;
        [SerializeField] [Tooltip("The textbox that shows the time this was found")] private TextMeshProUGUI timeText;

        [SerializeField][Tooltip("The image that shows what item is being looked at")] private TextMeshProUGUI weightLabel;
        [SerializeField][Tooltip("The image that shows what item is being looked at")] private TextMeshProUGUI lengthLabel;
        [SerializeField][Tooltip("The image that shows what item is being looked at")] private TextMeshProUGUI timeLabel;
        [SerializeField][Tooltip("The image that shows what item is being looked at")] private TextMeshProUGUI locationLabel;

        [SerializeField]
        [Tooltip("The button that is pressed to use an item")]
        private Button useItemButton;

        private IStorable _currentlyDisplayedItem;


        private readonly List<GameObject> _currentItemCards = new List<GameObject>();

        private void Start()
        {
            GameManager.Instance.GameEvents.OnInventoryUpdated += RefreshInventoryUI;
        }

        private void OnEnable()
        {
            SetAllLabelsActive(false);
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
            SetAllLabelsActive(false);
            foreach (GameObject card in _currentItemCards)
            {
                Destroy(card);
            }
            _currentItemCards.Clear();
        }

        public void UseButtonClicked()
        {
            _currentlyDisplayedItem.UseItem();
        }

        /// <summary>
        /// Refreshes the inventory UI with the latest list of items.
        /// </summary>
        /// <param name="itemList">The list of items to display.</param>
        public void RefreshInventoryUI(List<IStorable> itemList)
        {
            _currentlyDisplayedItem = null;
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
            _currentlyDisplayedItem = item;
            SetAllLabelsActive(true);
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
            useItemButton.gameObject.SetActive(false);
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
            useItemButton.gameObject.SetActive(false);
            if (itemImage) itemImage.sprite = entryClicked.GetTexture();
            if (itemNameText) itemNameText.text = entryClicked.GetName();
            if (lengthLabel) lengthLabel.text = "Length:";
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
            useItemButton.gameObject.SetActive(true);
            if (itemImage) itemImage.sprite = entryClicked.GetTexture();
            if (itemNameText) itemNameText.text = entryClicked.GetItemName();
            if (weight) weight.text = entryClicked.GetWeight() + "kg";
            if (lengthLabel) lengthLabel.text = "Charges:";
            if (length) length.text = entryClicked.GetCurrentUseCharge().ToString();

            timeLabel.gameObject.SetActive(false);
            locationLabel.gameObject.SetActive(false);
        }

        private void SetAllLabelsActive(bool isActive)
        {
            itemNameText.gameObject.SetActive(isActive);
            itemImage.gameObject.SetActive(isActive);
            weightLabel.gameObject.SetActive(isActive);
            lengthLabel.gameObject.SetActive(isActive);
            timeLabel.gameObject.SetActive(isActive);
            locationLabel.gameObject.SetActive(isActive);
            useItemButton.gameObject.SetActive(isActive);
        }
    }
}


