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
        [Tooltip("How many seconds should it take for the next arrow to be spawned")]
        private float timeUntilNextArrow;

        [SerializeField]
        [Tooltip("The type of arrow spawned")]
        private EMovementDirection arrowTypeToSpawn;

        /// <summary>
        /// Returns the time to spawn in seconds (float) of this entry
        /// </summary>
        /// <returns>The time to spawn in seconds (float)</returns>
        public float GetTimeToWaitForNextArrow() { return timeUntilNextArrow; }

        /// <summary>
        /// Returns the type of arrow this entry is as a enum
        /// </summary>
        /// <returns>An EMovementDirection representing the type of arrow of this entry</returns>
        public EMovementDirection GetArrowType () { return arrowTypeToSpawn; }

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
                return timeUntilNextArrow.CompareTo(other.timeUntilNextArrow);
            }

            throw new ArgumentException("Object is not an ArrowWaveType");
        }
    }
}
