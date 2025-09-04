using System.Collections.Generic;
using FishingGame.FishSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        #region Fish info screen variables
        [SerializeField]
        [Tooltip("The parent of the individual fish ui elements")]
        private GameObject individualFishParent;

        [SerializeField]
        [Tooltip("The image that shows what fish is being looked at")]
        private Image fishImage;

        [SerializeField]
        [Tooltip("The textbox that says the species name")]
        private TextMeshProUGUI speciesNameText;

        [SerializeField]
        [Tooltip("The textbox that shows the species bio")]
        private TextMeshProUGUI speciesBioText;


        #endregion

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
            fishLogSystem.FishCaughtForFirstTime += OnFishCaught;
        }

        private void OnEnable()
        {
            RefreshAllEntries(); // Fix: refresh when UI becomes visible
        }

        /// <summary>
        /// Enables and disables required UI elements to change the display to show whatever fish was clicked
        /// </summary>
        public void FishEntryClicked(FishScriptableObject fishData)
        {
            gameObject.SetActive(false);
            individualFishParent.SetActive(true);

            fishImage.sprite = fishData.Texture;
            speciesNameText.text = fishData.SpeciesName;
            speciesBioText.text = fishData.FishBio;
        }

        private void OnFishCaught(FishScriptableObject fish)
        {
            if (_entryDict.TryGetValue(fish, out FishLogUIEntry entry))
            {
                entry.MarkAsCaught(true);
            }
            fishLogSystem.FishCaughtForFirstTime += OnFishCaught;
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
