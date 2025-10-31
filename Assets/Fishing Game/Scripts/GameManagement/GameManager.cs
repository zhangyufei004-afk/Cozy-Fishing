using FishingGame.FishSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingGame.GameManagement
{
    /// <summary>
    /// Game Manager class. Responsible for all things related to the Game. Also stores references to the GameEvents
    /// class - a class which stores game wide events for scripts to subscribe to.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance => _instance;
        public GameEvents GameEvents => _gameEvents;
        
        private static GameManager _instance;
        private GameEvents _gameEvents;
        
        [Header("Input Properties")]
        [SerializeField] private PlayerInput playerInput;

        [Header("Fishing Properties")]
        
        [SerializeField]
        [Tooltip("A list of all potential fish in this level")]
        private List<FishScriptableObject> potentialFishTypes;

        [SerializeField]
        [Tooltip("A list of all potential trash in this level")]
        private List<TrashScriptable> potentialTrashTypes;
        
        [Header("Loading Screen Properties")]

        [SerializeField] private List<String> toolTips = new List<string>{"Baits increase your chances of catching certain fish"};

        private Dictionary<FishScriptableObject, int> _fishTimesCaught;
        private Dictionary<FishScriptableObject, float> _fishBiggestCatch;
        

        private void OnEnable()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }

            _instance = this;
            UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
            _gameEvents = new GameEvents();

            _instance.GameEvents.OnFishCaught += AddToTimesCaught;
            _instance.GameEvents.OnFishCaught += CheckBiggestCatch;

            _fishTimesCaught = new Dictionary<FishScriptableObject, int>();
            _fishBiggestCatch = new Dictionary<FishScriptableObject, float>();

            foreach (FishScriptableObject fishData in potentialFishTypes)
            {
                _fishTimesCaught.Add(fishData, 0);
                _fishBiggestCatch.Add(fishData, 0);
            }
        }

        private void Update()
        {
            Debug.Log(GetCurrentControlScheme());
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            } 
        }

        /// <summary>
        /// Returns the list of fish available in this level
        /// </summary>
        /// <returns>A list of fish available in this level</returns>
        public List<FishScriptableObject> GetPossibleFishList()
        {
            return potentialFishTypes ;
        }

        /// <summary>
        /// Returns the list of trash available in this level
        /// </summary>
        /// <returns>A list of trash available in this level</returns>
        public List<TrashScriptable> GetPossibleTrashList()
        {
            return potentialTrashTypes;
        }

        /// <summary>
        /// Returns the amount of times a fish type has been caught
        /// </summary>
        /// <param name="fishToCheck">The fish type being checked</param>
        /// <returns>Amount of times this fish type has been caught as an int value</returns>
        public int GetFishTimesCaught(FishScriptableObject fishToCheck)
        {
            return _fishTimesCaught[fishToCheck];
        }

        /// <summary>
        /// Returns the biggest size ever caught of the inputed fishtype
        /// </summary>
        /// <param name="fishToCheck">Fish type to check</param>
        /// <returns>The biggest ever caught size as a float</returns>
        public float GetBiggestCaught(FishScriptableObject fishToCheck)
        {
            return _fishBiggestCatch[fishToCheck];
        }

        /// <summary>
        /// Gets a randomised death tip from the predefined messages for the player.
        /// </summary>
        /// <returns>The string of the death tip.</returns>
        public string GetRandomDeathTip()
        {
            return toolTips[UnityEngine.Random.Range(0, toolTips.Count)];
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public InputDevice GetCurrentControlScheme()
        {
            return playerInput.devices[0];
        }

        /// <summary>
        /// Updates a fishtype to show it has been caught an additional time
        /// This is tied to the onfishcaught event
        /// </summary>
        /// <param name="fishToAddTo">Fish being caught</param>
        private void AddToTimesCaught(Fish fishToAddTo)
        {
            if (fishToAddTo.GetBase() is FishScriptableObject fishData)
            {
                _fishTimesCaught[fishData] += 1;
            }
        }

        /// <summary>
        /// Checks if the caught fish is bigger than the current biggest caught fish of this type
        /// Updates biggest caught fish if so
        /// </summary>
        /// <param name="newFish">The fish being caught</param>
        private void CheckBiggestCatch(Fish newFish)
        {
           if (newFish.GetBase() is FishScriptableObject fishData && newFish.GetWeight() > _fishBiggestCatch[fishData])
           {
                _fishBiggestCatch[fishData] = newFish.GetWeight();
           }
        }

    }
}