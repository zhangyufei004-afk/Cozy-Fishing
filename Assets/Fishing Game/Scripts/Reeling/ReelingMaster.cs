using FishingGame.Player;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Android;

namespace FishingGame.Reeling
{

    /// <summary>
    /// This class manages what minigames are currently active during the reeling process.
    /// It contains a list of all available minigame types, controls what one is currently playing
    /// and contains the logic to switch from one minigame to another.
    /// </summary>
    public class ReelingMaster : MonoBehaviour
    {
        #region Private Fields

        // Minigame stats and fields \\
        private GameObject _currentMinigame;
        private int _miniGameWinsRequired = 4;
        private int _currentMiniGameWins;
        private bool _hasWon = false;
        private int _fishDifficulty;
        private FishScriptableObject _currentlyReelingFish;
        //TODO: Can combine this likely with the other fish variable once I have scriptable objects working
        private GameObject _currentFish3DObject;

        [SerializeField]
        private ReelingInitiation initiationScript;

        [SerializeField]
        private FishingHook fishingHook;

        // Unity dosen't support making interface types a list so this is a gameobject list
        [SerializeField]
        [Tooltip("A list of all potential minigames.")]
        private List<GameObject> miniGameTypes;

        //***************************\\

        [SerializeField]
        private PlayerController characterController;

        [SerializeField]
        private GameObject winText;

        [SerializeField]
        private GameObject loseText;
        #endregion

        /// <summary>
        /// Begin Catch is run immeaditly once a fish collides with the players fishing rod
        /// it initializes the catch process
        /// </summary>
        /// <param name="fishCaught">The Fish Scriptable Object which was caught</param>
        /// /// <param name="fish3DObject">The 3D object of the fish</param>
        public void BeginCatch(FishScriptableObject fishCaught, GameObject fish3DObject)
        {
            _currentFish3DObject = fish3DObject;

            characterController.GetComponent<PlayerController>().AreControlsEnabled = false;

            initiationScript.InitiateFishingPerspective();

            //Resset properties for the new catch
            _currentlyReelingFish = fishCaught;
            _fishDifficulty = _currentlyReelingFish.FishCatchDifficulty;
            _currentMinigame = null;
            _currentMiniGameWins = 0;
            initiationScript.AllowControls = false;

            loseText.SetActive(false);
            winText.SetActive(false);

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
                testNextMiniGame.GetComponent<IReelingMinigame>().InitializeMiniGame(_fishDifficulty);
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
            return fishDifficulty += 1;
        }


        /// <summary>
        /// Ends the catch process, reenabling player controls, reseting perspective and reseting key variables
        /// Enables victory or loss text based on result
        /// </summary>
        /// <param name="didWin">Represents if the minigame was succsesful or not</param>
        private void EndCatch(bool didWin)
        {
            _currentlyReelingFish = null;
            _currentMinigame = null;
            _currentMiniGameWins = 0;
            characterController.GetComponent<PlayerController>().AreControlsEnabled = true;
            initiationScript.AllowControls = true;
            initiationScript.ShouldEnableFishPerspective(false);
            

            _currentFish3DObject.SetActive(false);


            // TODO: Implement more logic on if reeling was a win or not
            if (didWin == false)
            {
                loseText.SetActive(true);
                StartCoroutine(HideUIAfterCatch());
            }
            else
            {
                winText.SetActive(true);
                StartCoroutine(HideUIAfterCatch());
            }
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
        /// Starts a timer that will then run the HideReelFinishedUI() function
        /// Also resets the position of the hook, this avoids camera freaking out as it is attatched to the hook
        /// </summary>
        private IEnumerator HideUIAfterCatch()
        {
            yield return new WaitForSeconds(2);
            HideReelFinishedUI();
            fishingHook.ResetHookSpot();
        }
    }
}
