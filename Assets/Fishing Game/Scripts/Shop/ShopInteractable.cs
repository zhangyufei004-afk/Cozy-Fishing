using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using FishingGame.Player;

namespace FishingGame.Shop
{
    /// <summary>
    /// Handles player interaction with the shop.
    /// Opens/closes shop UI when pressing Interact, auto-closes when leaving range,
    /// and ensures UI buttons are interactable.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ShopInteractable : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ShopController shopController;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private TextMeshProUGUI interactPromptText;

        [Header("Settings")]
        [SerializeField] private float interactDistance = 3f;

        private Transform _player;
        private InputAction _interactAction;
        private bool _isPlayerNearby;
        private bool _isShopOpen;

        private void OnEnable()
        {
            var inputActions = InputSystem.actions;
            inputActions.FindActionMap("Player").Enable();
            inputActions.FindActionMap("UI").Enable();
            _interactAction = inputActions.FindAction("Player/Interact");
        }

        private void Start()
        {
            _player = GameObject.FindWithTag("Player").transform;

            // Hide prompt initially
            if (interactPromptText != null)
                interactPromptText.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_player == null || shopController == null) return;

            // Check distance
            float distance = Vector3.Distance(_player.position, transform.position);
            _isPlayerNearby = distance <= interactDistance;

            // Auto-close shop if player leaves range
            if (_isShopOpen && !_isPlayerNearby)
            {
                CloseShop();
            }

            // Show/hide interact prompt only if shop is closed
            if (interactPromptText != null)
                interactPromptText.gameObject.SetActive(_isPlayerNearby && !_isShopOpen);

            // Toggle shop when interact pressed
            if (_isPlayerNearby && _interactAction != null && _interactAction.WasPressedThisFrame())
            {
                if (_isShopOpen)
                    CloseShop();
                else
                    OpenShop();
            }
        }

        /// <summary>
        /// Opens the shop UI and disables player movement.
        /// </summary>
        private void OpenShop()
        {
            shopController.OpenShopUI();
            playerController.ToggleMovement(false);
            _isShopOpen = true;
        }

        /// <summary>
        /// Closes the shop UI and re-enables player movement and animation.
        /// </summary>
        private void CloseShop()
        {
            shopController.CloseShopUI();
            playerController.ToggleMovement(true); // Ensure animator params are restored in ToggleMovement
            _isShopOpen = false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactDistance);
        }
    }
}


