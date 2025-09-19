using FishingGame.FishSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FishingGame.Reeling
{
    /// <summary>
    /// The realistic minigame simulates spinning a reel
    /// The player has to spin the reel either clockwise or anti clockwise by clicking and dragging a dragable UI image
    /// That dragable is tied to this class through the dragableScript
    /// This class controls the progress and backend data logic
    /// NOTE: THIS IS A WIP, it functions but there is a lot of work to still go into this minigame
    /// Several inefficient functions currently present to get this working in a low amount of time
    /// </summary>
    public class RealisticMiniGameMaster : MonoBehaviour, IReelingMinigame
    {
        [Header("Script References")]

        [SerializeField]
        [Tooltip("Reference to the ReelingMaster script.")]
        private ReelingMaster reelingMaster;

        [SerializeField]
        [Tooltip("The dragable circle")]
        private RealisticDragable dragableScript;

        [Header("UI elements")]

        [SerializeField]
        [Tooltip("The gameobject that holds the UI in it. This is a child of reelingUI.")]
        private GameObject realisticCanvas;

        [SerializeField]
        [Tooltip("The UI slider that shows the progress of the minigame.")]
        private Slider progressSlider;

        [SerializeField]
        [Tooltip("The centerpoint of the rod in the UI")]
        private Image centerPoint;

        [SerializeField]
        [Tooltip("The Direction indicator for what way a player needs to spin the reel")]
        private Image textDirectionHolder;

        [Header("GameData")]

        [SerializeField]
        [Tooltip("The default value for how much progress is lost per second spinning wrong way")]
        private float defaultDecayValue;

        private int _fishDifficulty;
        private float _progressValue;
        private float _progressMaxValue;

        private bool _goClockWise = false;
        private bool _miniGameActive = false;

        private void Update()
        {
            if (_miniGameActive != true) { return; }

            if (CheckIfDragableInRightDirection() && dragableScript.GetIsMovingValue() != false)
            {
                float speed = dragableScript.GetSpeed() * Time.deltaTime;
                AddToProgressSlider(speed);
            }
            else
            {
                RemoveFromProgressSlider(defaultDecayValue * Time.deltaTime);
            }
        }

        #region Public Functions

        /// <summary>
        /// Initializes the minigame, setting the catchdifficulty and runs the initiation functions
        /// </summary>
        /// <param name="fishScriptable">Data of fish being caught</param>
        public void InitializeMiniGame(Fish fishScriptable)
        {
            _fishDifficulty = fishScriptable.GetFishCatchDifficulty();
            realisticCanvas.SetActive(true);

            InitializeRunTimeData();
        }

        /// <summary>
        /// Begins the minigame
        /// </summary>
        public void BeginMiniGame()
        {
            _miniGameActive = true;
        }

        /// <summary>
        /// Runs the Endminigame function telling it the minigame was lost
        /// </summary>
        public void LoseMiniGame()
        {
            EndMiniGame(false);
        }

        /// <summary>
        /// Runs the win minigame function telling it the minigame was won
        /// </summary>
        public void WinMiniGame()
        {
            EndMiniGame(true);
        }

        /// <summary>
        /// Returns the center image point, this is the middle point of the reel UI and is used to calculate the 
        /// mouse direction
        /// </summary>
        /// <returns>The center point UI object</returns>
        public Image GetCentreImage()
        {
            return centerPoint;
        }

        #endregion

        #region Runtime Functions

        /// <summary>
        /// Adds the inputed float value to the progress slider
        /// If the player is going clockwise the value is reversed from a negative to a positive
        /// </summary>
        /// <param name="progressToAdd">The amount of progress to add</param>
        private void AddToProgressSlider(float progressToAdd)
        {
            // TEMP VALUE TO MAKE NOT TAKE TOO LONG will be balanced in future
            progressToAdd *= 3;
            if (_goClockWise)
            {
                progressToAdd = -progressToAdd;
            }
            progressSlider.value += progressToAdd;
            _progressValue += progressToAdd;

            if (CheckIfWon())
            {
                WinMiniGame();
            }
        }

        /// <summary>
        /// Takes inputed float value away from current progress
        /// </summary>
        /// <param name="progressToRemove">Value of progress to remove</param>
        private void RemoveFromProgressSlider(float progressToRemove)
        {
            progressSlider.value -= progressToRemove;
            _progressValue -= progressToRemove;
        }

        /// <summary>
        /// Deactivates the minigame, takes a bool parameter that will be used to tell the reeling master
        /// if this minigame was won or not. Parameter = true means minigame was won otherwise false
        /// </summary>
        /// <param name="didWin">True if minigame was won otherwise false</param>
        private void EndMiniGame(bool didWin)
        {
            _miniGameActive = false;
            realisticCanvas.SetActive(false);
            reelingMaster.EndCurrentMiniGame(didWin);
        }

        /// <summary>
        /// Decides what direction player must spin in by rolling a random value between 0 and 2
        /// Rolling a 0 = clockwise
        /// Rolling a 1 = anti-clockwise
        /// </summary>
        private void DecideDirection()
        {
            int rolledNumber = Random.Range(0, 2);
            Debug.Log(rolledNumber);
            if (rolledNumber == 0)
            {
                _goClockWise = true;
            }
            else { _goClockWise = false; }
        }

        /// <summary>
        /// Sets the text that tells the player what direction to spin in
        /// Paremeter bool is used to decide what text to set
        /// true = Clockwise, False = anti-clockwise
        /// </summary>
        /// <param name="isClockwise">True = clockwise, false = anti-clockwise</param>
        private void SetTextForDirection(bool isClockwise)
        {
            if (isClockwise)
            {
                textDirectionHolder.GetComponentInChildren<TextMeshProUGUI>().text = "Go clockwise!";
            }
            else
            {
                textDirectionHolder.GetComponentInChildren<TextMeshProUGUI>().text = "Go anti-clockwise!";
            }
        }

        /// <summary>
        /// Returns true if the dragable is currently being dragged in the right direction
        /// </summary>
        /// <returns>True if dragged in right direction otherwise false</returns>
        private bool CheckIfDragableInRightDirection()
        {
            bool goingClockwise = dragableScript.GetCurrentDirection();

            if (goingClockwise == _goClockWise)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the progress value is equal to the max progress value, if so return true
        /// </summary>
        /// <returns>True if enough progress has been met</returns>
        private bool CheckIfWon()
        {
            if (_progressValue >= _progressMaxValue) { return true; }
            else { return false; }
        }


        #endregion

        #region Initilization Functions

        /// <summary>
        /// Runs all the required functions that setup initial minigame variables
        /// </summary>
        private void InitializeRunTimeData()
        {
            ResetGameTimeVariables();
            DifficultyScalars();
            DecideDirection();
            SetTextForDirection(_goClockWise);
        }

        /// <summary>
        /// Resets any variables that might change during gametime
        /// </summary>
        private void ResetGameTimeVariables()
        {
            _progressValue = 0f;
            progressSlider.value = _progressValue;
        }

        /// <summary>
        /// Applies any difficulty scalars based on fish difficulty
        /// </summary>
        private void DifficultyScalars()
        {
            // TODO: Add some difficulty scalars here
            _progressMaxValue = 100;
            progressSlider.maxValue = _progressMaxValue;
        }

        #endregion
    }
}
