using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace PrototypeFishingMechanics
{
    public class FishSpawner : MonoBehaviour
    {
        #region Constants

        // In Seconds
        private const float SPAWNER_RATE = 7f;
        private const float INITIAL_SPAWN_DELAY = 3f;

        #endregion
        
        #region Private Fields

        #region Serialized Fields

        [SerializeField] private GameObject fishPrefab;
        [FormerlySerializedAs("fishPath")] [SerializeField] private List<Transform> fishPathTransforms;

        #endregion
        
        

        #endregion

        private void Start()
        {
            InvokeRepeating(nameof(SpawnFish), INITIAL_SPAWN_DELAY, SPAWNER_RATE);
        }

        void SpawnFish()
        {
            GameObject fish = Instantiate(fishPrefab);
            fish.transform.position = transform.position;
            
            FishAI fishAI = fish.GetComponent<FishAI>();
            
            // FISH PATHFIND ADD WAYPOINTS
            foreach (var waypointTransform in fishPathTransforms)
            {
                fishAI.AddPathWaypoint(waypointTransform);
            }

            fishAI.ShouldPath = true;
        }
    }
}