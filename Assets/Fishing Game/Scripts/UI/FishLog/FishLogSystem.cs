using System;
using System.Collections.Generic;
using FishingGame.FishSystem;
using UnityEngine;

namespace FishingGame.FishLog
{
    /// <summary>
    /// Manages the fish log and records which fish have been caught.
    /// </summary>
    public class FishLogSystem : MonoBehaviour
    {
        private readonly HashSet<string> _caughtFishIds = new();

        /// <summary>
        /// Event triggered when a new fish is caught for the first time.
        /// </summary>
        public event Action<FishScriptableObject> FishCaughtForFirstTime;

        /// <summary>
        /// Returns whether the fish has been caught before.
        /// </summary>
        public bool HasCaughtFish(FishScriptableObject fish)
        {
            return _caughtFishIds.Contains(fish.ID);
        }

        /// <summary>
        /// Call this when the player catches a fish. If it's the first time.
        /// </summary>
        public void RegisterFishCaught(FishScriptableObject fish)
        {
            if (_caughtFishIds.Add(fish.ID))
            {
                FishCaughtForFirstTime?.Invoke(fish);
            }
        }
    }
}


