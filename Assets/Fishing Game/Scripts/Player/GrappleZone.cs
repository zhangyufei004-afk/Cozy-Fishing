using UnityEngine;

namespace FishingGame.Player
{
    /// <summary>
    /// Defines a grapple zone. When the player enters/exits the zone,
    /// it triggers events so PlayerController can handle Grapple prompts and mode.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class GrappleZone : MonoBehaviour
    {
        // === Events ===
        public delegate void PlayerEnteredZone(PlayerController player);
        public delegate void PlayerExitedZone(PlayerController player);

        /// <summary>
        /// Event fired when a player enters the grapple zone.
        /// </summary>
        public event PlayerEnteredZone OnPlayerEntered;

        /// <summary>
        /// Event fired when a player exits the grapple zone.
        /// </summary>
        public event PlayerExitedZone OnPlayerExited;

        private void Awake()
        {
            // Ensure collider is a trigger
            Collider col = GetComponent<Collider>();
            if (!col.isTrigger)
                col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null && player.HasGrappleHook)
            {
                Debug.Log($"Player entered GrappleZone: {gameObject.name}");
                OnPlayerEntered?.Invoke(player);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                Debug.Log($"Player exited GrappleZone: {gameObject.name}");
                OnPlayerExited?.Invoke(player);
            }
        }
    }
}
