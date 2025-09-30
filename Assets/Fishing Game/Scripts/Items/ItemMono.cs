using FishingGame.GameManagement;
using FishingGame.Inventory;
using FishingGame.Items;
using FishingGame.Player;
using FishingGame.Reeling;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingGame
{
    /// <summary>
    /// This is the monobehaviour script that is attatched to item gameobjects
    /// It will update the visuals of the gameobject based on itemModel variable
    /// It can contain anytype of itemScriptable object which will be turned into data
    /// at runtime. It contains functionality for determining if player is in range to pickup item
    /// </summary>
    public class ItemMono : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The item that this object should represent, inputed as a scriptable object")]
        private ItemScriptable itemScriptable;

        [SerializeField]
        [Tooltip("A reference to the player controller script")]
        private PlayerController playerControllerScript;

        [SerializeField]
        [Tooltip("The model this item has in 3D")]
        private Mesh itemModel;

        private IStorable _itemData;
        private bool _playerInInteractionRange = false;

        private InputAction _playerInputAction;
        private InputActionMap _uiActionMap;


        private void OnEnable()
        {
            gameObject.GetComponent<MeshFilter>().mesh = itemModel;
            InputActionAsset inputAction = InputSystem.actions;
            _uiActionMap = inputAction.FindActionMap("Player");
            _playerInputAction = _uiActionMap.FindAction("Interact");

            GameManager.Instance.GameEvents.OnAttemptItemPickup += AttemptItemPickup;

            SetItemData();
        }

        private void OnDestroy()
        {
            GameManager.Instance.GameEvents.OnAttemptItemPickup -= AttemptItemPickup;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == playerControllerScript.gameObject)
            {
                _playerInInteractionRange = true;
                GameManager.Instance.GameEvents.PickupItemRange(true, "Press E to pickup " + _itemData.GetItemName());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == playerControllerScript.gameObject)
            {
                _playerInInteractionRange = false;
                GameManager.Instance.GameEvents.PickupItemRange(false, "Press E to picskup " + _itemData.GetItemName());
            }
        }

        /// <summary>
        /// Sets the itemdata based on what type of scriptable object has been added to this from the inspector
        /// </summary>
        private void SetItemData()
        {
            switch(itemScriptable.ItemType)
            {
                case EItemType.RodAttachment:
                    SetItemDataRodAttatch();
                    break;
                default:
                    Debug.Log("Whatever type of item this is we don't have the implementation for it yet, check ItemMono script under SetItemData()");
                        break;
            }
        }

        /// <summary>
        /// Runs a switch to check what type of itemscriptable has been set from the inspector
        /// Creates a type of ItemData based on that
        /// </summary>
        private void SetItemDataRodAttatch()
        {
            switch (itemScriptable)
            {
                case FishTypeBaitScriptable:
                    _itemData = new FishTypeBait((FishTypeBaitScriptable)itemScriptable, playerControllerScript.CurrentFishingRod);
                    break;
                case ItemScriptable:
                    _itemData = new ItemData(itemScriptable);
                    break;
                default:
                    Debug.Log("Whatever type of item this is we don't have the implementation for it yet, check ItemMono script under SetItemDataBait()");
                    break;

            }
        }

        /// <summary>
        /// Attempts to pickup the item if player is in range
        /// This is run through a game event
        /// If in range adds item to inventory by running itemrecieved event
        /// and then turns off the text that shows an item is in range
        /// Also destroys the game object.
        /// </summary>
        private void AttemptItemPickup()
        {
            if (_playerInInteractionRange)
            {
                GameManager.Instance.GameEvents.ItemReceived(_itemData);
                GameManager.Instance.GameEvents.PickupItemRange(false, "Press E to picskup " + _itemData.GetItemName());
                Destroy(this.gameObject);
            }
        }

    }
}
