using System;
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

        private void OnEnable()
        {
            _gameEvents = new GameEvents();
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            _instance = this;
        }
    }
}