using System;
using UnityEngine;

namespace FishingGame.GameManagement
{
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