using FishingGame.GameManagement;
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
        [Tooltip("A reference to the player gameobect")]
        private GameObject playerGameObject;

        private ItemData _itemData;
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
            if (other.gameObject == playerGameObject)
            {
                _playerInInteractionRange = true;
                GameManager.Instance.GameEvents.PickupItemRange(true, "Press E to pickup " + _itemData.GetItemName());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == playerGameObject)
            {
                _playerInInteractionRange = false;
                GameManager.Instance.GameEvents.PickupItemRange(false, "Press E to picskup " + _itemData.GetItemName());
            }
        }

        private void SetItemData()
        {
            _itemData = new ItemData(itemScriptable);
        }

        private void AttemptItemPickup()
        {
            if (_playerInInteractionRange)
            {
                Destroy(this.gameObject);
            }
            else
            {
                GameManager.Instance.GameEvents.PickupItemRange(false, "Press E to picskup " + _itemData.GetItemName());
            }
        }

    }
}
