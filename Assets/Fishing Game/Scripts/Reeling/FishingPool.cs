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

        [Header("Pool Stats")]

        [SerializeField]
        [Tooltip("How much fish are in this pool, the higher the value the more fish")]
        private int amountOfFishHeld;

        [SerializeField]
        [Tooltip("How much trash/how polluted this pool is, the higher the value the more polluted")]
        private int amountOfTrash;

        [SerializeField]
        [Tooltip("The max amount of fish/trash that can be in this pool")]
        private int maxAmountOfPopulation;

        [SerializeField]
        [Tooltip("Should this pool have its fish count be able to be depleted from after fishing")]
        private bool populationLowerable;

        [SerializeField]
        [Tooltip("Reference to the gametime script running")]
        private InGameTime timeScript;

        [SerializeField]
        [Tooltip("What type of location is this fishing area")]
        private EFishingLocation fishingLocation;

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

        private List<FishScriptableObject> baseFishList;
        private List<TrashScriptable> baseTrashList;

        // The type of fish that is spawned when the location is infested
        private FishScriptableObject _infestationFish;

        private GameManager _gameManager;

        #endregion

        private void OnEnable()
        {
            _gameManager = GameManager.Instance;

            CreateFishList();
            CreateTrashList();
        }

        /// <summary>
        /// Checks if fish pool is empty and then determines the fish type caught
        /// Randomly selects a fish type based on the amount of types in the pool
        /// </summary>
        /// <returns>Returns the data of the fish being caught</returns>
        public IFishAble GetFishableCaught()
        {
            ETimeOfDay timeCaught = timeScript.GetTimePeriod();
            string locationCaught = gameObject.name;

            if (overrideFishList.Count > 0)
            {
                int randomFish = Random.Range(0, overrideFishList.Count);
                Fish overRideFish = new Fish(overrideFishList[randomFish], timeCaught, locationCaught);
                return overRideFish;
            }

            if (isInfested)
            {
                Fish evilFishData = new Fish(_infestationFish, timeCaught, locationCaught);
                return evilFishData;
            }

            if (CatchFishOrTrash())
            {
                int fishTypeAmount = baseFishList.Count;
                int fishCaughtIndex = Random.Range(0, fishTypeAmount);
                FishScriptableObject fishCaught = baseFishList[fishCaughtIndex];
                Fish fishData = new Fish(fishCaught, timeCaught, locationCaught);
                return fishData;
            }
            else
            {
                int trashTypeAmount = baseTrashList.Count;
                int trashCaughtIndex = Random.Range(0, trashTypeAmount);
                TrashScriptable trashCaught = baseTrashList[trashCaughtIndex];
                Trash trashData = new Trash(trashCaught, timeCaught, locationCaught);
                return trashData;
            }
        }

        /// <summary>
        /// This is a public function that is called to reduce the amount of fish or trash currently in the pool
        /// </summary>
        public void ObjectCaught(IFishAble objectCaught)
        {
            if (objectCaught.GetCatchType() == ECatchableType.Fish)
            {
                amountOfFishHeld -= 1;
            }
            else if (objectCaught.GetCatchType() == ECatchableType.Trash)
            {
                amountOfTrash -= 1;
            }

            if (IsEmpty()) { EmptyPool(); }
        }

        /// <summary>
        /// Checks if fishing pool is empty or not and then returns true if so otherwise false
        /// </summary>
        /// <returns>Returns true if fishing pool is empty, otherwise false</returns>
        public bool IsEmpty()
        {
            if (amountOfFishHeld + amountOfTrash == 0) { return true; }
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

            List<FishScriptableObject> potentialFish = _gameManager.GetPossibleFishList();
            List<FishScriptableObject> fishAvailable = new List<FishScriptableObject>();

            foreach (FishScriptableObject fish in potentialFish)
            {
                if (fish.LocationsFound.Contains(fishingLocation) && fish.IsInvasive)
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
        /// Adds a fish data object to the base fishable fish list
        /// This means that the item added is able to be fished up from this pool
        /// even if its typical stats dont match the pool
        /// </summary>
        /// <param name="fishTypeToAdd">The fish type to add</param>
        public void AddToFishableFishList(FishScriptableObject fishTypeToAdd)
        {
            baseFishList.Add(fishTypeToAdd);
        }

        /// <summary>
        /// Adds a trash data object to the base fishable trash list
        /// This means that the item added is able to be fished up from this pool
        /// even if its typical stats dont match the pool
        /// </summary>
        /// <param name="trashTypeToAdd">The trash type to add</param>
        public void AddTrashToFishableTrashList(TrashScriptable trashTypeToAdd)
        {
            baseTrashList.Add(trashTypeToAdd);
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

        public void AddFishPopulation()
        {
            if (amountOfFishHeld + amountOfTrash <= maxAmountOfPopulation)
            {
                amountOfFishHeld += 1;
            }
            else { Debug.Log("Max amount of fish reached"); }
        }

        /// <summary>
        /// Returns a difficulty in the range of this pools lowest and highest fish difficulty
        /// </summary>
        /// <returns>An integer value represneting a difficulty inbetween this pools lowest and highest potential difficulty</returns>
        public int GetADifficultyInRange()
        {
            return Random.Range(lowestFishDifficulty, highestFishDifficulty);
        }

        private void CreateFishList()
        {
            List<FishScriptableObject> potentialFish = _gameManager.GetPossibleFishList();
            List<FishScriptableObject> fishAvailable = new List<FishScriptableObject>();

            foreach (FishScriptableObject fish in potentialFish)
            {
                if (fish.LocationsFound.Contains(fishingLocation))
                {
                    if (fish.FishCatchDifficulty <= highestFishDifficulty && fish.FishCatchDifficulty >= lowestFishDifficulty)
                    {
                        fishAvailable.Add(fish);
                    }
                }
            }

            baseFishList = fishAvailable;
        }

        /// <summary>
        /// Creates a list of all fishable trash in this level based on the list held by the game manager
        /// Filters that list based on pools location and difficulty
        /// </summary>
        private void CreateTrashList()
        {
            List<TrashScriptable> potentialTrash = _gameManager.GetPossibleTrashList();
            List<TrashScriptable> trashAvailable = new List<TrashScriptable>();

            foreach (TrashScriptable trash in potentialTrash)
            {
                if (trash.LocationsFound.Contains(fishingLocation))
                {
                    if (trash.TrashDifficulty <= highestFishDifficulty && trash.TrashDifficulty >= lowestFishDifficulty)
                    {
                        trashAvailable.Add(trash);
                    }
                }
            }

            baseTrashList = trashAvailable;
        }

        /// <summary>
        /// Decides if the caught object is a fish or trash
        /// If it returns true it is a fish otherwise it returns false
        /// meaning it is a trash object
        /// </summary>
        /// <returns></returns>
        private bool CatchFishOrTrash()
        {
            int rolledNumber = Random.Range(0, amountOfTrash + amountOfFishHeld);

            if (amountOfFishHeld >= rolledNumber)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Currently empty, this will contain logic for what to do upon becoming empty
        /// TODO: Add event here so the emptying of a pool can be tied to a quest
        /// </summary>
        private void EmptyPool()
        {

        }
    }
}
