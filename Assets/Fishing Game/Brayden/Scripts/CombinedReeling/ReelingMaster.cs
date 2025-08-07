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

        // Unity does not support interfaces being serialized fields so this is public
        [SerializeField]
        [Tooltip("A list of all potential minigames.")]
        private List<GameObject> _miniGameTypes;

        // The current fish being caught
        private Fish _currentlyReelingFish;

        // FishDifficulty will be taken from the fish being captured
        private int _fishDifficulty;

        #endregion 

        public void Start()
        {
            SetNextMiniGame();
        }


        /*
         * Begin Catch is run immeaditly once a fish collides with the players fishing rod
         * it initializes the catch process
         */
        public void BeginCatch(Fish fishCaught)
        {
            _currentlyReelingFish = fishCaught;

            _fishDifficulty = fishCaught._FishCatchDifficulty;

            _currentMinigame = null;

            // Check if the fish is strong enough for minigames to be ran
            if (CheckIsFishDifficult() == true)
            {
                SetNextMiniGame();
                _currentMinigame.GetComponent<IReelingMinigame>().BeginMiniGame();
            }
            else
            {
                EndCatch();
            }
        }

        private void SetNextMiniGame()
        {
            int index = Random.Range(0, _miniGameTypes.Count);

            GameObject testNextMiniGame = _miniGameTypes[index];

            // Check if next minigame is not the current minigame
            // Note: Going to try find a more efficient way to do this if I have time
            // Not sure rerunning the function in the event of an overlap is the best way to do it
            // 5/08/2025 - Brayden
            if (testNextMiniGame != _currentMinigame)
            {
                testNextMiniGame.GetComponent<IReelingMinigame>().InitializeMiniGame();
                _currentMinigame = testNextMiniGame;
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


        /*
         * Ran once fishing ends
         * Will deinitialize the catch process
         */
        private void EndCatch()
        {
            _currentlyReelingFish = null;
            _currentMinigame = null;
        }


    }
}
