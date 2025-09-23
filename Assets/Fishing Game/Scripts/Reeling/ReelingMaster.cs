using FishingGame.FishSystem;
using FishingGame.Inventory;
using FishingGame.Player;
using FishingGame.UI.Inventory;
using System.Collections;
using System.Collections.Generic;
using FishingGame.GameManagement;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace FishingGame.Reeling
{

    /// <summary>
    /// This class manages what minigames are currently active during the reeling process.
    /// It contains a list of all available minigame types, controls what one is currently playing
    /// and contains the logic to switch from one minigame to another.
    /// </summary>
    public class ReelingMaster : MonoBehaviour
    {
        #region Private Variables

        [Header("Script References")]

        [SerializeField]
        [Tooltip("A reference to the initiation script attatched to player.")]
        private ReelingInitiation initiationScript;

        [SerializeField]
        [Tooltip("A reference to the fishing hook script which is attatched to a fishing rod.")]
        private FishingHook fishingHook;

        [SerializeField]
        [Tooltip("A reference to the inventory system.")]
        private InventorySystem inventoryScript;

        [SerializeField]
        [Tooltip("A reference to the character controller")]
        private PlayerController characterController;

        [Header("Minigame Variables")]

        // Unity dosen't support making interface types a list so this is a gameobject list
        [SerializeField]
        [Tooltip("A list of all potential minigames.")]
        private List<GameObject> miniGameTypes;

        private GameObject _currentMinigame;
        private Fish _currentlyReelingFish;
        private FishingPool _currentFishPool;
        private GameObject _currentFish3DObject;

        private int _miniGameWinsRequired = 1;
        private int _currentMiniGameWins;
        private int _fishDifficulty;

        private bool _hasWon = false;

        [Header("UI elements")]

        [SerializeField]
        [Tooltip("The image UI element shown if succsesfully fishing.")]
        private Image caughtFishImage;

        [SerializeField]
        [Tooltip("The textbox that is displayed after fishing")]
        private TextMeshProUGUI fishingFinishedText;

        [SerializeField]
        [Tooltip("The timer UI element contained in ReelingUI.")]
        private GameObject timerObject;

        [SerializeField]
        [Tooltip("The UI button that allows the player to exit from fishing")]
        private Button cancelButton;

        [Header("Misc")]

        [Tooltip("This is a public variable that can be referenced to check if the player is currently fishing")]
        public bool IsFishing { get; private set; }

#endregion

        #region Public Methods

        /// <summary>
        /// Begin catch is run once a player succsesfully lands the fishing rod on a pool or an individual fish
        /// It has two variations, 1 takes a fishscriptableobject and a gameobject
        /// The other variation additionally takes a Fishing pool input.
        /// This variation is to be used for individually caught fish and not fishing pools.
        /// </summary>
        /// <param name="fishCaught">The Fish Scriptable Object which was caught</param>
        /// <param name="fish3DObject">The 3D object of the fish</param>
        public void BeginCatch(Fish fishCaught, GameObject fish3DObject)
        {
            GameManager.Instance.GameEvents.SetPlayerOccupied(true);
            _currentFishPool = null;

            _currentFish3DObject = fish3DObject;
            _currentFish3DObject.GetComponent<Animator>().SetBool("Active", true);

            DisableControls(true);

            initiationScript.InitiateFishingPerspective();

            //Reset properties for the new catch
            _currentlyReelingFish = fishCaught;
            _fishDifficulty = _currentlyReelingFish.GetFishCatchDifficulty();
            _currentMinigame = null;
            _currentMiniGameWins = 0;

            IsFishing = true;

            // Check if the fish is strong enough for minigames to be ran
            if (CheckIsFishDifficult() == true)
            {
                _miniGameWinsRequired = GetMiniGamesRequired(_fishDifficulty);
                SetNextMiniGame(); 
            }
            else
            {
                // TODO: Implement a visual indicator so this debug log is not needed when a catch is not difficult enough
                Debug.Log("DEBUGLOG: This fish was not difficult enough to cause minigames");
                EndCatch(true);
            }
        }

        /// <summary>
        /// Begin catch is run once a player succsesfully lands the fishing rod on a pool or an individual fish
        /// It has two variations, 1 takes a fishscriptableobject and a gameobject
        /// The other variation additionally takes a Fishing pool input.
        /// This variation is to be used for fishing pools. 
        /// </summary>
        /// <param name="fishCaught">The Fish Scriptable Object which was caught</param>
        /// <param name="fish3DObject">The 3D object of the fish</param>
        /// <param name="fishPool">The fish pool being fished from</param>
        public void BeginCatch(Fish fishCaught, GameObject fish3DObject, FishingPool fishPool)
        {
            GameManager.Instance.GameEvents.SetPlayerOccupied(true);

            _currentFishPool = fishPool;

            _currentFish3DObject = fish3DObject;
            _currentFish3DObject.GetComponent<Animator>().SetBool("Active", true);

            DisableControls(true);

            initiationScript.InitiateFishingPerspective();

            //Reset properties for the new catch
            _currentlyReelingFish = fishCaught;
            _fishDifficulty = _currentlyReelingFish.GetFishCatchDifficulty();
            _currentMinigame = null;
            _currentMiniGameWins = 0;

            IsFishing = true;

            // Check if the fish is strong enough for minigames to be ran
            if (CheckIsFishDifficult() == true)
            {
                _miniGameWinsRequired = GetMiniGamesRequired(_fishDifficulty);
                SetNextMiniGame();
            }
            else
            {
                // TODO: Implement a visual indicator so this debug log is not needed when a catch is not difficult enough
                Debug.Log("DEBUGLOG: This fish was not difficult enough to cause minigames");
                EndCatch(true);
            }
        }

        /// <summary>
        /// Ends the current catch and runs logic based on if the player won the minigame or did not
        /// If player won the minigame this checks if they have won enough to have completed the catch
        /// If player lost the catch is immeaditly ended with a loss
        /// </summary>
        /// <param name="didWin">Represents if the player won the minigame or not</param>
        public void EndCurrentMiniGame(bool didWin)
        {
            if (didWin == false)
            {
                EndCatch(false);
                return;
            }

            _currentMiniGameWins += 1;
            Vector3 fishPosition = _currentFish3DObject.transform.position;
            _currentFish3DObject.transform.position = new Vector3(fishPosition.x, fishPosition.y += 0.20f, fishPosition.z);
            _hasWon = CheckIfWonEnough();

            if (_hasWon)
            {
                EndCatch(true);
                return;
            }
            else
            {
                SetNextMiniGame();
            }
        }

        /// <summary>
        /// Run when a fishing location is empty when fished from, rather than begin catch. This will display a text to the player
        /// saying the fishing location is empty. 
        /// </summary>
        /// <param name="fishingLocation">The fishing location that is empty</param>
        public void CaughtNothing(FishingPool fishingLocation)
        {
            string textToDisplay = $"The {fishingLocation.gameObject.name} is empty of fish!";

            fishingFinishedText.text = textToDisplay;
            fishingFinishedText.gameObject.SetActive(true);
            fishingHook.PullBackHook();
            StartCoroutine(HideUIAfterCatch(2));
        }

        /// <summary>
        /// Enables or disables controls on the character controller
        /// Public function
        /// </summary>
        /// <param name="isDisabled">True means the controls should be disabled, otherwise false</param>
        public void DisableControls(bool isDisabled)
        {
            characterController.ToggleMovement(!isDisabled);
            initiationScript.AreReelingControlsActive(!isDisabled);
        }

        /// <summary>
        /// Run by a button, this will cancel fishing
        /// Does this differently based on fishing is in stage one or the minigame section
        /// </summary>
        public void CancelFishing()
        {
            SetCancelButtonVisibilty(false);

            if (IsFishing == true)
            {
                _currentMinigame.GetComponent<IReelingMinigame>().LoseMiniGame();
            }
            else
            {
                initiationScript.CancelStageOne();
            }
        }

        /// <summary>
        /// Sets the cancel button to be visible if parameter is true, otherwise nonvisible
        /// </summary>
        /// <param name="isVisible">If true the button will be visisble, otherwise it will be hidden</param>
        public void SetCancelButtonVisibilty(bool isVisible)
        {
            cancelButton.gameObject.SetActive(isVisible);
        }

        /// <summary>
        /// Returns the current 3D object representing the fish being reeled
        /// </summary>
        /// <returns>The 3D object of the current fish</returns>
        public GameObject GetCurrent3DFishObject()
        {
            return _currentFish3DObject;
        }


        #endregion

        /// <summary>
        /// Randomly selects the next minigame that will be played and then initializes and begins it
        /// The same minigame can not be player two times in a row
        /// </summary>
        private void SetNextMiniGame()
        {
            int index = Random.Range(0, miniGameTypes.Count);

            GameObject testNextMiniGame = miniGameTypes[index];

            // TODO: Check if next minigame is not the current minigame
            // Going to try find a more efficient way to do this if I have time
            // Not sure rerunning the function in the event of an overlap is the best way to do it
            // 5/08/2025 - Brayden
            if (testNextMiniGame != _currentMinigame)
            {
                testNextMiniGame.GetComponent<IReelingMinigame>().InitializeMiniGame(_currentlyReelingFish);
                _currentMinigame = testNextMiniGame;
                _currentMinigame.GetComponent<IReelingMinigame>().BeginMiniGame();
            }
            else
            {
                SetNextMiniGame();
            }
        }

        /// <summary>
        /// Returns true if the fish difficulty of the current fish is above 0
        /// </summary>
        /// <returns>Returns true if the fish difficulty is above 0</returns>
        private bool CheckIsFishDifficult()
        {
            if (_fishDifficulty > 0)
            {
                return true;
            }
            else { return false; }
        }

        /// <summary>
        /// Returns true if the player has won the required amount of minigames
        /// </summary>
        /// <returns>Returns true if the currentMiniGamesWon variables is greater than the minigamewins required variable</returns>
        private bool CheckIfWonEnough()
        {
            if (_currentMiniGameWins > _miniGameWinsRequired)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the amount of minigames required to complete the catch
        /// This is based of the fishes difficulty
        /// </summary>
        /// <param name="fishDifficulty">The difficulty of caught fish</param>
        /// <returns>Returns the amount of minigames that will need to be done based on the inputed fishdifficulty</returns>
        private int GetMiniGamesRequired(int fishDifficulty)
        {
            _miniGameWinsRequired = 1;
            return _miniGameWinsRequired + (fishDifficulty / 2);
        }


        /// <summary>
        /// Ends the catch process, reenabling player controls, reseting perspective and reseting key variables
        /// Enables victory or loss text based on result
        /// </summary>
        /// <param name="didWin">Represents if the minigame was succsesful or not</param>
        private void EndCatch(bool didWin)
        {
            IsFishing = false;
            _currentMinigame = null;
            _currentMiniGameWins = 0;
            DisableControls(false);
            initiationScript.AreReelingControlsActive(true);
            initiationScript.ShouldEnableFishPerspective(false);
            fishingHook.PullBackHook();
            SetCancelButtonVisibilty(false);

            _currentFish3DObject.GetComponent<Animator>().SetBool("Active", false);
            Destroy(_currentFish3DObject);

            GameManager.Instance.GameEvents.SetPlayerOccupied(false);
            // TODO: Implement more logic on if reeling was a win or not
            if (didWin == false)
            {
                DisplayFishingResult(_currentlyReelingFish, false);
                StartCoroutine(HideUIAfterCatch(4));
                _currentlyReelingFish = null;
            }
            else
            {
                // Check if this was from a fishing pool
                if (_currentFishPool != null)
                {
                    _currentFishPool.FishCaught();
                }

                inventoryScript.AddItem(_currentlyReelingFish);
                GameManager.Instance.GameEvents.FishCaught(_currentlyReelingFish);
                DisplayFishingResult(_currentlyReelingFish, true);

                _currentlyReelingFish = null;
                StartCoroutine(HideUIAfterCatch(2));

            }
        }

        /// <summary>
        /// Hides the ui shown after completiting a reel
        /// </summary>
        private void HideReelFinishedUI()
        {
            fishingFinishedText.gameObject.SetActive(false);
            caughtFishImage.gameObject.SetActive(false);
        }       

        /// <summary>
        /// Starts a timer that will then run the HideReelFinishedUI() function
        /// Also resets the position of the hook, this avoids camera freaking out as it is attatched to the hook
        /// </summary>
        private IEnumerator HideUIAfterCatch(int secondsToWait)
        {
            yield return new WaitForSeconds(secondsToWait);
            HideReelFinishedUI();
        }

        /// <summary>
        /// Displays the fishing reslt after a catch is ended. Formats the text based on what is caught
        /// Sets the required UI elements to be active
        /// If the catch was a failure string will be formated to show that the fish got away
        /// </summary>
        /// <param name="fishData">The data of the fish being reeled</param>
        /// <param name="didCatch">Was the fish caught</param>
        private void DisplayFishingResult(Fish fishData, bool didCatch)
        {
            if (didCatch)
            {
                string textToDisplay = $"You have caught a {fishData.GetWeight()}kg {fishData.GetSpeciesName()}!";

                caughtFishImage.sprite = fishData.GetTexture();
                fishingFinishedText.text = textToDisplay;
                caughtFishImage.gameObject.SetActive(true);
                fishingFinishedText.gameObject.SetActive(true);
            }
            else
            {
                string textToDisplay = $"The {fishData.GetSpeciesName()} got away!";
                fishingFinishedText.text = textToDisplay;
                fishingFinishedText.gameObject.SetActive(true);
            }

        }
    }
}
