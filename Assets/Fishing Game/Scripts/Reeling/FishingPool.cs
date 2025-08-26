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

        [SerializeField]
        [Tooltip("Reference to the gametime script running")]
        private IngameTime timeScript;

        [SerializeField]
        private PossibleFish levelsPossibleFish;

        [SerializeField]
        [Tooltip("What type of location is this fishing area")]
        private EFishingLocation _fisingLocation;

        [SerializeField]
        [Tooltip("The lowest difficulty a fish from this area can have")]
        private int lowestFishDifficulty;

        [SerializeField]
        [Tooltip("The highest difficulty a fish from this area can have")]
        private int highestFishDifficulty;

        [SerializeField]
        [Tooltip("Infested pools only spawn invasive fish untill they are depleted")]
        private bool isInfested = false;

        // The type of fish that is spawned when the location is infested
        [SerializeField]
        private FishScriptableObject infestationFish;

        #endregion

        public void OnEnable()
        {
            BecomeInfested();
        }

        /// <summary>
        /// Checks if fish pool is empty and then determines the fish type caught
        /// Randomly selects a fish type based on the amount of types in the pool
        /// </summary>
        public Fish DetermineFishCaught()
        {
            if (CheckIfEmpty() == true) { return null; }

            ETimeOfDay timeCaught = timeScript.GetTimePeriod();
            string locationCaught = gameObject.name;

            List<FishScriptableObject> potentialFish = levelsPossibleFish.GetPossibleFishList();
            List<FishScriptableObject> fishAvailable = new List<FishScriptableObject>();

            if (isInfested)
            {
                Fish evilFishData = new Fish(infestationFish, timeCaught, locationCaught);
                return evilFishData;
            }

            foreach (FishScriptableObject fish in potentialFish)
            {
                if (fish.LocationsFound.Contains(_fisingLocation))
                {
                    if (fish.FishCatchDifficulty <= highestFishDifficulty && fish.FishCatchDifficulty >= lowestFishDifficulty)
                    {
                        fishAvailable.Add(fish);
                    }
                }
            }

            int fishTypeAmount = fishAvailable.Count;
            int fishCaughtIndex = Random.Range(0, fishTypeAmount);
            FishScriptableObject fishCaught = fishAvailable[fishCaughtIndex];
            

            Fish fishData = new Fish(fishCaught, timeCaught, locationCaught);
            return fishData;
        }

        /// <summary>
        /// This is a public function that is called to reduce the amount of fish currently in the pool
        /// It will also destroy the pool if the pool no longer has catchable fish.
        /// </summary>
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

        public void BecomeInfested()
        {
            isInfested = true;

            List<FishScriptableObject> potentialFish = levelsPossibleFish.GetPossibleFishList();
            List<FishScriptableObject> fishAvailable = new List<FishScriptableObject>();

            foreach (FishScriptableObject fish in potentialFish)
            {
                if (fish.LocationsFound.Contains(_fisingLocation) && fish.IsInvasive)
                {
                    fishAvailable.Add(fish);
                }
            }

            int fishTypeAmount = fishAvailable.Count;
            int fishCaughtIndex = Random.Range(0, fishTypeAmount);
            FishScriptableObject invasiveFish = fishAvailable[fishCaughtIndex];
            infestationFish = invasiveFish;
        }
    }
}
