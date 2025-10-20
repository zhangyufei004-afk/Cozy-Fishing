using FishingGame.FishSystem;
using System;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace FishingGame.Reeling
{
    internal enum ERealisticDirection
    {
        Clockwise = 0,
        AntiClockwise,
        Stop
    };

    /// <summary>
    /// The realistic minigame simulates spinning a reel
    /// The player has to spin the reel either clockwise or anti clockwise by clicking and dragging a dragable UI image
    /// That dragable is tied to this class through the dragableScript
    /// This class controls the progress and backend data logic
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
        [Tooltip("The centerpoint of the rod in the UI used for calculating radius and showing the direction")]
        private Image centerPoint;

        [SerializeField]
        [Tooltip("The Fish iamge that moves")]
        private Image fishImage;

        [Header("GameData")]

        [SerializeField]
        [Tooltip("The default value for how much progress is lost per second spinning wrong way")]
        private float defaultDecayValue;

        [SerializeField]
        [Tooltip("The default value for how much progress is needed to win")]
        private float defaultProgressMax;

        [SerializeField]
        [Tooltip("The value for how much progress needed difficulty causes")]
        private float defaultProgressScaleValue;

        [SerializeField]
        [Tooltip("The value for how many seconds until the time scale is increased")]
        private int timeScaleMaxSeconds;

        [SerializeField]
        [Tooltip("How many points should the circle bounds have, the higher the points the greater the accurarcy but the worse the performance")]
        private int amountOfPointsInBounds;

        [SerializeField]
        [Tooltip("The radius of the bounds circle")]
        private float radius;

        [SerializeField]
        [Tooltip("temp")]
        private GameObject tempObject;

        [SerializeField]
        [Tooltip("The reset spot for the fish UI image at the start of the game")]
        private Vector3 fishResetSpot;

        private Vector2[] _boundsPoints;
        private int _previousBoundsPoint = 0;
        private int _currentBoundsPoint = 0;

        private int _fishDifficulty;

        private float _currentTimeScale;
        private float _currentScaleTimerValue;
        private float _progressValue;
        private float _progressMaxValue;
        private float _speedScalar = 20f;

        private IFishAble _fishedObject;

        private float _fishWeight = 10f;

        private ERealisticDirection _currentDirection;
        private bool _miniGameActive = false;

        private void OnEnable()
        {
            _boundsPoints = new Vector2[amountOfPointsInBounds];
            SetBounds();
        }


        private void Update()
        {
            if (_miniGameActive != true) { return; }

            if (CheckIfLost()) { EndMiniGame(false); return; }

            float speedToAdd = (dragableScript.GetSpeed() * _speedScalar) * Time.deltaTime;

            Vector3 fishCurrentPosition = fishImage.transform.localPosition;
            Vector3 newFishPosition = new Vector3(fishCurrentPosition.x, fishCurrentPosition.y + speedToAdd, fishCurrentPosition.z);
            fishImage.transform.localPosition = newFishPosition;

            if (_currentScaleTimerValue >= timeScaleMaxSeconds)
            {
                UpdateTimeScale();
            }
        }

        #region Public Functions

        /// <summary>
        /// Initializes the minigame, setting the catchdifficulty and runs the initiation functions
        /// </summary>
        /// <param name="fishScriptable">Data of fish being caught</param>
        public void InitializeMiniGame(IFishAble fishScriptable)
        {
            _fishedObject = fishScriptable;
            _fishDifficulty = _fishedObject.GetCatchDifficulty();
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

        /// <summary>
        /// Returns the closest circle point to the mouses position
        /// </summary>
        /// <param name="mousePosition">The mouse position</param>
        /// <returns>The closest vector2 point of the circle bounds</returns>
        public Vector2 GetClosestPoint(Vector2 mousePosition)
        {
            _previousBoundsPoint = _currentBoundsPoint;
            Vector2 currentClosest = _boundsPoints[0];
            float smallestDistance = 999f;
            int i = 0;

            foreach (Vector2 point in _boundsPoints)
            {
                float thisDistance = Vector2.Distance(point, mousePosition);
                if (smallestDistance > thisDistance)
                {
                    smallestDistance = thisDistance;
                    currentClosest = point;
                    _currentBoundsPoint = i;
                }
                i++;
            }

            return currentClosest;
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
            if (_currentDirection == ERealisticDirection.Clockwise)
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
        /// Checks if the progress value is equal to the max progress value, if so return true
        /// </summary>
        /// <returns>True if enough progress has been met</returns>
        private bool CheckIfWon()
        {
            if (_progressValue >= _progressMaxValue) { return true; }
            else { return false; }
        }

        /// <summary>
        /// Checks if the progress value is less than or equal to 0 if so returns true
        /// </summary>
        /// <returns>True if below 0 progress</returns>
        private bool CheckIfLost()
        {
            if (_progressValue <= 0) { return true; }
            else { return false; }
        }

        /// <summary>
        /// Increase the current timescale value by 1
        /// </summary>
        private void UpdateTimeScale()
        {
            _currentScaleTimerValue = 0;
            _currentTimeScale += 1;
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
        }

        /// <summary>
        /// Resets any variables that might change during gametime
        /// </summary>
        private void ResetGameTimeVariables()
        {
            SetBounds();
            _currentTimeScale = 1.0f;
            _progressValue = 0f;
            progressSlider.value = _progressValue;
            fishImage.transform.localPosition = fishResetSpot;
            fishImage.sprite = _fishedObject.GetTexture();
        }

        /// <summary>
        /// Applies any difficulty scalars based on fish difficulty
        /// </summary>
        private void DifficultyScalars()
        {
            _progressMaxValue = defaultProgressMax + (defaultProgressScaleValue * _fishDifficulty);
            progressSlider.maxValue = _progressMaxValue;
            _progressValue = Mathf.Clamp(20f, _progressMaxValue / (_fishDifficulty + 1), 10000f);
            progressSlider.value = _progressValue;
        }

        /// <summary>
        /// Calcualtes the bounds of the reeling UI
        /// </summary>
        private void SetBounds()
        {
            for (int i = 0; i < amountOfPointsInBounds; i++)
            {
                float pointNum = (i * 1.0f) / amountOfPointsInBounds;
                float angle = pointNum * Mathf.PI * 2;

                float floatX = Mathf.Sin(angle) * radius;
                float floatY = Mathf.Cos(angle) * radius;

                Vector3 pointPos = new Vector3(floatX, floatY) + centerPoint.transform.position;



                _boundsPoints[i] = pointPos;
                // The below line is temp for visualization sometimes when needed
                // Instantiate(tempObject, _boundsPoints[i], Quaternion.identity);
            }
        }

        #endregion
    }
}
