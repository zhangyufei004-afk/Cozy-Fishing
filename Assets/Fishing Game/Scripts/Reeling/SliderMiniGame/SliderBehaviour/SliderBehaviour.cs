using UnityEngine;

namespace FishingGame.Reeling
{
    /// <summary>
    /// This struct represents a data entry for the slider minigame
    /// It containts 3 key variables that need to be set for each entry
    /// timeToSpendOnGoal: Represents how many seconds a fish should spend on this goal location
    /// locationToMoveTo: Represents what point on the bar this fish should move to for this entry
    /// speedToUse: Represents how fast the fish should go when moving to this location
    /// </summary>
    [System.Serializable]
    public struct SliderBehaviour
    {
        [SerializeField]
        [Tooltip("How many seconds passed until this should move to the next goal")]
        private float timeToSpendOnGoal;

        [Range(-297.3f, 297.6f)]
        [SerializeField]
        [Tooltip("The value for what location this should move to")]
        private float locationToMoveTo;

        [SerializeField]
        [Tooltip("The speed to move at when going to this location, 0 will use default speed of 100")]
        private float speedToUse;

        /// <summary>
        /// Returns the time to spend on goal variable for this slider entry
        /// </summary>
        /// <returns>The time to spend on goal variable for this slider entry</returns>
        public float GetTimeToSpendOnGoal() { return timeToSpendOnGoal; }

        /// <summary>
        /// Returns the location to move to variable for this slider entry
        /// </summary>
        /// <returns>The location to move to variable for this slider entry</returns>
        public float GetLocationToMoveTo() { return locationToMoveTo; }

        /// <summary>
        /// Returns the speed to use variable for this slider entry
        /// </summary>
        /// <returns>The speed to use variable for this slider entry</returns>
        public float GetSpeedToUse()  { return speedToUse; }
    }
}
