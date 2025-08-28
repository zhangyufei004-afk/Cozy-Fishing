using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingGame.PlayerCustomization
{
    /// <summary>
    /// Allows the player to preview and rotate their character model in the customization UI.
    /// </summary>
    public class CharacterPreviewRotator : MonoBehaviour
    {
        [Tooltip("Transform of the character model to rotate.")]
        [SerializeField] private Transform modelTransform;

        [Tooltip("Rotation speed multiplier.")]
        [SerializeField] private float rotationSpeed = 1.0f;

        [Tooltip("Reference to the Input Action Asset.")]
        [SerializeField] private InputActionAsset inputActions;

        private InputAction _mousePressAction;
        private InputAction _mouseDeltaAction;
        private bool _isDragging = false;

        private void OnEnable()
        {
            // Enable action map
            var playerMap = inputActions.FindActionMap("Player");
            playerMap.Enable();

            // Get actions
            _mousePressAction = playerMap.FindAction("MousePress");
            _mouseDeltaAction = playerMap.FindAction("MouseDelta");

            // Subscribe to press events
            _mousePressAction.started += ctx => _isDragging = true;
            _mousePressAction.canceled += ctx => _isDragging = false;
        }

        private void OnDisable()
        {
            _mousePressAction.started -= ctx => _isDragging = true;
            _mousePressAction.canceled -= ctx => _isDragging = false;
        }

        private void Update()
        {
            if (_isDragging && modelTransform != null)
            {
                Vector2 delta = _mouseDeltaAction.ReadValue<Vector2>();
                // Rotate around Y-axis based on horizontal mouse movement
                modelTransform.Rotate(Vector3.up, delta.x * rotationSpeed, Space.World);
            }
        }
    }
}
