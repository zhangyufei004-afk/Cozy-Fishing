using FishingGame.FishSystem;
using System.Collections;
using TMPro;
using TMPro.Examples;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

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
        #region Private Fields

        private Fish _fishData;

        [SerializeField]
        [Tooltip("Reference to the ReelingMaster script.")]
        private ReelingMaster reelingMaster;

        [SerializeField]
        [Tooltip("The UI gameobject that parents the UI.")]
        private GameObject sliderCanvas;

        private bool _isMinigameActive = false;
        private float _timerValue = 0f;
        private float _maxTime = 0f;
        private float _catchProgress = 50f;
        private int _catchMax = 100;
        private float _timeSinceLastGoal = 0f;
        private float _maxTimeBetweenGoals = 0f;
        private bool _isGoingLeft;
        private float _catchBoxScale;
        private float _catchBoxVelocity = 0f;
        private float _fightBackVelocity = 0f;

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

        // NOTE: All UI transform modifications in this script use Y for left to right
        // This is because the ui image has been rotated by default

        [SerializeField]
        [Tooltip("Transform of the catch box ui element")]
        private RectTransform catchBox;

        [SerializeField]
        [Tooltip("The middle point of the bar.")]
        private float middleBarPoint;

        [SerializeField]
        [Tooltip("How strong the fight back force on the box is.")]
        private float fightBackSpeed;

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

        [SerializeField]
        [Tooltip("The UI text that shows how much time is left")]
        private TextMeshProUGUI timerText;

        private InputAction _directionAction;

        #endregion

        private void OnEnable()
        {
            InputActionAsset inputActions = InputSystem.actions;
            InputActionMap uiActionMap = inputActions.FindActionMap("UI");
            uiActionMap.Enable();
            _directionAction = uiActionMap.FindAction("Navigate");
        }

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

            MovementFightBack();

            // Move player if keys are held

            if (_directionAction.ReadValue<Vector2>().x > 0)
            {
                SetPlayerVelocity(boxSpeedScalar, false);
            }

            if (_directionAction.ReadValue<Vector2>().x < 0)
            {
                SetPlayerVelocity(-boxSpeedScalar, true);
            }

            MoveCatchBox();

            DetermineIfNeedGoal();
            UpdateFishLocation();
            SetFishDirection();
            UpdateTimer();

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
        /// 
        /// Difficulty modifiers:
        /// The initial catch progress is 55, each level of difficulty reduces the initial progress by 5 i.e a difficulty of 2 will result in an initial progress of 45
        /// </summary>
        /// <param name="fishScriptable">The data of fish object being caught</param>
        public void InitializeMiniGame(Fish fishScriptable) 
        {
            _fishData = fishScriptable;

            fishImage.sprite = _fishData.GetTexture();
            sliderCanvas.SetActive(true);
            _timerValue = 0f;
            _maxTimeBetweenGoals = 1;
            Vector3 startLocation = CreateGoalLocation();
            fishImage.transform.localPosition = startLocation;
            Vector3 newFishGoal = CreateGoalLocation();
            FishSetGoal(newFishGoal);

            // Scaling variables based on difficulty
            _catchProgress = Mathf.Clamp(55 - 5 * fishScriptable.GetFishCatchDifficulty(), 0, 100);
            _catchBoxScale = Mathf.Clamp(1.5f - 0.1f * fishScriptable.GetFishCatchDifficulty(), 0.5f, 1.5f);
            SetCatchBoxYScale(_catchBoxScale);

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
        /// This will cause the catchbox to try and fight back against the player
        /// It will run IsLeftSide to check what side it is closest to then add some acceleration in that direction
        /// TODO: Look into balancing this game mode more with feature like this, ran out of time for vertical slice
        /// </summary>
        private void MovementFightBack()
        {
            if (IsLeftSide())
            {
               //Mathf.Max(_fightBackVelocity += fightBackSpeed * Time.deltaTime, catchBoxFightBackSpeedMax);
            }
            else
            {
                //Mathf.Min(_fightBackVelocity -= fightBackSpeed * Time.deltaTime, -catchBoxFightBackSpeedMax);
            }
        }

        /// <summary>
        /// Compares localPosition to the middle point of the UI bar.
        /// Returns true if closer to left, false if closer to right
        /// </summary>
        /// <returns> True if closer to left, false if closer to right</returns>
        private bool IsLeftSide()
        {
            if (middleBarPoint > catchBox.transform.localPosition.y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Move the catchbox ui element based on player input
        /// Limits the y position based on the catchbox min and max values
        /// </summary>
        /// <param name="moveValue">The value for how far to move</param>
        private void SetPlayerVelocity(float accelerationValue, bool isGoingLeft)
        {
            if (isGoingLeft  && _catchBoxVelocity < 0)
            {
                _catchBoxVelocity = 0;
            }
            else if (!isGoingLeft && _catchBoxVelocity > 0)
            {
                _catchBoxVelocity = 0;
            }

            _catchBoxVelocity += accelerationValue * Time.deltaTime;
        }

        private void MoveCatchBox()
        {
            Vector3 currentPosition = catchBox.transform.localPosition;
            float yPosition = currentPosition.y += _catchBoxVelocity;
            yPosition = Mathf.Clamp(yPosition, catchBoxMinXCord, catchBoxMaxXCord);
            Vector3 newPosition = new Vector3(currentPosition.x, yPosition, currentPosition.z);
            catchBox.localPosition = newPosition;
        }

        /// <summary>
        /// Checks the time since last goal was set to determine if it has been long enough to set a new goal
        /// If it has been long enough this function will then create a new goal location through CreateGoalLocation()
        /// and then set it through FishSetGoal.
        /// </summary>
        private void DetermineIfNeedGoal()
        {
            if (_timeSinceLastGoal >= _maxTimeBetweenGoals)
            {
                Vector3 newFishGoal = CreateGoalLocation();
                FishSetGoal(newFishGoal);
            }
        }

        /// <summary>
        /// This function will generate a location goal for the fish to move to and return it.
        /// It will find a random location that does not collide with the catchbox and set that as the new
        /// goal.
        /// </summary>
        /// <returns>Vector3 newGoalLocation</returns>
        private Vector3 CreateGoalLocation()
        {
            Vector3 currentPosition = fishImage.transform.localPosition;

            float randomYPosition = UnityEngine.Random.Range(fishMinYCord, fishMaxYCord);

            Vector3 newGoal = new Vector3(currentPosition.x, randomYPosition, currentPosition.z);
            return newGoal;
        }

        /// <summary>
        /// Takes a Vector3 goal local variable, sets the _fishMoveGoal to equal this
        /// Resets the time since last goal variable
        /// </summary>
        /// <param name="goalLocation">The new location to move to</param>
        private void FishSetGoal(Vector3 goalLocation)
        {
            _timeSinceLastGoal = 0;
            _fishMoveGoal = goalLocation;
        }

        /// <summary>
        /// Checks if the fish's current goal is to the left or right of the fish
        /// Sets the animators IsLeft bool parameter based on the direction determined.
        /// </summary>
        private void SetFishDirection()
        {
            if (_fishMoveGoal.y > fishImage.transform.localPosition.y)
            {
                fishImage.GameObject().GetComponent<Animator>().SetBool("IsLeft", false);
            }
            else
            {
                fishImage.GameObject().GetComponent<Animator>().SetBool("IsLeft", true);
            }
        }

        /// <summary>
        /// Moves the fish towards its current goal if it is not already at it
        /// </summary>
        private void UpdateFishLocation()
        {
            if (fishImage.transform.position.y == _fishMoveGoal.y)
            {
                return;
            }

            if (_isGoingLeft)
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
        /// Sets the catchboxes y scale to the inputed float variable
        /// Does not change x or z scale.
        /// </summary>
        /// <param name="newYScale">The value for new y scale</param>
        private void SetCatchBoxYScale(float newYScale)
        {
            Vector3 currentScale = catchBox.transform.localScale;
            Vector3 newScale = new Vector3(currentScale.x, newYScale, currentScale.z);
            catchBox.transform.localScale = newScale;
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

        /// <summary>
        /// Updates the ui timer
        /// </summary>
        public void UpdateTimer()
        {
            timerText.text = ("Time Remaining: " + Mathf.RoundToInt(_maxTime - _timerValue));
        }
    }
}
