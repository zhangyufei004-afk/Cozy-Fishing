using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using FishingGame.Player;
using FishingGame.Shop;
using FishingGame.GameManagement;

namespace FishingGame.Shop
{
    /// <summary>
    /// Handles player interaction with the shop.
    /// Opens/closes shop UI when pressing Interact, auto-closes when leaving range,
    /// and ensures UI buttons are interactable.
    /// Uses trigger detection to reduce unnecessary Update checks.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ShopInteractable : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ShopController shopController;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private TextMeshProUGUI interactPromptText;

        [Header("Settings")]
        [SerializeField] private string playerTag = "Player";

        private Transform _player;
        private InputAction _interactAction;
        private bool _isPlayerNearby;
        private bool _isShopOpen;

        #region Unity Methods

        private void OnEnable()
        {
            var inputActions = InputSystem.actions;
            inputActions.FindActionMap("Player").Enable();
            _interactAction = inputActions.FindAction("Player/Interact");
            InputSystem.actions.FindAction("UI/Back").performed += 
                context => CloseShop();
            InputSystem.actions.FindAction("UI/CloseUI").performed += context => CloseShop();
        }

        private void Start()
        {
            if (interactPromptText != null)
                interactPromptText.gameObject.SetActive(false);
        }

        private void Update()
        {
            // Only check input if player is in range
            if (_isPlayerNearby && _interactAction != null && _interactAction.WasPressedThisFrame())
            {
                if (!_isShopOpen)
                {
                    OpenShop();
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
            }
        }

        #endregion

        #region Trigger Methods

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(playerTag)) return;

            _player = other.transform;
            _isPlayerNearby = true;

            if (interactPromptText != null && !_isShopOpen)
            {
                interactPromptText.gameObject.SetActive(true);
                string buttonToPress = "E";
                if (GameManager.Instance.GetCurrentControlScheme() is Gamepad)
                {
                    buttonToPress = "the interact button";
                }

                interactPromptText.text = $"Press {buttonToPress} to access the shop.";
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(playerTag)) return;

            _isPlayerNearby = false;
            _player = null;

            if (interactPromptText != null)
                interactPromptText.gameObject.SetActive(false);

            if (_isShopOpen)
                CloseShop();
        }

        #endregion

        #region Shop Methods

        /// <summary>
        /// Opens the shop UI, disables player movement, and sets player as occupied.
        /// </summary>
        private void OpenShop()
        {
            shopController.OpenShopUI();
            playerController.ToggleMovement(false);
            GameManager.Instance.GameEvents.SetPlayerOccupied(true);
            _isShopOpen = true;

            if (interactPromptText != null)
                interactPromptText.gameObject.SetActive(false);
        }

        /// <summary>
        /// Closes the shop UI, re-enables player movement, and clears player occupied status.
        /// </summary>
        private void CloseShop()
        {
            shopController.CloseShopUI();
            playerController.ToggleMovement(true);
            GameManager.Instance.GameEvents.SetPlayerOccupied(false);
            _isShopOpen = false;

            if (_isPlayerNearby && interactPromptText != null)
                interactPromptText.gameObject.SetActive(true);
        }

        /// <summary>
        /// Returns whether the shop is currently open.
        /// </summary>
        public bool IsShopOpen() => _isShopOpen;

        #endregion
    }
}
