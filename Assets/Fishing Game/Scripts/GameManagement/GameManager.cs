using FishingGame.FishSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

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

        [SerializeField]
        [Tooltip("A list of all potential fish in this level")]
        private List<FishScriptableObject> potentialFishTypes;

        private void OnEnable()
        {
            _gameEvents = new GameEvents();
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            _instance = this;
        }

        /// <summary>
        /// Returns the list of fish available in this level
        /// </summary>
        /// <returns>A list of fish available in this level</returns>
        public List<FishScriptableObject> GetPossibleFishList()
        {
            return potentialFishTypes;
        }
    }
}