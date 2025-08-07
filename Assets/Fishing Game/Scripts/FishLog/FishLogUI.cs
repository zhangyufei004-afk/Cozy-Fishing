using System.Collections.Generic;
using UnityEngine;

namespace FishingGame.FishLog
{
    /// <summary>
    /// Manages the UI of the fish log.
    /// </summary>
    public class FishLogUI : MonoBehaviour
    {
        [SerializeField] private FishLogSystem _fishLogSystem;

        [Tooltip("All fish log entries already placed in the scene.")]
        [SerializeField] private FishLogUIEntry[] _allEntries;

        private Dictionary<FishScriptableObject, FishLogUIEntry> _entryDict;

        private void Awake()
        {
            _entryDict = new Dictionary<FishScriptableObject, FishLogUIEntry>();

            foreach (var entry in _allEntries)
            {
                if (entry.GetFishData() != null)
                {
                    _entryDict[entry.GetFishData()] = entry;
                    entry.MarkAsCaught(_fishLogSystem.HasCaughtFish(entry.GetFishData()));
                }
            }
        }

        private void OnEnable()
        {
            _fishLogSystem.FishCaughtForFirstTime += OnFishCaught;
            RefreshAllEntries(); // Fix: refresh when UI becomes visible
        }

        private void OnDisable()
        {
            _fishLogSystem.FishCaughtForFirstTime -= OnFishCaught;
        }

        private void OnFishCaught(FishScriptableObject fish)
        {
            if (_entryDict.TryGetValue(fish, out var entry))
            {
                entry.MarkAsCaught(true);
            }
        }

        /// <summary>
        /// Refresh all fish entries according to the current caught status in FishLogSystem.
        /// </summary>
        private void RefreshAllEntries()
        {
            foreach (var pair in _entryDict)
            {
                var fishData = pair.Key;
                var entry = pair.Value;
                bool isCaught = _fishLogSystem.HasCaughtFish(fishData);
                entry.MarkAsCaught(isCaught);
            }
        }
    }
}
