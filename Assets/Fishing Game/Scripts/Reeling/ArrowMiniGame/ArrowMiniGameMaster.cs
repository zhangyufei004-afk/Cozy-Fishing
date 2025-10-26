using FishingGame.FishSystem;
using FishingGame.Input;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace FishingGame.Reeling
{
    /// <summary>
    /// The type of direction an arrow will represent
    /// </summary>
     internal enum EMovementDirection
    {
        Left = 0,
        Right = 1,
        Up = 2,
        Down = 3
    };
    
    /// <summary>
    /// This minigame involves arrows falling down the screen
    /// Once an arrow reaches a specific point on the screen the player needs to press the corresponding arrow key
    /// Failing this or pressing a key too early results in progress loss
    /// Doing so correctly results in progress gain
    /// This is the master script
    /// </summary>
    public class ArrowMiniGameMaster : MonoBehaviour, IReelingMinigame
    {
        [Header("Major Script References")]

        [SerializeField]
        [Tooltip("Reference to the ReelingMaster script.")]
        private ReelingMaster reelingMaster;

        [Header("UI elements")]

        [SerializeField]
        [Tooltip("All UI spawnpoints for arrows")]
        private List<ArrowSpawner> spawnPoints;

        [SerializeField]
        [Tooltip("The UI elements that arrows must reach, the order of this list should match the order of spawnpoints, for example spawnPoins[0] should be the spawnpoint directly above arrowGoalPoints[0]")]
        private List<ArrowGoalPoints> arrowGoalPoints;

        [SerializeField]
        [Tooltip("The parent object of the ui")]
        private GameObject fishingCanvas;

        [SerializeField]
        [Tooltip("The slider that visually represents the total progress")]
        private Slider progressSlider;

        [Header("Minigame Data")]

        [SerializeField]
        [Tooltip("The arrow prefab")]
        private GameObject spawnableArrow;

        [SerializeField]
        [Tooltip("Default slow speed value for arrows")]
        private int slowSpeed;

        [SerializeField]
        [Tooltip("Default medium speed value for arrows")]
        private int mediumSpeed;

        [SerializeField]
        [Tooltip("Default fast speed value for arrows")]
        private int fastSpeed;

        [SerializeField]
        [Tooltip("The minimum amount of time needed inbetween arrow spawns, regardless of difficulty. Stops arrows overlapping exactly even at highest difficulty levels")]
        private float minTimeBetweenArrows;

        [SerializeField]
        [Tooltip("The maximum amount of time needed inbetween arrow spawns, regardless of difficulty.")]
        private float maxTimeBetweenArrows;

        [SerializeField]
        [Tooltip("The max amount of variance in the total amount of arrows being spawned")]
        private int maxArrowVariance;

        [SerializeField]
        [Tooltip("The absolute minimum amount of progress to be removed or added regardless of difficulty")]
        private float minProgressModify;

        [SerializeField]
        [Tooltip("The default amount of progress to add or takeaway, this is modified as time in the minigame goes on")]
        private float defaultProgressModify;

        [SerializeField]
        [Tooltip("The default amount of progress needed to complete minigame, this is scaled based on difficulty")]
        private float defaultProgressMax;

        [SerializeField]
        [Tooltip("The amount of time it takes for the time scaler to increase")]
        private float maxTimeBeforeModify;

        [SerializeField]
        [Tooltip("Rate of spawn during a wave")]
        private float waveSpawnTime;

        [SerializeField]
        private AudioSource rightSound;
        [SerializeField]
        private AudioClip[] rightSounds;

        [SerializeField]
        private AudioSource wrongSound;
        [SerializeField]
        private AudioClip[] wrongSounds;

        private IFishAble _currentlyReelingObject;
        private int _fishDifficulty;
        private int _maxAmountOfActiveArrows;
        private int _currentArrowCount;
        private float _currentProgress;
        private float _currentTimePassed;
        private int _timeModifier;
        private float _maxProgress;
        private int _waveArrowCount = 0;
        private bool _gameActive = false;
        private bool _waveActive = false;
        private int _maxRangeWaveChance = 10;
        private int _currentWaveChance = 0;

        private List<MovingArrow> _arrowsAvailableToBePressed;
        private List<MovingArrow> _activeArrows;
        private List<MovingArrow> _arrowsToRemove;
        private InputAction _directionAction;
        private EMovementDirection _arrowType;
        private InputActionMap _uiActionMap;

        private InputAction _upAction;
        private InputAction _downAction;
        private InputAction _leftAction;
        private InputAction _rightAction;

        private int _rightSoundIndex = 0;
        private int _wrongSoundIndex = 0;


        private void OnEnable()
        {
            _activeArrows = new List<MovingArrow>();
            _arrowsAvailableToBePressed = new List<MovingArrow>();
            _arrowsToRemove = new List<MovingArrow>();

            InputActionAsset inputAction = InputSystem.actions;
            _uiActionMap = inputAction.FindActionMap("ArrowMiniGame");
        }

        private void OnDisable()
        {
            DisableArrowKeys();
        }

        private void Update()
        {
            if (_gameActive != true) { return; }

            _currentTimePassed += Time.deltaTime;

            CheckTimePassed();



            if (_activeArrows.Count == 0)
            {
                EmergencySpawnArrow();
            }
        }

        #region Public Functions

        /// <summary>
        /// Setsup all the required logic for the minigame
        /// </summary>
        /// <param name="fishScriptable">The data of the fish being caught</param>
        public void InitializeMiniGame(IFishAble fishScriptable)
        {
            _currentlyReelingObject = fishScriptable;
            _fishDifficulty = _currentlyReelingObject.GetCatchDifficulty();
            fishingCanvas.SetActive(true);
            SetUpArrowKeys();

            ResetRuntimeVariables();
            SetDifficultyModifiers();

            int i = 0;
            foreach (ArrowSpawner spawner in spawnPoints)
            {
                spawner.InitiateSpawner(_fishDifficulty, spawnableArrow, arrowGoalPoints[i]);
                i++;
            }
        }

        /// <summary>
        /// Activates the spawners and begins the minigame
        /// </summary>
        public void BeginMiniGame()
        {
            _gameActive = true;

            foreach (ArrowSpawner spawner in spawnPoints)
            {
                spawner.ActivateOrDeactivateSpawner(true);
            }

            SpawnArrow();
        }

        /// <summary>
        /// Loses the minigame
        /// </summary>
        public void LoseMiniGame()
        {
            DeactivateMiniGame(false);
        }

        /// <summary>
        /// Wins the minigame
        /// </summary>
        public void WinMiniGame()
        {
            DeactivateMiniGame(true);
        }

        /// <summary>
        /// Adds or removes an arrow from the active arrow list
        /// First parameter is the arrowtomodify the second paremeter should be true if adding
        /// false if removing from list.
        /// Will modifiy the currentarrowcount as well
        /// </summary>
        /// <param name="arrowModified">The arrow being modified</param>
        /// <param name="addToList">True if adding to list, false if removing from list</param>
        public void AddOrRemoveActiveArrow(MovingArrow arrowModified, bool addToList)
        {
            if (addToList)
            {
                _activeArrows.Add(arrowModified);
                _currentArrowCount++;
            }
            else
            {
                _activeArrows.Remove(arrowModified);
                _currentArrowCount--;
            }
        }

        /// <summary>
        /// Adds arrow to  the available to be pressed list
        /// </summary>
        /// <param name="arrowToAdd">Arrow to Add</param>
        public void AddArrowToPressList(MovingArrow arrowToAdd)
        {
            _arrowsAvailableToBePressed.Add(arrowToAdd);
        }

        /// <summary>
        /// Removes inputed arrow from the available to be pressed list
        /// </summary>
        /// <param name="arrowToRemove">Arrow to remove</param>
        public void RemoveArrowFromPressList(MovingArrow arrowToRemove)
        {
            _arrowsAvailableToBePressed.Remove(arrowToRemove);
        }

        /// <summary>
        /// Checks if the available to be pressed list contains inputed arrow
        /// If so returns true otherwise false
        /// </summary>
        /// <param name="arrowToCheck">The arrow being checked</param>
        /// <returns>True if the list does contain arrow, otherwise false</returns>
        public bool DoesThisContainArrow(MovingArrow arrowToCheck)
        {
            if (_arrowsAvailableToBePressed.Contains(arrowToCheck) == true) { return true; }
            else { return false; }
        }

        /// <summary>
        /// Run when an arrow has passed the fail point
        /// This will Remove the arrow from active arrow list
        /// Subtract progress
        /// And will then check if progress is low enough for a fail
        /// </summary>
        /// <param name="arrowFailed"></param>
        public void ArrowFailedToBePressed(MovingArrow arrowFailed)
        {
            if (_wrongSoundIndex == wrongSounds.Length)
                _wrongSoundIndex = 0;
            wrongSound.clip = wrongSounds[_wrongSoundIndex];
            wrongSound.Play();
            _wrongSoundIndex++;

            AddOrRemoveActiveArrow(arrowFailed, false);
            ModifyProgress(-defaultProgressModify * _timeModifier);
            if (CheckIfFailed()) { LoseMiniGame(); }
        }

        #endregion


        #region CoreGameTimeFunctions

        private void InputLogic(EMovementDirection inputedDirection)
        {
            if (CheckIfThereAreArrowsPressable())
            {
                foreach (MovingArrow arrow in _arrowsAvailableToBePressed)
                {
                    int directionAsInt = arrow.GetDirectionEnumAsInt();
                    _arrowType = (EMovementDirection)directionAsInt;

                    if (inputedDirection == _arrowType)
                    {
                        _arrowsToRemove.Add(arrow);
                    }
                }
                foreach (MovingArrow arrow in _arrowsToRemove)
                {
                    ArrowSuccsessfullyPressed(arrow);
                    RemoveArrowFromPressList(arrow);
                    arrow.DeactivateArrow();
                    arrow.StartFadeAway(true);
                }
                _arrowsToRemove.Clear();
            }
            else
            // In the event a key was pressed with no arrows pressable this section is run
            {
                ArrowSpawner spawnerToPunish = null;
                foreach (ArrowSpawner spawner in spawnPoints)
                {
                    int directionAsInt = spawner.GetSpawnerTypeAsInt();
                    _arrowType = (EMovementDirection)directionAsInt;

                    if (inputedDirection == _arrowType)
                    {
                        spawnerToPunish = spawner;
                        break;
                    }
                }

                spawnerToPunish.PunishPoorPress();
            }

        }

        #region IndividualActionFunctions
        
        /// <summary>
        /// Logic for what to do when up is pressed
        /// </summary>
        /// <param name="inputAction">Context of action</param>
        private void UpPressed(InputAction.CallbackContext inputAction)
        {
            InputLogic(EMovementDirection.Up);
        }

        /// <summary>
        /// Logic for what to do when down is pressed
        /// </summary>
        /// <param name="inputAction">Context of action</param>
        private void DownPressed(InputAction.CallbackContext inputAction)
        {
            InputLogic(EMovementDirection.Down);
        }

        /// <summary>
        /// Logic for what to do when left is pressed
        /// </summary>
        /// <param name="inputAction">Context of action</param>
        private void LeftPressed(InputAction.CallbackContext inputAction)
        {
            InputLogic(EMovementDirection.Left);
        }

        /// <summary>
        /// Logic for what to do when right is pressed
        /// </summary>
        /// <param name="inputAction">Context of action</param>
        private void RightPressed(InputAction.CallbackContext inputAction)
        {
            InputLogic(EMovementDirection.Right);
        }

        #endregion

        /// <summary>
        /// Run when an arrow is pressed at the correct time
        /// This takes the inputed arrow, removes it from the active arrow list
        /// Modifys the progress by increasing it
        /// It then check if the progress is high enough, and if so wins the minigame
        /// </summary>
        /// <param name="arrowCompleted">The arrow being modified</param>
        private void ArrowSuccsessfullyPressed(MovingArrow arrowCompleted)
        {
            if (_rightSoundIndex == rightSounds.Length)
                _rightSoundIndex = 0;
            rightSound.clip = rightSounds[_rightSoundIndex];
            rightSound.Play();
            _rightSoundIndex++;

            AddOrRemoveActiveArrow(arrowCompleted, false);
            ModifyProgress(defaultProgressModify * _timeModifier);
            if (CheckIfEnoughProgress()) { WinMiniGame(); }
        }

        /// <summary>
        /// Spawns an Arrow as long as the current amount of arrows active is not greater to the maximum amount of arrows
        /// Starts a timer for when this will next be run
        /// Chooses a random spawner each time to spawn the arrow
        /// </summary>
        private void SpawnArrow()
        {
            if (HasReachedMaxArrowSpawned()) { return; }

            int chosenSpawner = Random.Range(0, spawnPoints.Count);

            spawnPoints[chosenSpawner].SpawnArrow(fastSpeed);

            if (_waveActive)
            {
                _waveArrowCount++;
                if (CheckIfWaveCountReached())
                {
                    _waveActive = false;
                    StartCoroutine(SpawnCoolDown(DetermineTimeUntilNextSpawn()));
                }
                else
                {
                    StartCoroutine(SpawnCoolDown(waveSpawnTime));
                }
            }
            else
            {
                StartCoroutine(SpawnCoolDown(DetermineTimeUntilNextSpawn()));
                DecideIfStartWave();
            }
        }

        /// <summary>
        /// Forces spawns a new arrow, should be run if there is no active arrows
        /// Unlike normal arrow spawning this does not start a cooldown timer
        /// </summary>
        private void EmergencySpawnArrow()
        {
            int chosenSpawner = Random.Range(0, spawnPoints.Count);

            spawnPoints[chosenSpawner].SpawnArrow(fastSpeed);
        }

        /// <summary>
        /// Modifys the current progress based on the inputed value
        /// Updates the slider UI to properly reflect the progress
        /// </summary>
        /// <param name="progressValue"></param>
        private void ModifyProgress(float progressValue)
        {
            _currentProgress += progressValue;
            progressSlider.value = _currentProgress;
        }

        /// <summary>
        /// Rolls a random number between the max wave chance and 0
        /// If that number is less than or equal to current wave chance
        /// A wave is begun
        /// Everytime this roll fails currentwavechance increases by 1
        /// </summary>
        private void DecideIfStartWave()
        {
            int rolledNumber = Random.Range(0, _maxRangeWaveChance);

            if (rolledNumber <= _currentWaveChance)
            {
                _waveActive = true;
            }
            else { _currentWaveChance++; }
        }

        /// <summary>
        /// Deactivates the minigame
        /// Has a bool parameter that is used for changing logic based on if the minigame was won or lost
        /// </summary>
        /// <param name="didWin">Was the minigame won or lost, true if won, otherwise false</param>
        private void DeactivateMiniGame(bool didWin)
        {
            _gameActive = false;
            DisableArrowKeys();

            foreach (MovingArrow arrow in _activeArrows)
            {
                arrow.StartFadeAway(didWin);
            }

            foreach (ArrowSpawner spawner in spawnPoints)
            {
                spawner.ActivateOrDeactivateSpawner(false);
            }

            StartCoroutine(UIDissapear(didWin));
        }

        #endregion

        #region ChecksAndGets

        /// <summary>
        /// Checks if the maximum arrow count has been reached
        /// Returns true if it has otherwise false
        /// </summary>
        /// <returns>True if max arrow count has been reached, otherwise false</returns>
        private bool HasReachedMaxArrowSpawned()
        {
            if (_currentArrowCount == _maxAmountOfActiveArrows) { return true; }
            else { return false; }
        }

        /// <summary>
        /// Checks if enough progress has been reached
        /// If progress is enough, will return true
        /// Else returns false
        /// </summary>
        /// <returns>True if progress is high enough, otherwise false</returns>
        private bool CheckIfEnoughProgress()
        {
            if (_currentProgress >= _maxProgress) { return true; }
            else { return false; }
        }

        /// <summary>
        /// Checks if the progress has reached 0 meaning a fail
        /// Return true if it has, otherwise false
        /// </summary>
        /// <returns>True if failed minigame, otherwise false</returns>
        private bool CheckIfFailed()
        {
            if (_currentProgress <= 0) { return true; }
            else { return false; }
        }

        /// <summary>
        /// Checks if enough time has past since the last time the time modifier was changed
        /// If so, increases the time modified by 1
        /// </summary>
        private void CheckTimePassed()
        {
            if (_currentTimePassed >= maxTimeBeforeModify)
            {
                _currentTimePassed = 0;
                _timeModifier += 1;
            }
        }

        /// <summary>
        /// Returns a float value time between the minimum and maximum allowed time between arrows
        /// </summary>
        /// <returns>A time value between min and max time between values</returns>
        private float DetermineTimeUntilNextSpawn()
        {
            float time = Random.Range(minTimeBetweenArrows, maxTimeBetweenArrows);
            return time;
        }

        /// <summary>
        /// Checks if the current amount of spawned arrows in a wave is equal to or greater than the fish difficulty
        /// If they are  it returns true otherwise false
        /// </summary>
        /// <returns>True if enough arrows have spawned otherwise false</returns>
        private bool CheckIfWaveCountReached()
        {
            if (_waveArrowCount >= _fishDifficulty) { return true; }
            else { return false; }
        }

        /// <summary>
        /// Checks if there are arrows that are flagged as pressable
        /// Returns true if so otherwise false
        /// </summary>
        /// <returns>True if there is atleast 1 pressable arrow otherwise false</returns>
        private bool CheckIfThereAreArrowsPressable()
        {
            if (_arrowsAvailableToBePressed.Count != 0) { return true; }
            else { return false; }
        }

        #endregion

        #region Timers

        /// <summary>
        /// A timer that waits for the inputed amount of seconds
        /// Once it completes it reruns spawnarrow
        /// </summary>
        /// <param name="timeToWait">Seconds to wait</param>
        /// <returns>Runs SpawnArrow() after inputed seconds</returns>
        private IEnumerator SpawnCoolDown(float timeToWait)
        {
            yield return new WaitForSeconds(timeToWait);
            SpawnArrow();
        }

        /// <summary>
        /// This timer controls the UIDissapearing
        /// Waits for 1 seconds then clears all UI and fully ends the minigame
        /// Takes a bool paremeter that should be set to true if the minigame was won
        /// otherwise false
        /// </summary>
        /// <param name="didWin">Was the minigame won or not</param>
        /// <returns></returns>
        private IEnumerator UIDissapear(bool didWin)
        {
            yield return new WaitForSeconds(1f);
            foreach (MovingArrow arrow in _activeArrows)
            {
                Destroy(arrow.gameObject);
            }
            _activeArrows.Clear();

            fishingCanvas.SetActive(false);
            reelingMaster.EndCurrentMiniGame(didWin);
            StopAllCoroutines();
        }

        #endregion

        #region Initilization_Functions

        /// <summary>
        /// Subscribes arrow key actions to their relevant functions
        /// </summary>
        private void SetUpArrowKeys()
        {
            _upAction = _uiActionMap.FindAction("Up");
            _upAction.performed += UpPressed;

            _downAction = _uiActionMap.FindAction("Down");
            _downAction.performed += DownPressed;

            _rightAction = _uiActionMap.FindAction("Right");
            _rightAction.performed += RightPressed;

            _leftAction = _uiActionMap.FindAction("Left");
            _leftAction.performed += LeftPressed;
        }

        /// <summary>
        /// Unsubscribes Arrowkey actions from their relevant function
        /// </summary>
        private void DisableArrowKeys()
        {
            _upAction.performed -= UpPressed;

            _downAction.performed -= DownPressed;

            _rightAction.performed -= RightPressed;

            _leftAction.performed -= LeftPressed;
        }

        /// <summary>
        /// Runs the functions that set variables based on the current difficulty
        /// </summary>
        private void SetDifficultyModifiers()
        {
            SetMaxAmountOfActiveArrows();
            SetMaxAmountOfProgress();
        }

        /// <summary>
        /// Sets the amount of arrows that can be active at once
        /// The max amount of arrows is determined by the difficulty multiplied by 2
        /// A random integer is decided between the fishdifficulty and the maxarrowvariance + the fishing difficulty
        /// That integer is then added to the max amount of arrows to spawn
        /// </summary>
        private void SetMaxAmountOfActiveArrows()
        {
            int arrowSpawnRange = Random.Range(_fishDifficulty, maxArrowVariance + _fishDifficulty);

            _maxAmountOfActiveArrows = (_fishDifficulty * 2) + arrowSpawnRange;
        }

        /// <summary>
        /// Sets the max amount of progress needed to complete the minigame
        /// This is set through multiplying the defaulmaxprogress with the fishes difficulty
        /// Also sets the sliders max progress value
        /// </summary>
        private void SetMaxAmountOfProgress()
        {
            _maxProgress = defaultProgressMax * _fishDifficulty;
            progressSlider.maxValue = _maxProgress;
            progressSlider.value = _currentProgress;
        }

        /// <summary>
        /// Resets all variables that change as the minigame is played
        /// </summary>
        private void ResetRuntimeVariables()
        {
            _timeModifier = 1;
            _currentArrowCount = 0;
            _currentProgress = defaultProgressModify * 4;
            _activeArrows.Clear();
            _waveActive = false;
            _waveArrowCount = 0;
            _currentWaveChance = 0;
            _arrowsToRemove.Clear();
            _arrowsAvailableToBePressed.Clear();
        }

        #endregion
    }
}
