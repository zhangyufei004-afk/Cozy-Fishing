using System;
using UnityEngine;

namespace FishingGame.Reeling
{
    [System.Serializable]
    public struct ArrowWaveEntry : IComparable
    {
        [SerializeField]
        [Tooltip("How many seconds passed should this arrow be spawned")]
        private float timeToSpawnInSeconds;

        [SerializeField]
        [Tooltip("The type of arrow spawned")]
        private EMovementDirection arrowTypeToSpawn;

        [SerializeField]
        [Tooltip("Is this being spawned with another arrow")]
        private bool isDouble;

        public float GetTimeToSpawn() { return timeToSpawnInSeconds; }

        public EMovementDirection GetArrowType () { return arrowTypeToSpawn; }

        public bool GetIsDouble() { return isDouble; }

        public int CompareTo(object obj) 
        {
            if (obj is ArrowWaveEntry other)
            {
                return timeToSpawnInSeconds.CompareTo(other.timeToSpawnInSeconds);
            }

            throw new ArgumentException("Object is not an ArrowWaveType");
        }
    }




}
