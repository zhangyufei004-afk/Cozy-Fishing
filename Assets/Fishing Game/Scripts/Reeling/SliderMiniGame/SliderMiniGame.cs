using FishingGame.FishSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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

        [Header("Script references")]

        [SerializeField]
        [Tooltip("Reference to the ReelingMaster script.")]
        private ReelingMaster reelingMaster;

        [SerializeField]
        [Tooltip("This is connected to the catch box (green square) of the ui")]
        private CatchBox uiCatchBoxScript;

        [Header("UI elements")]

        // NOTE: All UI transform modifications in this script use Y for left to right
        // This is because the ui image has been rotated by default

        [SerializeField]
        [Tooltip("The UI gameobject that parents the UI.")]
        private GameObject sliderCanvas;

        [SerializeField]
        [Tooltip("The slider that shows the total progress of this minigame")]
        private UnityEngine.UI.Slider progressSlider;

        [SerializeField]
        [Tooltip("The fish image that the player is trying to catch")]
        private UnityEngine.UI.Image fishImage;

        [SerializeField]
        [Tooltip("The right arrow UI indicator")]
        private UnityEngine.UI.Image rightArrow;

        [SerializeField]
        [Tooltip("Transform of the catch box ui element")]
        private RectTransform catchBox;

        [Header("Minigame Data")]

        [SerializeField]
        [Tooltip("The starting x location for the catchbox")]
        private float catchboxYStartLocation;

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
        [Tooltip("How strong the fight back force on the box is.")]
        private float fightBackSpeed;

        [SerializeField]
        [Tooltip("The middle point of the bar.")]
        private float middleBarPoint;

        [SerializeField]
        [Tooltip("The default speed of fish")]
        private int defaultSpeed;

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
        [Tooltip("How many seconds required to increase time scalar")]
        private float suddenDeathTimer;

        [SerializeField]
        [Tooltip("The max speed going left the catchbox can go")]
        private float catchBoxMaxReverseSpeed;

        [SerializeField]
        [Tooltip("Max forward speed of the catch box")]
        private float catchBoxForwardMaxSpeed;

        [SerializeField]
        [Tooltip("The time buffer the player is given before the catchbox starts to move")]
        private float initialTimeToWait;

        private bool _inputHeld = false;
        private bool _goingLeft = true;
        private bool _suddenDeath = false;

        private bool _behaviourLoaded = false;
        private SliderData _sliderData;
        private List<SliderBehaviour> _sliderBehaviourList;
        private int _currentBehaviourIndex = 0;
        private int _initialPointScalar = 5;

        private float _catchProgress = 50f;
        private float _timeSinceLastGoal = 0f;
        private float _maxTimeBetweenGoals = 0f;
        private float _catchBoxVelocity = 0f;
        private float _timePassed;
        private float _catchIncreaseValueToUse;
        private float _catchDecreaseValueToUse;
        private float _speedToUse;
        private float _defaultSuddenDeathTime;

        private int _wanderRange = 50;

        private float _catchMax = 100;


        private IFishAble _reelingObjectData;
        private bool _isMinigameActive = false;
        private bool _isMiniGamePaused = false;
        private bool _isGoingLeft;

        private InputAction _clickAction;

        private Vector3 _fishMoveGoal = Vector3.zero;
        #endregion

        private void OnEnable()
        {
            InputActionAsset inputActions = InputSystem.actions;
            InputActionMap uiActionMap = inputActions.FindActionMap("SliderMiniGame");
            uiActionMap.Enable();
            _clickAction = uiActionMap.FindAction("LeftClick");

            _defaultSuddenDeathTime = suddenDeathTimer;
        }

        private void OnDisable()
        {
            if (_clickAction == null) { return; }
            _clickAction.started -= MouseDown;
            _clickAction.canceled -= MouseUp;
        }

        public void Update()
        {
            if (_isMinigameActive == false) { return; }

            if (_isMiniGamePaused == true) { CheckInitialBufferTimer(); }


            // Right movement
            if (_inputHeld)
            {
                UnPauseCatchboxMovement();
                rightArrow.color = Color.green;
                SetPlayerVelocity(boxSpeedScalar, false);
            }

            _timePassed += Time.deltaTime;

            CheckTimePassed();

            _timeSinceLastGoal += Time.deltaTime;

            if (_catchProgress <= 0)
            {
                LoseMiniGame();
            }

            DetermineIfNeedGoal();
            UpdateFishLocation();
            SetFishDirection();
            
            if (uiCatchBoxScript.CheckUIOverlap(fishImage.rectTransform, catchBox))
            { ModifyCatchProgress(_catchIncreaseValueToUse); }
            else { ModifyCatchProgress(_catchDecreaseValueToUse); }

            if (_isMiniGamePaused) { return; }

            if (!_inputHeld)
            {
                rightArrow.color = Color.white;
                MovementFightBack();
            }

            MoveCatchBox();
        }

        #region Public Functions

        /// <summary>
        /// Setups up any variable or field needed for the minigame to run
        /// Difficulty variable from the fishscriptableobject can be used to modify stats
        /// 
        /// Difficulty modifiers:
        /// The initial catch progress is 55, each level of difficulty reduces the initial progress by 5 i.e a difficulty of 2 will result in an initial progress of 45
        /// </summary>
        /// <param name="fishScriptable">The data of fish object being caught</param>
        public void InitializeMiniGame(IFishAble fishScriptable) 
        {
            _reelingObjectData = fishScriptable;

            InitializeVariables();
            DetermineBehaviour();            
            _isMiniGamePaused = true;
        }

        /// <summary>
        /// Begins the logic of the minigame
        /// </summary>
        public void BeginMiniGame()
        {
            _isMinigameActive = true;
        }

        /// <summary>
        /// Sets ui elements to not be active
        /// Tells the Reelingmaster minigame has been won
        /// </summary>
        public void WinMiniGame()
        {
            StopAllCoroutines();
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
            StopAllCoroutines();
            _isMinigameActive = false;
            sliderCanvas.SetActive(false);
            reelingMaster.EndCurrentMiniGame(false);
        }

        /// <summary>
        /// Returns true if going left, otherwise false
        /// </summary>
        /// <returns>True if fish going left otherwise false</returns>
        public bool GetDirection()
        {
            return fishImage.GameObject().GetComponent<Animator>().GetBool("IsLeft");
        }

        /// <summary>
        /// Gets input bool for audio cues
        /// </summary>
        /// <returns></returns>
        public bool GetInput()
        {
            return _directionAction.ReadValue<Vector2>().x > 0;
        }

        #endregion

        #region BoxMovement

        /// <summary>
        /// This will cause the catchbox to try and fight back against the player
        /// It will run IsLeftSide to check what side it is closest to then add some acceleration in that direction
        /// TODO: Look into balancing this game mode more with feature like this, ran out of time for vertical slice
        /// </summary>
        private void MovementFightBack()
        {
            _catchBoxVelocity += fightBackSpeed * Time.deltaTime;
            // Doubles the Fight back if catchbox is going forward
            if (_catchBoxVelocity > 0)
            {
                _catchBoxVelocity += fightBackSpeed * 2 * Time.deltaTime;
            }

            // Resets velocity if catchbox is against the left edge
            if (Mathf.Approximately(catchBox.transform.localPosition.y, catchBoxMinXCord))
            {
                _catchBoxVelocity = 0;
            }

            // Increase fight back if right at right edge
            if (Mathf.Approximately(catchBox.transform.localPosition.y, catchBoxMaxXCord))
            {
                _catchBoxVelocity += fightBackSpeed * 2 * Time.deltaTime;
            }

            _catchBoxVelocity = Mathf.Clamp(_catchBoxVelocity, catchBoxMaxReverseSpeed, catchBoxForwardMaxSpeed);
        }

        /// <summary>
        /// Move the catchbox ui element based on player input
        /// Limits the y position based on the catchbox min and max values
        /// </summary>
        /// <param name="moveValue">The value for how far to move</param>
        private void SetPlayerVelocity(float accelerationValue, bool isGoingLeft)
        {
            // Resets velocity if catchbox is against the left edge
            if (Mathf.Approximately(catchBox.transform.localPosition.y, catchBoxMinXCord))
            {
                _catchBoxVelocity = 0;
            }

            _catchBoxVelocity += accelerationValue * Time.deltaTime;
        }

        /// <summary>
        /// Moves the catchbox based on current velocity
        /// </summary>
        private void MoveCatchBox()
        {
            Vector3 currentPosition = catchBox.transform.localPosition;
            float yPosition = currentPosition.y += _catchBoxVelocity;
            yPosition = Mathf.Clamp(yPosition, catchBoxMinXCord, catchBoxMaxXCord);
            Vector3 newPosition = new Vector3(currentPosition.x, yPosition, currentPosition.z);
            catchBox.localPosition = newPosition;
        }

        /// <summary>
        /// Sets input held to true
        /// Run when the mouse is pressed down
        /// </summary>
        private void MouseDown(InputAction.CallbackContext inputAction)
        {
            _inputHeld = true;
        }

        /// <summary>
        /// Sets input held to false
        /// Run when the mouse is released
        /// </summary>
        private void MouseUp(InputAction.CallbackContext inputAction)
        {
            _inputHeld = false;
        }


        #endregion

        #region FishMovementCustomBehaviour

        /// <summary>
        /// Setsup the custom slider behaviour
        /// </summary>
        private void SetupCustomBehaviour()
        {
            _behaviourLoaded = true;
            _currentBehaviourIndex = 0;

            _sliderData = new SliderData(_reelingObjectData.GetSliderMinigameBehaviour());
            _sliderBehaviourList = _sliderData.GetSliderBehaviours();

            _catchIncreaseValueToUse = _sliderData.GetPointPerSecond();
            _catchDecreaseValueToUse = -_sliderData.GetPointPerSecond() / 2;

            suddenDeathTimer = _sliderData.GetSuddenDeathTimer();

            // Starting Locations
            Vector3 currentPosition = fishImage.transform.localPosition;
            Vector3 startingLocation = new Vector3(currentPosition.x, _sliderData.GetStartingLocation(), currentPosition.z);
            fishImage.transform.localPosition = startingLocation;

            // Scaling variables based on difficulty
            _catchMax = _sliderData.GetMaxPointsNeeded();
            progressSlider.maxValue = _catchMax;
            _catchProgress = _sliderData.GetPointsToStartWith();


            float initialFishGoal = _sliderBehaviourList[_currentBehaviourIndex].GetLocationToMoveTo();
            Vector3 newGoal = new Vector3(currentPosition.x, initialFishGoal, currentPosition.z);
            _speedToUse = _sliderBehaviourList[_currentBehaviourIndex].GetSpeedToUse();
            if (_speedToUse == 0) { _speedToUse = defaultSpeed; }
            FishSetGoal(newGoal);
            FishSetSpeed(_speedToUse);
            StartCoroutine(CustomBehaviourTime(_sliderBehaviourList[_currentBehaviourIndex].GetTimeToSpendOnGoal()));
            _currentBehaviourIndex++;
        }

        /// <summary>
        /// Sets the next behaviour point that the fish should move to, starts a timer based on this entrys time to spend on goal
        /// </summary>
        private void SetNextBehaviourPoint()
        {
            Vector3 currentPosition = fishImage.transform.localPosition;
            float newFishGoal = _sliderBehaviourList[_currentBehaviourIndex].GetLocationToMoveTo();
            Vector3 newGoal = new Vector3(currentPosition.x, newFishGoal, currentPosition.z);
            _speedToUse = _sliderBehaviourList[_currentBehaviourIndex].GetSpeedToUse();
            FishSetGoal(newGoal);
            FishSetSpeed(_speedToUse);
            if (_speedToUse == 0) { _speedToUse = defaultSpeed; }
            Debug.Log("GOal: " + newGoal);
            StartCoroutine(CustomBehaviourTime(_sliderBehaviourList[_currentBehaviourIndex].GetTimeToSpendOnGoal()));

            _currentBehaviourIndex++;
            if (_currentBehaviourIndex >= _sliderBehaviourList.Count) {  _currentBehaviourIndex = 0; }
        }

        /// <summary>
        /// A timer for how long until the next behaviour point should be set
        /// </summary>
        /// <param name="timeToWait">How long to wait</param>
        /// <returns>Sets the new behaviour point</returns>
        private IEnumerator CustomBehaviourTime(float timeToWait)
        {
            yield return new WaitForSeconds(timeToWait);
            SetNextBehaviourPoint();
        }

        #endregion

        #region FishMovementDefaultBehaviour

        /// <summary>
        /// Setsup the games default behaviour if no behaviour data was inputed
        /// </summary>
        private void SetupDefaultBehaviour()
        {
            suddenDeathTimer = _defaultSuddenDeathTime;
            _maxTimeBetweenGoals = 1;
            Vector3 startLocation = CreateGoalLocation();
            fishImage.transform.localPosition = startLocation;
            Vector3 newFishGoal = CreateGoalLocation();
            FishSetGoal(newFishGoal);

            _catchIncreaseValueToUse = catchIncreaseAmount;
            _catchDecreaseValueToUse = catchDecreaseAmount;

            // Scaling variables based on difficulty
            progressSlider.maxValue = _catchMax;
            _catchProgress = Mathf.Clamp(55 - 5 * _reelingObjectData.GetCatchDifficulty(), 40, 100);
        }

        /// <summary>
        /// Checks the time since last goal was set to determine if it has been long enough to set a new goal
        /// If it has been long enough this function will then create a new goal location through CreateGoalLocation()
        /// and then set it through FishSetGoal.
        /// </summary>
        private void DetermineIfNeedGoal()
        {
            if (_behaviourLoaded == true) { return; }

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
            if (Mathf.Approximately(fishImage.transform.localPosition.y, _fishMoveGoal.y))
            {
                Wander();
            }

            if (_isGoingLeft)
            {
                float speedValue = _speedToUse * _reelingObjectData.GetCatchDifficulty();

                Vector3 currentPosition = fishImage.transform.localPosition;
                Vector3 newPosition = Vector3.MoveTowards(currentPosition, _fishMoveGoal, speedValue * Time.deltaTime);
                fishImage.transform.localPosition = newPosition;
            }
            else
            {
                float speedValue = _speedToUse * _reelingObjectData.GetCatchDifficulty();

                Vector3 currentPosition = fishImage.transform.localPosition;
                Vector3 newPosition = Vector3.MoveTowards(currentPosition, _fishMoveGoal, speedValue * Time.deltaTime);
                fishImage.transform.localPosition = newPosition;
            }
        }

        /// <summary>
        /// This is run when the fish is at its goal, it causes it to wander a small bit until it gets a new goal
        /// </summary>
        private void Wander()
        {
            float wanderValue = Random.Range(-_wanderRange, _wanderRange);
            float wanderSpeed = _speedToUse / 2;

            Vector3 currentPosition = fishImage.transform.localPosition;
            Vector3 newGoal = new Vector3(currentPosition.x, currentPosition.y + wanderValue, currentPosition.z);
            FishSetGoal(newGoal);
            FishSetSpeed(wanderSpeed);
        }


        #endregion

        #region Minigame Stats

        /// <summary>
        /// Unpauses the minigame after the player has inputed right arrow
        /// </summary>
        private void UnPauseCatchboxMovement()
        {
            _isMiniGamePaused = false;
        }

        /// <summary>
        /// Modifys the progress bar for this minigame
        /// Checks if the minigame has been won
        /// </summary>
        /// <param name="valueToAdd">The value for how much to change the catch bar</param>
        private void ModifyCatchProgress(float valueToAdd)
        {
            if (_isMiniGamePaused) { valueToAdd = valueToAdd / 2; }

            if (_suddenDeath) { valueToAdd *= 4; }

            _catchProgress += (valueToAdd * Time.deltaTime);
            progressSlider.value = _catchProgress;

            if (CheckIfCatchWon()) { WinMiniGame(); }
        }

        /// <summary>
        /// Returns true if _catchProgress is greater or equal to the max progress value
        /// </summary>
        private bool CheckIfCatchWon()
        {
            if (_catchProgress >= _catchMax) { return true; }
            else { return false; }
        }

        /// <summary>
        /// Checks if the time passed is equal to the time required for sudden death, if so sudden death is set to true
        /// </summary>
        private void CheckTimePassed()
        {
            if (_timePassed >= suddenDeathTimer && _suddenDeath != true)
            {
                _suddenDeath = true;
            }
        }

        /// <summary>
        /// Sets the speed of the fish based on inputed float
        /// </summary>
        /// <param name="speedToSet">The speed to set the fish to</param>
        private void FishSetSpeed(float speedToSet)
        {
            _speedToUse = speedToSet;
        }

        /// <summary>
        /// Checks if the total time passed has passed the initialwaittimer, if so unpauses the catchbox default movement
        /// </summary>
        private void CheckInitialBufferTimer()
        {
            if (_timePassed >= initialTimeToWait)
            {
                UnPauseCatchboxMovement();
            }
        }

        /// <summary>
        /// Determines if this will use a custom behaviour or a default behaviour
        /// </summary>
        private void DetermineBehaviour()
        {
            if (_reelingObjectData.GetSliderMinigameBehaviour() == null) { SetupDefaultBehaviour(); }
            else { SetupCustomBehaviour(); }
        }

        /// <summary>
        /// Setsup default variables that are needed
        /// </summary>
        private void InitializeVariables()
        {
            fishImage.sprite = _reelingObjectData.GetTexture();
            sliderCanvas.SetActive(true);

            _clickAction.started += MouseDown;
            _clickAction.canceled += MouseUp;

            catchBox.transform.localPosition = new Vector3(catchBox.transform.localPosition.x, catchboxYStartLocation, catchBox.transform.localPosition.z);
            _timePassed = 0f;
            _timeSinceLastGoal = 0f;
            _suddenDeath = false;
            _catchBoxVelocity = 0f;
            FishSetSpeed(defaultSpeed);
            _inputHeld = false;

            _behaviourLoaded = false;

            int decideDirection = Random.Range(0, 2);
            if (decideDirection == 0) { _goingLeft = true; }
            else {  _goingLeft = false; }
        }

        #endregion
    }
}
