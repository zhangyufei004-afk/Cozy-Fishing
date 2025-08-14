using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

namespace PrototypeFishingMechanics
{
    

    public class ReelingMaster : MonoBehaviour
    {
        #region Public Variables

        // TODO: This variable is currently inplace for _currentlyReelingFish
        // It will eventually be removed once FishScritableObjects can easily be implemented
        // It represents the image shown in the ui of the fish being dragged up
        public GameObject TestFish;

        #endregion


        #region Private Fields

        // Minigame stats and fields \\
        private GameObject _currentMinigame;
        private int _miniGameWinsRequired = 4;
        private int _currentMiniGameWins;
        private bool _hasWon = false;
        private int _fishDifficulty;
        private FishScritableObject _currentlyReelingFish;

        [SerializeField]
        private ReelingInitiation InitiationScript;

        // Unity dosen't support making interface types a list so this is a gameobject list
        [SerializeField]
        [Tooltip("A list of all potential minigames.")]
        private List<GameObject> MiniGameTypes;

        //***************************\\

        [SerializeField]
        private GameObject CharacterController;

        [SerializeField]
        private GameObject WinText;

        [SerializeField]
        private GameObject LoseText;
        #endregion

        public void Start()
        {
           // SetNextMiniGame();
            // Bellow two are temp lines for testing, delete when no longer needed
          //  characterController.GetComponent<CharacterMovement>().AllowMovement = false;
        }

        public void TEMPSTART()
        {
            CharacterController.GetComponent<CharacterMovement>().AllowMovement = false;

            _fishDifficulty = 4;

            _currentMinigame = null;

            _currentMiniGameWins = 0;

            InitiationScript.AllowControls = false;

            LoseText.SetActive(false);
            WinText.SetActive(false);

            // Check if the fish is strong enough for minigames to be ran
            if (CheckIsFishDifficult() == true)
            {
                _miniGameWinsRequired = SetMiniGamesRequired(_fishDifficulty);
                SetNextMiniGame();
            }
            else
            {
                EndCatch(true);
            }
        }


        /// <summary>
        /// * Begin Catch is run immeaditly once a fish collides with the players fishing rod
        /// it initializes the catch process
        /// </summary>
        /// <param name="fishCaught">The Fish Scriptable Object which was caught</param>
        public void BeginCatch(FishScritableObject fishCaught)
        {
            CharacterController.GetComponent<CharacterMovement>().AllowMovement = false;

            _currentlyReelingFish = fishCaught;

            _fishDifficulty = _currentlyReelingFish.FishCatchDifficulty;

            _currentMinigame = null;

            _currentMiniGameWins = 0;

            InitiationScript.AllowControls = false;

            LoseText.SetActive(false);
            WinText.SetActive(false);

            // Check if the fish is strong enough for minigames to be ran
            if (CheckIsFishDifficult() == true)
            {
                _miniGameWinsRequired = SetMiniGamesRequired(_fishDifficulty);
                SetNextMiniGame(); 
            }
            else
            {
                EndCatch(true);
            }
        }

        private void SetNextMiniGame()
        {
            int index = Random.Range(0, MiniGameTypes.Count);

            GameObject testNextMiniGame = MiniGameTypes[index];

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

        public void EndCurrentMiniGame(bool didWin)
        {
            if (didWin == false)
            {
                EndCatch(false);
                return;
            }

            _currentMiniGameWins += 1;

            Vector3 fishPosition = TestFish.transform.position;
            TestFish.transform.position = new Vector3(fishPosition.x, fishPosition.y += 30, fishPosition.z);



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
        private void EndCatch(bool didWin)
        {
            _currentlyReelingFish = null;
            _currentMinigame = null;
            _currentMiniGameWins = 0;
            CharacterController.GetComponent<CharacterMovement>().AllowMovement = true;
            InitiationScript.AllowControls = true;
            InitiationScript.ShouldEnableCamera(false);

            TestFish.SetActive(false);


            // TODO: Implement more logic on if reeling was a win or not
            if (didWin == false)
            {
                LoseText.SetActive(true);
                StartCoroutine(HideUIAfterCatch());
            }

            else
            {
                WinText.SetActive(true);
                StartCoroutine(HideUIAfterCatch());
            }
        }

        private void HideUI()
        {
            WinText.SetActive(false);
            LoseText.SetActive(false);
        }



        private IEnumerator HideUIAfterCatch()
        {
            yield return new WaitForSeconds(2);
            HideUI();
        }
    }
}
