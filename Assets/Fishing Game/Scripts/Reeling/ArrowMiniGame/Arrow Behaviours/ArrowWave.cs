using UnityEngine;

namespace FishingGame.Reeling
{
    [System.Serializable]
    public struct ArrowWaveEntry
    {
        [SerializeField]
        [Tooltip("How many seconds passed should this arrow be spawned")]
        private float timeToSpawnInSeconds;
        [SerializeField]
        [Tooltip("The type of arrow spawned")]
        private EMovementDirection arrowTypeToSpawn;

    }




}
