using UnityEngine;

namespace FishingGame.Player
{
    /// <summary>
    /// Defines an area where the player can use the grappling hook.
    /// Now supports a destination point for directional grappling.
    /// Displays an optional visual indicator when the player enters the zone.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class GrappleZone : MonoBehaviour
    {
        [Header("Anchor & Destination Points")]
        [Tooltip("Where the rope connects when grappling.")]
        public Transform anchorPoint;

        [Tooltip("Where the player will be pulled to after grappling.")]
        public Transform destinationPoint;

        [Header("Zone Visuals")]
        [Tooltip("Optional visual object shown when the player is inside the zone.")]
        [SerializeField] private GameObject visualIndicator;
        [Tooltip("Whether the visual indicator should always rotate slowly for feedback.")]
        [SerializeField] private bool rotateIndicator = true;
        [SerializeField] private float rotationSpeed = 25f;
        [Tooltip("Color of the indicator when the player is inside the zone.")]
        [SerializeField] private Color activeColor = new Color(0f, 0.8f, 1f, 0.6f);
        [Tooltip("Color of the indicator when inactive.")]
        [SerializeField] private Color idleColor = new Color(0f, 0.6f, 1f, 0.2f);

        [Header("Zone Detection Settings")]
        [Tooltip("The collider that defines the grappling zone. Must be set to Trigger.")]
        [SerializeField] private Collider zoneCollider;

        public event System.Action<GrappleZone> OnPlayerEntered;
        public event System.Action<GrappleZone> OnPlayerExited;

        private Renderer _indicatorRenderer;
        private bool _playerInside;

        private void Awake()
        {
            if (zoneCollider == null)
                zoneCollider = GetComponent<Collider>();

            if (visualIndicator != null)
            {
                _indicatorRenderer = visualIndicator.GetComponent<Renderer>();
                visualIndicator.SetActive(false);
            }

            if (zoneCollider != null && !zoneCollider.isTrigger)
            {
                Debug.LogWarning($"Collider on {gameObject.name} should be marked as Trigger for GrappleZone to work properly.");
                zoneCollider.isTrigger = true;
            }
        }

        private void Update()
        {
            if (rotateIndicator && visualIndicator != null && visualIndicator.activeSelf)
            {
                visualIndicator.transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInside = true;
                visualIndicator?.SetActive(true);
                SetIndicatorColor(activeColor);
                OnPlayerEntered?.Invoke(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInside = false;
                visualIndicator?.SetActive(false);
                SetIndicatorColor(idleColor);
                OnPlayerExited?.Invoke(this);
            }
        }

        private void SetIndicatorColor(Color color)
        {
            if (_indicatorRenderer != null && _indicatorRenderer.material.HasProperty("_Color"))
            {
                _indicatorRenderer.material.color = color;
            }
        }

#if UNITY_EDITOR
        // Editor gizmos (visualizes zone + anchor → destination path)
        private void OnDrawGizmos()
        {
            if (zoneCollider != null)
            {
                Gizmos.color = new Color(0f, 0.5f, 1f, 0.25f);
                Gizmos.DrawCube(zoneCollider.bounds.center, zoneCollider.bounds.size);
            }

            if (anchorPoint != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(anchorPoint.position, 0.2f);
                Gizmos.DrawLine(transform.position, anchorPoint.position);
            }

            if (destinationPoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(destinationPoint.position, 0.2f);

                if (anchorPoint != null)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(anchorPoint.position, destinationPoint.position);
                }
            }
        }
#endif
    }
}
