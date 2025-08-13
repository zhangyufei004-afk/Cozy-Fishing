using System.Collections.Generic;
using UnityEngine;

namespace FishingGame.FishLog
{
    /// <summary>
    /// Manages the UI of the fish log.
    /// </summary>
    public class FishLogUI : MonoBehaviour
    {
        [SerializeField] private FishLogSystem fishLogSystem;

        [Tooltip("All fish log entries already placed in the scene.")]
        [SerializeField] private FishLogUIEntry[] allEntries;

        private Dictionary<FishScriptableObject, FishLogUIEntry> _entryDict;

        private void Awake()
        {
            _entryDict = new Dictionary<FishScriptableObject, FishLogUIEntry>();

            foreach (FishLogUIEntry entry in allEntries)
            {
                if (entry.GetFishData() != null)
                {
                    _entryDict[entry.GetFishData()] = entry;
                    entry.MarkAsCaught(fishLogSystem.HasCaughtFish(entry.GetFishData()));
                }
            }
        }

        private void OnEnable()
        {
            fishLogSystem.FishCaughtForFirstTime += OnFishCaught;
            RefreshAllEntries(); // Fix: refresh when UI becomes visible
        }

        private void OnDisable()
        {
            fishLogSystem.FishCaughtForFirstTime -= OnFishCaught;
        }

        private void OnFishCaught(FishScriptableObject fish)
        {
            if (_entryDict.TryGetValue(fish, out FishLogUIEntry entry))
            {
                entry.MarkAsCaught(true);
            }
        }

        /// <summary>
        /// Refresh all fish entries according to the current caught status in FishLogSystem.
        /// </summary>
        private void RefreshAllEntries()
        {
            foreach (KeyValuePair<FishScriptableObject, FishLogUIEntry> pair in _entryDict)
            {
                FishScriptableObject fishData = pair.Key;
                FishLogUIEntry entry = pair.Value;
                bool isCaught = fishLogSystem.HasCaughtFish(fishData);
                entry.MarkAsCaught(isCaught);
            }
        }
    }
}
