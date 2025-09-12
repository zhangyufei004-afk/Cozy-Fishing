using System;
using System.Collections.Generic;
using FishingGame.FishSystem;
using FishingGame.GameManagement;
using FishingGame.GameTime;
using TMPro;
using Unity.VisualScripting;
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

        [SerializeField]
        [Tooltip("The list of location text")]
        private List<TextMeshProUGUI> locations;

        [SerializeField]
        [Tooltip("The list of time texts")]
        private List<TextMeshProUGUI> timeTexts;

        [SerializeField]
        [Tooltip("The text showing how many times this has been caught")]
        private TextMeshProUGUI amountCaughtText;

        [SerializeField]
        [Tooltip("The text showing the biggest every caught size in KG")]
        private TextMeshProUGUI biggestCatchText;

        [SerializeField]
        [Tooltip("The text showing if this is invasive or not")]
        private TextMeshProUGUI isInvasiveText;

        private GameManager _gameManager;

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
            _gameManager = GameManager.Instance;
            RefreshAllEntries(); // Fix: refresh when UI becomes visible
        }

        /// <summary>
        /// Enables and disables required UI elements to change the display to show whatever fish was clicked
        /// </summary>
        public void FishEntryClicked(FishScriptableObject fishData)
        {
            if (!CheckIfFishHasBeenCaught(fishData)) { return; }

            gameObject.SetActive(false);
            individualFishParent.SetActive(true);

            fishImage.sprite = fishData.Texture;
            speciesNameText.text = fishData.SpeciesName;
            speciesBioText.text = fishData.FishBio;
            
            if (fishData.IsInvasive){ isInvasiveText.text = "Invasive Fish"; }
            else { isInvasiveText.text = "Noninvasive Fish"; }

            SetupLocationTexts(fishData);
            SetupTimeTexts(fishData);

            int fishCaughtCount = _gameManager.GetFishTimesCaught(fishData);
            amountCaughtText.text = "Total Caught: " + fishCaughtCount;

            float fishBiggestCatch = _gameManager.GetBiggestCaught(fishData);
            biggestCatchText.text = "Biggest Catch: " + fishBiggestCatch + " kg";
        }

        /// <summary>
        /// Sets the fish Entry UI elements to be invisible 
        /// </summary>
        public void HideFishEntry()
        {
            individualFishParent.SetActive(false);

            foreach (TextMeshProUGUI locationText in locations)
            {
                locationText.gameObject.SetActive(false);
            }

            foreach (TextMeshProUGUI timeText in timeTexts)
            {
                timeText.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Sets the fishlogui to be active
        /// </summary>
        public void EnableFishLogUI()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Marks a fish as being caught and then calls the event
        /// </summary>
        /// <param name="fish">The fish caught</param>
        private void OnFishCaught(FishScriptableObject fish)
        {
            if (_entryDict.TryGetValue(fish, out FishLogUIEntry entry))
            {
                entry.MarkAsCaught(true);
            }
            fishLogSystem.FishCaughtForFirstTime += OnFishCaught;
        }

        /// <summary>
        /// Returns true if a specific fish type has been caught before
        /// </summary>
        /// <param name="fish">The fish type to check</param>
        /// <returns>True if fish type has been caught before otherwise false</returns>
        private bool CheckIfFishHasBeenCaught(FishScriptableObject fish)
        {
            if (fishLogSystem.HasCaughtFish(fish)) { return true; }
            else { return false; }
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

        /// <summary>
        /// Sets up the location text ui elements.
        /// </summary>
        /// <param name="fishData">The fish data that is being used</param>
        private void SetupLocationTexts(FishScriptableObject fishData)
        {
            List<EFishingLocation> fishLocations = fishData.LocationsFound;
            List<string> fishLocationsStrings = new List<string>();

            for (int i = 0; i < fishLocations.Count; i++)
            {
                var location = fishLocations[i];
                fishLocationsStrings.Add(location.ToString());
            }

            for (int i = 0; i < fishLocationsStrings.Count; i++)
            {
                locations[i].gameObject.SetActive(true);
                locations[i].text = fishLocationsStrings[i];
            }
        }

        /// <summary>
        /// Sets up the time UI text elements
        /// </summary>
        /// <param name="fishData">The fish data being used for the UI</param>
        private void SetupTimeTexts(FishScriptableObject fishData)
        {
            List<ETimeOfDay> timesFound = fishData.TimesFound;
            List<string> fishTimesStrings = new List<string>();

            for (int i = 0; i < timesFound.Count; i++)
            {
                var time = timesFound[i];
                fishTimesStrings.Add(time.ToString());
            }

            for (int i = 0; i < fishTimesStrings.Count; i++)
            {
                timeTexts[i].gameObject.SetActive(true);
                timeTexts[i].text = fishTimesStrings[i];
            }
        }
    }
}
