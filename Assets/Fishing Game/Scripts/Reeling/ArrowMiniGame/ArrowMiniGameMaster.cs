using FishingGame.FishSystem;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace FishingGame.Reeling
{
    internal enum EMovementDirection
    {
        Left,
        Right,
        Up,
        Down
    };

    public class ArrowMiniGameMaster : MonoBehaviour, IReelingMinigame
    {
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

        private Fish _fishData;
        private int _fishDifficulty;
        private int _maxAmountOfActiveArrows;
        private int _currentArrowCount;
        private float _currentProgress;
        private float _currentTimePassed;
        private float _maxTimeBeforeModify;
        private int _timeModifier;
        private float _maxProgress;
        private bool _gameActive = false;


        private List<MovingArrow> _activeArrows;
        private List<MovingArrow> _arrowsAbleToBePressed;

        [Header("Misc")]

        private InputAction _directionAction;

        

        


        private void OnEnable()
        {
            InputActionAsset inputActions = InputSystem.actions;
            InputActionMap uiActionMap = inputActions.FindActionMap("UI");
            uiActionMap.Enable();
            _directionAction = uiActionMap.FindAction("ArrowMiniGame");

            _activeArrows = new List<MovingArrow>();
        }


        void Update()
        {
            if (_gameActive != true) { return; }

            _currentTimePassed += Time.deltaTime;

            CheckTimePassed();
        }

        public void InitializeMiniGame(Fish fishScriptable)
        {
            _fishData = fishScriptable;
            _fishDifficulty = _fishData.GetFishCatchDifficulty();
            fishingCanvas.SetActive(true);

            ResetRuntimeVariables();
            SetDifficultyModifiers();

            int i = 0;
            foreach (ArrowSpawner spawner in spawnPoints)
            {
                spawner.InitiateSpawner(_fishDifficulty, spawnableArrow, arrowGoalPoints[i]);
                i++;
            }
        }

        public void BeginMiniGame()
        {
            _gameActive = true;
            

            foreach (ArrowSpawner spawner in spawnPoints)
            {
                spawner.ActivateOrDeactivateSpawner(true);

                // TEMP
                spawner.SpawnArrow();
            }



        }

        public void LoseMiniGame()
        {
            throw new System.NotImplementedException();
        }

        public void WinMiniGame()
        {
            throw new System.NotImplementedException();
        }

        private void SpawnArrow()
        {
            if (HasReachedMaxArrowSpawned()) { return; }
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
        /// Run when an arrow is pressed at the correct time
        /// This takes the inputed arrow, removes it from the active arrow list
        /// Modifys the progress by increasing it
        /// It then check if the progress is high enough, and if so wins the minigame
        /// </summary>
        /// <param name="arrowCompleted">The arrow being modified</param>
        public void ArrowSuccsessfullyPressed(MovingArrow arrowCompleted)
        {
            AddOrRemoveActiveArrow(arrowCompleted, false);
            ModifyProgress(defaultProgressModify * _timeModifier);
            if (CheckIfEnoughProgress()) { WinMiniGame(); }
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
            AddOrRemoveActiveArrow(arrowFailed, false);
            ModifyProgress(-defaultProgressModify * _timeModifier);
            if (CheckIfFailed()) { LoseMiniGame(); }
        }

        /// <summary>
        /// Checks if enough progress has been reached
        /// If progress is enough, will return true
        /// Else returns false
        /// </summary>
        /// <returns>True if progress is high enough, otherwise false</returns>
        private bool CheckIfEnoughProgress()
        {
            if (_currentProgress == _maxProgress) { return true; }
            else { return false; }
        }

        /// <summary>
        /// Checks if the progress has reached 0 meaning a fail
        /// Return true if it has, otherwise false
        /// </summary>
        /// <returns>True if failed minigame, otherwise false</returns>
        private bool CheckIfFailed()
        {
            if (_currentProgress == 0) { return true; }
            else { return false; }
        }

        /// <summary>
        /// Modifys the current progress based on the inputed value
        /// Updates the slider UI to properly reflect the progress
        /// </summary>
        /// <param name="progressValue"></param>
        private void ModifyProgress(float progressValue)
        {
            _currentProgress += progressValue;
            progressSlider.value = progressValue;
        }

        /// <summary>
        /// Checks if enough time has past since the last time the time modifier was changed
        /// If so, increases the time modified by 1
        /// </summary>
        private void CheckTimePassed()
        {
            if (_currentTimePassed >= _maxTimeBeforeModify)
            {
                _currentTimePassed = 0;
                _timeModifier += 1;
            }
        }
        

        

        #region Initilization_Functions

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

            _maxAmountOfActiveArrows = (_fishDifficulty * 2) + maxArrowVariance;
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
        }

        /// <summary>
        /// Resets all variables that change as the minigame is played
        /// </summary>
        private void ResetRuntimeVariables()
        {
            _timeModifier = 0;
            _currentProgress = 0;
            _currentArrowCount = 0;
            _activeArrows.Clear();
        }

        #endregion
    }
}
