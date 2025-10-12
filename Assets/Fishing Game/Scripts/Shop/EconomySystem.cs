using System;
using UnityEngine;

namespace FishingGame.Economy
{
    /// <summary>
    /// Handles the player's money and raises events when it changes.
    /// Singleton that handles the player's money and raises events when it changes.
    /// </summary>
    public class EconomySystem : MonoBehaviour
    {
        public static EconomySystem Instance { get; private set; }

        /// <summary>
        /// Event fired whenever the player's money changes.
        /// Passes the new money value as argument.
        /// </summary>
        public event Action<int> MoneyChanged;

        [SerializeField] private int startingMoney = 100;
        private int _playerMoney;

        /// <summary>Current amount of player's money.</summary>
        public int PlayerMoney => _playerMoney;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _playerMoney = startingMoney;
        }

        /// <summary>
        /// Try to spend money. Returns true if successful.
        /// </summary>
        public bool SpendMoney(int amount)
        {
            if (amount <= 0) return false;
            if (_playerMoney < amount) return false;

            _playerMoney -= amount;
            MoneyChanged?.Invoke(_playerMoney);
            return true;
        }

        /// <summary>
        /// Add money to the player.
        /// </summary>
        public void AddMoney(int amount)
        {
            if (amount <= 0) return;
            _playerMoney += amount;
            MoneyChanged?.Invoke(_playerMoney);
        }

        /// <summary>
        /// Reset money (for debugging or new game).
        /// </summary>
        public void ResetMoney(int amount)
        {
            _playerMoney = Mathf.Max(0, amount);
            MoneyChanged?.Invoke(_playerMoney);
        }
    }
}
