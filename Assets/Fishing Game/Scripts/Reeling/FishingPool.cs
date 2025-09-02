using FishingGame.FishSystem;
using FishingGame.GameManagement;
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
    /// </summary>
    public class FishingPool : MonoBehaviour
    {
        #region Private Properties

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

        [SerializeField]
        [Tooltip("This field can be used to force the pool to spawn specific fish instead of randomized")]
        private List<FishScriptableObject> overrideFishList;

        // The type of fish that is spawned when the location is infested
        private FishScriptableObject _infestationFish;

        #endregion

        private void OnEnable()
        {
            GameManager gameManager = GameManager.Instance;
        }

        /// <summary>
        /// Checks if fish pool is empty and then determines the fish type caught
        /// Randomly selects a fish type based on the amount of types in the pool
        /// </summary>
        public Fish DetermineFishCaught()
        {
            ETimeOfDay timeCaught = timeScript.GetTimePeriod();
            string locationCaught = gameObject.name;

            List<FishScriptableObject> potentialFish = levelsPossibleFish.GetPossibleFishList();
            List<FishScriptableObject> fishAvailable = new List<FishScriptableObject>();

            if (isInfested)
            {
                Fish evilFishData = new Fish(_infestationFish, timeCaught, locationCaught);
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
        /// Currently empty, this will contain logic for what to do upon becoming empty
        /// TODO: Add event here so the emptying of a pool can be tied to a quest
        /// </summary>
        private void EmptyPool()
        {

        }

        /// <summary>
        /// This is a public function that is called to reduce the amount of fish currently in the pool
        /// </summary>
        public void FishCaught()
        {
            amountOfFishHeld -= 1;
            if (CheckIfEmpty()) { EmptyPool(); }
        }

        /// <summary>
        /// Returns true if the fishing pool is empty
        /// </summary>
        public bool CheckIfEmpty()
        {
            if (amountOfFishHeld == 0) { return true; }
            else { return false; }
        }

        /// <summary>
        /// Causes a pool to become infested
        /// This will find a random fish that is allowed in the level that is tagged with IsInvasive
        /// That fish will then be chosen to be the only fish that is catchable while the infestation remains
        /// </summary>
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
            _infestationFish = invasiveFish;
        }

        /// <summary>
        /// Adds a fish to the override list.
        /// Takes a bool that tells the function wether it needs to clear the current override list or not
        /// And the fish data to add
        /// </summary>
        /// <param name="clearList">Clear the current ovveride list</param>
        /// <param name="fishToAdd">The data of the fish to add</param>
        public void AddOverRideFish(bool clearList, FishScriptableObject fishToAdd)
        {
            if (clearList)
            {
                overrideFishList.Clear();
            }

            overrideFishList.Add(fishToAdd);
        }
    }
}
