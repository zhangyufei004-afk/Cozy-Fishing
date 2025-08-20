using FishingGame.FishSystem;
using FishingGame.GameTime;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace FishingGame.Reeling
{
    /// <summary>
    /// Fishing pools contain types of fish scriptable objects that is fished from them
    /// Player can cast lines into these to begin fishing
    /// These can randomly spawn and have a max amount of fish that can be caught from them
    /// These are designed to be easier to catch from compared to catching and individual swimming fish
    /// </summary>
    public class FishingPool : MonoBehaviour
    {
        #region Private Properties
        [SerializeField]
        [Tooltip("A list of the type of fish that can be caught from this pool")]
        private List<FishScriptableObject> typesOfFishInPool;

        [SerializeField]
        [Tooltip("How much fish this began with, this changes as pool is fished from")]
        private int amountOfFishHeld;

        #endregion

        /// <summary>
        /// Checks if fish pool is empty and then determines the fish type caught
        /// Randomly selects a fish type based on the amount of types in the pool
        /// </summary>
        public Fish DetermineFishCaught()
        {
            if (CheckIfEmpty() == true) { return null; }

            int fishTypeAmount = typesOfFishInPool.Count;
            int fishCaughtIndex = Random.Range(0, fishTypeAmount);
            FishScriptableObject fishCaught = typesOfFishInPool[fishCaughtIndex];
            TimeOfDay tempTimeValue = TimeOfDay.Morning;
            string tempLocation = "TEMPDATAFIELD";

            Fish fishData = new Fish(fishCaught, tempTimeValue, tempLocation);

            return fishData;
        }

        public void FishCaught()
        {
            amountOfFishHeld -= 1;
            if (CheckIfEmpty()) { Destroy(gameObject); }
        }

        /// <summary>
        /// Returns true if the fishing pool is empty
        /// </summary>
        private bool CheckIfEmpty()
        {
            if (amountOfFishHeld == 0) { return true; }
            else { return false; }
        }





    }
}
