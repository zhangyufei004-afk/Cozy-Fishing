using System.Collections.Generic;
using FishingGame.FishSystem;
using FishingGame.GameManagement;
using FishingGame.GameTime;
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
        [Tooltip("The image that shows what fish is being looked at")]
        private Image fishImage;

        [SerializeField]
        [Tooltip("The textbox that says the species name")]
        private TextMeshProUGUI speciesNameText;

        [SerializeField]
        [Tooltip("The textbox that shows the species bio")]
        private TextMeshProUGUI speciesBioText;

        [SerializeField]
        [Tooltip("The textbox that shows the location text")]
        private TextMeshProUGUI location;

        [SerializeField]
        [Tooltip("The textbox that shows the time texts")]
        private TextMeshProUGUI timeText;

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
        /// Updates Log Info display to show whatever fish was clicked
        /// </summary>
        /// <param name="fishData">The fish type clicked</param>
        public void FishEntryClicked(FishScriptableObject fishData)
        {
            fishImage.sprite = fishData.Texture;
            bool hasBeenCaught = CheckIfFishHasBeenCaught(fishData);

            fishImage.color = hasBeenCaught ? Color.white : Color.black;

            speciesNameText.text = hasBeenCaught ? fishData.Name : "???";
            speciesBioText.text = hasBeenCaught ? fishData.Biography : "???";
            isInvasiveText.text = hasBeenCaught ? (fishData.IsInvasive ? "Invasive: Yes" : "Invasive: No") : "Invasive: ???";

            location.text = hasBeenCaught ? SetupLocationTexts(fishData) : "???";
            timeText.text = hasBeenCaught ? SetupTimeTexts(fishData) : "???";

            amountCaughtText.text = hasBeenCaught ? ("Amount Caught: " + _gameManager.GetFishTimesCaught(fishData)) : "Amount Caught: n/a";
            biggestCatchText.text = hasBeenCaught ? ("Biggest Catch: " + _gameManager.GetBiggestCaught(fishData) + " kg") : "Biggest Caught: n/a";
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
        /// Sets up and returns the location text ui elements.
        /// </summary>
        /// <param name="fishData">The fish data that is being used</param>
        /// <returns> String with the concatenated Locations </returns>
        private string SetupLocationTexts(FishScriptableObject fishData)
        {
            List<EFishingLocation> fishLocations = fishData.LocationsFound;
            string fishLocationsString = fishLocations[0].ToString();

            for (int i = 1; i < fishLocations.Count; i++)
            {
                fishLocationsString += ", " + fishLocations[i].ToString();
            }

            return fishLocationsString;
        }

        /// <summary>
        /// Sets up and returns the time UI text elements
        /// </summary>
        /// <param name="fishData">The fish data being used for the UI</param>
        /// <returns> String with the concatenated Times </returns>
        private string SetupTimeTexts(FishScriptableObject fishData)
        {
            List<ETimeOfDay> timesFound = fishData.TimesFound;
            string fishTimesString = timesFound[0].ToString();

            for (int i = 1; i < timesFound.Count; i++)
            {
                fishTimesString += ", " + timesFound[i].ToString();
            }

            return fishTimesString;
        }
    }
}
