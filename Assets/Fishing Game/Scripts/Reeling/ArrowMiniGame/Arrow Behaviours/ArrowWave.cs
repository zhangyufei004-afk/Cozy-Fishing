using System;
using UnityEngine;

namespace FishingGame.Reeling
{
    /// <summary>
    /// This struct represents an arrow wave entry, it contains the type of arrow to spawn
    /// the time it should spawn and a bool value to check if this is spawned at the same time as another arrow entry or not
    /// </summary>
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

        /// <summary>
        /// Returns the time to spawn in seconds (float) of this entry
        /// </summary>
        /// <returns>The time to spawn in seconds (float)</returns>
        public float GetTimeToSpawn() { return timeToSpawnInSeconds; }

        /// <summary>
        /// Returns the type of arrow this entry is as a enum
        /// </summary>
        /// <returns>An EMovementDirection representing the type of arrow of this entry</returns>
        public EMovementDirection GetArrowType () { return arrowTypeToSpawn; }

        /// <summary>
        /// Returns if this is a double or not
        /// </summary>
        /// <returns>True if this is a double</returns>
        public bool GetIsDouble() { return isDouble; }

        /// <summary>
        /// Overrides the comparision so entrys can be ordered based on their float timetospawninseconds values
        /// </summary>
        /// <param name="obj">The object being compared (should be an ArrowWaveEntry</param>
        /// <returns>The float timeToSpawn value compared between this and the object inputed</returns>
        /// <exception cref="ArgumentException">The inputed object was not an ArrowWaveType</exception>
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
