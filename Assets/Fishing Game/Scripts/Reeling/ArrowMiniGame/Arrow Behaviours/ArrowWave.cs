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
        private EMovementDirection ArrowTypeToSpawn;

        [SerializeField]
        [Tooltip("If this arrow has a custom speed set it here, if this is 0 it will default to normal arrow speed which is 400")]
        private float customSpeed;

        public float GetTimeToSpawn() { return timeToSpawnInSeconds; }

        public EMovementDirection GetArrowType () { return ArrowTypeToSpawn; }

        public float GetCustomSpeed() { return customSpeed; }  

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
