using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace PrototypeFishingMechanics
{
    

    public class ReelingMaster : MonoBehaviour
    {

        #region Variables that change every catch

        // The current minigame being played
        private GameObject _currentMinigame;

        private int _miniGameWinsRequired = 4;

        private int _currentMiniGameWins;

        [SerializeField]
        private GameObject characterController;

        private bool _hasWon = false;

        // Unity does not support interfaces being serialized fields so this is public
        [SerializeField]
        [Tooltip("A list of all potential minigames.")]
        private List<GameObject> miniGameTypes;

        // The current fish being caught
        private Fish _currentlyReelingFish;

        // FishDifficulty will be taken from the fish being captured
        private int _fishDifficulty;

        #endregion 

        public void Start()
        {
            SetNextMiniGame();
            // Bellow two are temp lines for testing, delete when no longer needed
            characterController.GetComponent<CharacterMovement>().AllowMovement = false;
        }


        /// <summary>
        /// * Begin Catch is run immeaditly once a fish collides with the players fishing rod
        /// it initializes the catch process
        /// </summary>
        /// <param name="fishCaught">The Fish Scriptable Object which was caught</param>
        public void BeginCatch(Fish fishCaught)
        {
            characterController.GetComponent<CharacterMovement>().AllowMovement = false;

            _currentlyReelingFish = fishCaught;

            _fishDifficulty = _currentlyReelingFish._FishCatchDifficulty;

            _currentMinigame = null;

            _currentMiniGameWins = 0;

            // Check if the fish is strong enough for minigames to be ran
            if (CheckIsFishDifficult() == true)
            {
                _miniGameWinsRequired = SetMiniGamesRequired(_fishDifficulty);
                SetNextMiniGame();
                
            }
            else
            {
                EndCatch();
            }
        }

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
                testNextMiniGame.GetComponent<IReelingMinigame>().InitializeMiniGame();
                _currentMinigame = testNextMiniGame;
                _currentMinigame.GetComponent<IReelingMinigame>().BeginMiniGame();
            }
            else
            {
                SetNextMiniGame();
            }
        }

        public void EndCurrentMiniGame(bool didWin)
        {
            if (didWin == false)
            {
                // TODO: Progress on catch goes down for losing
                SetNextMiniGame();
                return;
            }

            _currentMiniGameWins += 1;

            _hasWon = CheckIfWonEnough();

            if (_hasWon)
            {
                EndCatch();
                return;
            }
            else
            {
                SetNextMiniGame();
            }   
        }


        private bool CheckIsFishDifficult()
        {
            if (_fishDifficulty > 0)
            {
                return true;
            }
            else { return false; }
        }

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

        // Might add more complex logic here at a later stage
        // This is more of a placeholder right now
        private int SetMiniGamesRequired(int fishDifficulty)
        {
            return fishDifficulty += 1;
        }


        /*
         * Ran once fishing ends
         * Will deinitialize the catch process
         */
        private void EndCatch()
        {
            _currentlyReelingFish = null;
            _currentMinigame = null;
            _currentMiniGameWins = 0;
           characterController.GetComponent<CharacterMovement>().AllowMovement = true;
        }


    }
}
