using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Unity.VisualScripting;

namespace FishingGame.Reeling
{
    /// <summary>
    /// This class uses the IReelingGame interface
    /// The slider minigame involves a bar that consists of a fish icon and a catchbox icon
    /// The player must attempt to keep the catchbox icon over the fish icon
    /// The fish icon will attempt to randomly move around
    /// The player has a max amount of time they can spend before it is a fail
    /// </summary>
    public class SliderMiniGame : MonoBehaviour, IReelingMinigame
    {
        #region Public Variables

        #endregion

        #region Private Fields

        private FishScriptableObject _fishData;

        [SerializeField]
        [Tooltip("Reference to the ReelingMaster script.")]
        private ReelingMaster reelingMaster;

        [SerializeField]
        private GameObject sliderCanvas;

        private bool _isMinigameActive = false;
        private float _timerValue = 0f;
        private float _maxTime = 0f;
        private float _catchProgress = 50f;
        private int _catchMax = 100;
        private float _timeSinceLastGoal = 0f;
        private float _maxTimeBetweenGoals = 0f;
        private bool isGoingLeft;

        // Awareness is a difficulty variable, it affects how often a fish can attempt to avoid the catchbox
        // Higher Awareness means a fish will attempt to avoid the player more often
        private int _awareness = 0;

        [SerializeField]
        private Vector3 _fishMoveGoal = Vector3.zero;

        [SerializeField]
        [Tooltip("Scales how much to increase the progress by when the fish is inside of the catchbox")]
        private int catchIncreaseAmount;

        [SerializeField]
        [Tooltip("Scales how much to decrease the progress by when the fish is outside of the catchbox")]
        private int catchDecreaseAmount;

        [SerializeField]
        [Tooltip("Scales how fast the player moves the catchbox with input")]
        private int boxSpeedScalar;

        [SerializeField]
        [Tooltip("The slider that shows the total progress of this minigame")]
        private UnityEngine.UI.Slider progressSlider;

        [SerializeField]
        [Tooltip("This is connected to the catch box (green square) of the ui")]
        private CatchBox uiCatchBoxScript;

        [SerializeField]
        [Tooltip("Transform of the catch box ui element")]
        private RectTransform catchBox;

        [SerializeField]
        [Tooltip("The fish image that the player is trying to catch")]
        private UnityEngine.UI.Image fishImage;

        [SerializeField]
        [Tooltip("Minimum amount of distance fish can move")]
        private int fishMoveMin;

        [SerializeField]
        [Tooltip("Maximum amount of distance fish can move")]
        private int fishMoveMax;

        [SerializeField]
        [Tooltip("Min rotation for the fish icon")]
        private int rotationMin;

        [SerializeField]
        [Tooltip("Max rotation for the fish icon")]
        private int rotationMax;

        [SerializeField]
        [Tooltip("The maximum y axis value the fish icon can have")]
        private float fishMaxYCord;

        [SerializeField]
        [Tooltip("The minimum y axis value the fish icon can have")]
        private float fishMinYCord;

        [SerializeField]
        [Tooltip("The maximum y axis value the catchbox can have")]
        private float catchBoxMaxXCord;

        [SerializeField]
        [Tooltip("The minimum y axis value the catchbox can have")]
        private float catchBoxMinXCord;


        #endregion

        public void Update()
        {
            if (_isMinigameActive == false)
            {
                return;
            }

            _timerValue += Time.deltaTime;
            _timeSinceLastGoal += Time.deltaTime;

            // Check if timer complete
            if (_timerValue >= _maxTime)
            {
                LoseMiniGame();
            }

            // Move player if keys are held

            if (UnityEngine.Input.GetKey(KeyCode.RightArrow))
            {
                MoveCatchIndicator(boxSpeedScalar * Time.deltaTime);
            }

            if (UnityEngine.Input.GetKey(KeyCode.LeftArrow))
            {
                MoveCatchIndicator(-boxSpeedScalar * Time.deltaTime);
            }

            ///////////

            DetermineIfNeedGoal();

            UpdateFishLocation();

            if (uiCatchBoxScript.CheckUIOverlap(fishImage.rectTransform, catchBox))
            {
                ModifyCatchProgress(catchIncreaseAmount);
            }
            else
            {
                ModifyCatchProgress(catchDecreaseAmount);
            }
        }


        /// <summary>
        /// Setups up any variable or field needed for the minigame to run
        /// Difficulty variable from the fishscriptableobject can be used to modify stats
        /// </summary>
        /// <param name="fishScriptable">The data of fish object being caught</param>
        public void InitializeMiniGame(FishScriptableObject fishScriptable) 
        {
            _fishData = fishScriptable;

            fishImage.sprite = fishScriptable.Texture;
            sliderCanvas.SetActive(true);
            _timerValue = 0f;
            _catchProgress = 50;
            _awareness = fishScriptable.FishCatchDifficulty;
            _maxTimeBetweenGoals = 15 - _awareness;
            Vector3 newFishGoal = CreateGoalLocation();
            FishSetGoal(newFishGoal);

            // TODO: Set this to scale based on fish difficulty?
            _maxTime = 30f;


        }

        /// <summary>
        /// Begins the logic of the minigame
        /// </summary>
        public void BeginMiniGame()
        {
            _isMinigameActive = true;
        }

        /// <summary>
        /// Move the catchbox ui element based on player input
        /// Limits the y position based on the catchbox min and max values
        /// </summary>
        /// <param name="moveValue">The value for how far to move</param>
        public void MoveCatchIndicator(float moveValue)
        {
            Vector3 currentPosition = catchBox.transform.localPosition;
            float yPosition = currentPosition.y += moveValue;
            yPosition = Mathf.Clamp(yPosition, catchBoxMinXCord, catchBoxMaxXCord);
            Vector3 newPosition = new Vector3(currentPosition.x, yPosition, currentPosition.z);

            catchBox.localPosition = newPosition;
        }

        private void DetermineIfNeedGoal()
        {
            if (_timeSinceLastGoal >= _maxTimeBetweenGoals)
            {
                Vector3 newFishGoal = CreateGoalLocation();
                FishSetGoal(newFishGoal);
            }
        }

        private Vector3 CreateGoalLocation()
        {
            float fishYLocation = fishImage.transform.localPosition.y;

            float distanceFromLeftSide = fishYLocation - fishMinYCord;
            float distanceFromRightSide = fishYLocation - fishMaxYCord;

            // Go towards the left
            if (distanceFromLeftSide < distanceFromRightSide)
            {
                isGoingLeft = true;
                int randomYPosition = (int)UnityEngine.Random.Range(catchBox.transform.localPosition.y, fishMinYCord);

                Vector3 newGoalLocation = new Vector3(fishImage.transform.localPosition.x, randomYPosition, fishImage.transform.localPosition.z);
                return newGoalLocation;
            }
            else
            {
                isGoingLeft = false;
                int randomYPosition = (int)UnityEngine.Random.Range(catchBox.transform.localPosition.y, fishMaxYCord);

                Vector3 newGoalLocation = new Vector3(fishImage.transform.localPosition.x, randomYPosition, fishImage.transform.localPosition.z);
                return newGoalLocation;
            }
        }

        private void FishSetGoal(Vector3 goalLocation)
        {
            _fishMoveGoal = goalLocation;
        }

        private void UpdateFishLocation()
        {
            if (fishImage.transform.position.y == _fishMoveGoal.y)
            {
                return;
            }

            if (isGoingLeft)
            {
                int randomValue = UnityEngine.Random.Range(fishMoveMin, 0);

                Vector3 currentPosition = fishImage.transform.localPosition;
                Vector3 newPosition = Vector3.MoveTowards(currentPosition, _fishMoveGoal, randomValue * Time.deltaTime);
                fishImage.transform.localPosition = newPosition;
            }
            else
            {
                int randomValue = UnityEngine.Random.Range(0, fishMoveMax);

                Vector3 currentPosition = fishImage.transform.localPosition;
                Vector3 newPosition = Vector3.MoveTowards(currentPosition, _fishMoveGoal, randomValue * Time.deltaTime);
                fishImage.transform.localPosition = newPosition;
            }
        }

        /// <summary>
        /// Randomly moves the fish ui element
        /// Limits the y position based on the Min and Max fishMove values
        /// </summary>
        private void MoveFish()
        {
            int randomValue = UnityEngine.Random.Range(fishMoveMin, fishMoveMax);

            Vector3 currentPosition = fishImage.transform.localPosition;
            float yPosition = Mathf.Clamp(currentPosition.y += randomValue * Time.deltaTime, fishMoveMin, fishMoveMax);
            Vector3 newPosition = new Vector3(currentPosition.x, yPosition, currentPosition.z);

            int rotationRandomValue = UnityEngine.Random.Range(rotationMin, rotationMax);
            fishImage.transform.rotation *= Quaternion.Euler(0, 0, rotationRandomValue * Time.deltaTime);

            fishImage.transform.localPosition = newPosition;
        }

        /// <summary>
        /// Modifys the progress bar for this minigame
        /// Checks if the minigame has been won
        /// </summary>
        /// <param name="valueToAdd">The value for how much to change the catch bar</param>
        private void ModifyCatchProgress(float valueToAdd)
        {
            _catchProgress += valueToAdd * Time.deltaTime;
            progressSlider.value = _catchProgress;

            if (CheckIfCatchWon())
            {
                WinMiniGame();
            }
        }

        /// <summary>
        /// Returns true if _catchProgress is greater or equal to the max progress value
        /// </summary>
        private bool CheckIfCatchWon()
        {
            if (_catchProgress >= _catchMax){ return true; }
            else { return false; }
        }

        /// <summary>
        /// Sets ui elements to not be active
        /// Tells the Reelingmaster minigame has been won
        /// </summary>
        public void WinMiniGame()
        {
            _isMinigameActive = false;
            sliderCanvas.SetActive(false);
            reelingMaster.EndCurrentMiniGame(true);
        }

        /// <summary>
        /// Sets ui elements to not be active
        /// Tells the Reelingmaster minigame has been lost
        /// </summary>
        public void LoseMiniGame()
        {
            _isMinigameActive = false;
            sliderCanvas.SetActive(false);
            reelingMaster.EndCurrentMiniGame(false);
        }
    }
}
