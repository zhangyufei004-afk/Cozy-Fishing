using FishingGame.GameManagement;
using FishingGame.Inventory;
using FishingGame.Items;
using FishingGame.Player;
using FishingGame.Reeling;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingGame
{
    public class ItemMono : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The item that this object should represent, inputed as a scriptable object")]
        private ItemScriptable itemScriptable;

        [SerializeField]
        [Tooltip("A reference to the player controller script")]
        private PlayerController playerControllerScript;

        private IStorable _itemData;
        private bool _playerInInteractionRange = false;

        private InputAction _playerInputAction;
        private InputActionMap _uiActionMap;


        private void OnEnable()
        {
            InputActionAsset inputAction = InputSystem.actions;
            _uiActionMap = inputAction.FindActionMap("Player");
            _playerInputAction = _uiActionMap.FindAction("Interact");

            GameManager.Instance.GameEvents.OnAttemptItemPickup += AttemptItemPickup;

            SetItemData();
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

        private void SetItemData()
        {
            switch(itemScriptable.ItemType)
            {
                case EItemType.RodAttachment:
                    _itemData = new FishTypeBait((FishTypeBaitScriptable)itemScriptable, playerControllerScript.CurrentFishingRod);
                    break;
                default:
                    Debug.Log("Whatever type of item this is we don't have the implementation for it yet, check ItemMono script");
                        break;
            }
        }

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
