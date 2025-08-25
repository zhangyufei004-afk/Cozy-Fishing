using FishingGame.FishSystem;
using FishingGame.Inventory;
using FishingGame.Player;
using FishingGame.UI.Inventory;
using System.Collections;
using System.Collections.Generic;
using FishingGame.GameManagement;
using UnityEngine;

namespace FishingGame.Reeling
{

    /// <summary>
    /// This class manages what minigames are currently active during the reeling process.
    /// It contains a list of all available minigame types, controls what one is currently playing
    /// and contains the logic to switch from one minigame to another.
    /// </summary>
    public class ReelingMaster : MonoBehaviour
    {
        [Tooltip("This is a public variable that can be referenced to check if the player is currently fishing")]
        public bool IsFishing { get; private set; } 


        #region Private Fields

        // Minigame stats and fields \\
        private GameObject _currentMinigame;
        private int _miniGameWinsRequired = 1;
        private int _currentMiniGameWins;
        private bool _hasWon = false;
        private int _fishDifficulty;
        private Fish _currentlyReelingFish;
        private FishingPool _currentFishPool;

        //TODO: Can combine this likely with the other fish variable once I have scriptable objects working
        private GameObject _currentFish3DObject;

        [SerializeField]
        [Tooltip("A reference to the initiation script attatched to player.")]
        private ReelingInitiation initiationScript;

        [SerializeField]
        [Tooltip("A reference to the fishing hook script which is attatched to a fishing rod.")]
        private FishingHook fishingHook;

        [SerializeField]
        [Tooltip("A reference to the inventory system.")]
        private InventorySystem inventoryScript;

        // Unity dosen't support making interface types a list so this is a gameobject list
        [SerializeField]
        [Tooltip("A list of all potential minigames.")]
        private List<GameObject> miniGameTypes;

        //***************************\\

        [SerializeField]
        [Tooltip("A reference to the character controller")]
        private PlayerController characterController;

        [SerializeField]
        [Tooltip("The wintext contained in ReelingUI.")]
        private GameObject winText;

        [SerializeField]
        [Tooltip("The losetext contained in ReelingUI.")]
        private GameObject loseText;

        [SerializeField]
        [Tooltip("The timer UI element contained in ReelingUI.")]
        private GameObject timerObject;

        // TODO: Remove this once not needed with new inventory setup
        [SerializeField]
        [Tooltip("A reference to the inventory UI.")] 
        private InventoryUI _inventoryUI;
        #endregion

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
            _currentFishPool = null;

            _currentFish3DObject = fish3DObject;

            DisableControls(true);

            initiationScript.InitiateFishingPerspective();

            //Reset properties for the new catch
            _currentlyReelingFish = fishCaught;
            _fishDifficulty = _currentlyReelingFish.GetFishCatchDifficulty();
            _currentMinigame = null;
            _currentMiniGameWins = 0;
            timerObject.SetActive(true);

            loseText.SetActive(false);
            winText.SetActive(false);

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
            _currentFishPool = fishPool;

            _currentFish3DObject = fish3DObject;

            DisableControls(true);

            initiationScript.InitiateFishingPerspective();

            //Reset properties for the new catch
            _currentlyReelingFish = fishCaught;
            _fishDifficulty = _currentlyReelingFish.GetFishCatchDifficulty();
            _currentMinigame = null;
            _currentMiniGameWins = 0;
            timerObject.SetActive(true);

            loseText.SetActive(false);
            winText.SetActive(false);

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
        /// Returns true if the fish difficulty of the current fish is above 0
        /// </summary>
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
            initiationScript.ShouldEnableFishPerspective(false);
            timerObject.SetActive(false);
            fishingHook.PullBackHook();


            Destroy(_currentFish3DObject);


            // TODO: Implement more logic on if reeling was a win or not
            if (didWin == false)
            {
                loseText.SetActive(true);
                StartCoroutine(HideUIAfterCatch());
            }
            else
            {
                // Check if this was from a fishing pool
                if (_currentFishPool != null)
                {
                    _currentFishPool.FishCaught();
                }
                inventoryScript.AddItem(_currentlyReelingFish);
                GameManager.Instance.GameEvents.FishCaught();

                _currentlyReelingFish = null;

                winText.SetActive(true);
                StartCoroutine(HideUIAfterCatch());
            }

            _currentlyReelingFish = null;
        }

        /// <summary>
        /// Hides the ui shown after completiting a reel
        /// </summary>
        private void HideReelFinishedUI()
        {
            winText.SetActive(false);
            loseText.SetActive(false);
        }           

        /// <summary>
        /// Enables or disables controls on the character controller
        /// Public function
        /// </summary>
        /// <param name="isDisabled">True means the controls should be disabled, otherwise false</param>
        public void DisableControls(bool isDisabled)
        {
            characterController.ToggleMovement(!isDisabled);
        }

        /// <summary>
        /// Starts a timer that will then run the HideReelFinishedUI() function
        /// Also resets the position of the hook, this avoids camera freaking out as it is attatched to the hook
        /// </summary>
        private IEnumerator HideUIAfterCatch()
        {
            yield return new WaitForSeconds(2);
            HideReelFinishedUI();
            //fishingHook.ResetHookSpot();
        }
    }
}
