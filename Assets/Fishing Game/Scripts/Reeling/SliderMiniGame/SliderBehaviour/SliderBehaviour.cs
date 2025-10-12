using UnityEngine;

namespace FishingGame.Reeling
{
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
        [Tooltip("The speed to move at when going to this location, 0 will use default speed of 200")]
        private float speedToUse;

        public float GetTimeToSpendOnGoal() { return timeToSpendOnGoal; }

        public float GetLocationToMoveTo() { return locationToMoveTo; }

        public float GetSpeedToUse()  { return speedToUse; }
    }
}
