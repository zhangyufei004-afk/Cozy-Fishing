using FishingGame.FishSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        [Tooltip("The parent object of the ui")]
        private GameObject fishingCanvas;

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
        [Tooltip("The amount of progress needed to complete the minigame")]
        private float maxProgress;

        [SerializeField]
        [Tooltip("The absolute minimum amount of progress to be removed or added regardless of difficulty")]
        private float minProgressModify;

        private Fish _fishData;
        private int _fishDifficulty;
        private int _maxAmountOfActiveArrows;
        private int _currentArrowCount;
        private float _progress;
        private float _progressIncreaseAmount;
        private float _progressDecreaseAmount;


        private List<MovingArrow> _activeArrows;

        [Header("Misc")]

        private InputAction _directionAction;

        

        


        private void OnEnable()
        {
            InputActionAsset inputActions = InputSystem.actions;
            InputActionMap uiActionMap = inputActions.FindActionMap("UI");
            uiActionMap.Enable();
            _directionAction = uiActionMap.FindAction("ArrowMiniGame");
        }


        void Update()
        {

        }

        public void InitializeMiniGame(Fish fishScriptable)
        {
            _fishData = fishScriptable;
            _fishDifficulty = _fishData.GetFishCatchDifficulty();
            fishingCanvas.SetActive(true);

            SetDifficultyModifiers();



        }

        public void BeginMiniGame()
        {
            throw new System.NotImplementedException();
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
        
        private void ArrowSuccsessfullyPressed(MovingArrow arrowCompleted)
        {
            AddOrRemoveActiveArrow(arrowCompleted, false);

        }

        /// <summary>
        /// Checks if enough progress has been reached
        /// If progress is enough, will return true
        /// Else returns false
        /// </summary>
        /// <returns>True if progress is high enough, otherwise false</returns>
        private bool CheckIfEnoughProgress()
        {
            if (_progress == maxProgress) { return true; }
            else { return false; }
        }

        private void ModifyProgress(float progressValue)
        {

        }

        

        

        #region Initilization_Functions

        /// <summary>
        /// Runs the functions that set variables based on the current difficulty
        /// </summary>
        private void SetDifficultyModifiers()
        {
            SetMaxAmountOfActiveArrows();
            SetProgressModifiers();
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

        private void SetProgressModifiers()
        {

        }

        #endregion
    }
}
