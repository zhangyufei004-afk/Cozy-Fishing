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

        private Vector2[] _boundsPoints;

        private int _fishDifficulty;

        private GameObject _currentFish3D;
        private float _currentTimeScale;
        private float _currentScaleTimerValue;
        private float _progressValue;
        private float _progressMaxValue;
        private float _timeSinceLastDirectionChange;
        private float _directionRollTimerMax = 8f;


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

            // Triples the addition to timer if stop is current direction
            if (_currentDirection == ERealisticDirection.Stop) { _timeSinceLastDirectionChange += (Time.deltaTime * 3) * _currentTimeScale; }
            else { _timeSinceLastDirectionChange += Time.deltaTime * _currentTimeScale; }
            

            if (_timeSinceLastDirectionChange >= _directionRollTimerMax)
            {
                DecideDirection();
            }

            if (_currentScaleTimerValue >= timeScaleMaxSeconds)
            {
                UpdateTimeScale();
            }

            if (CheckIfDragableInRightDirection())
            {
                textDirectionHolder.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

                if (_currentDirection == ERealisticDirection.Stop)
                {
                    AddToProgressSlider(((defaultDecayValue) * Time.deltaTime) * _currentTimeScale);
                    return;
                }
                float speed = (dragableScript.GetSpeed() * Time.deltaTime) * _currentTimeScale;
                AddToProgressSlider(speed);
            }
            else
            {
                textDirectionHolder.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
                RemoveFromProgressSlider(defaultDecayValue * Time.deltaTime);
            }
        }

        #region Public Functions

        /// <summary>
        /// Initializes the minigame, setting the catchdifficulty and runs the initiation functions
        /// </summary>
        /// <param name="fishScriptable">Data of fish being caught</param>
        public void InitializeMiniGame(IFishAble fishScriptable)
        {
            _fishDifficulty = fishScriptable.GetCatchDifficulty();
            realisticCanvas.SetActive(true);
            _currentFish3D = reelingMaster.GetCurrent3DFishObject();

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
        /// Sets the initial direction to either clockwise or anti clockwise
        /// Does not have stop as an option
        /// </summary>
        private void SetInitialDirection()
        {
            int rolledNumber = UnityEngine.Random.Range(0, 2);
            _currentDirection = (ERealisticDirection)rolledNumber;
            SetTextAndAnimationForDirection();
        }

        /// <summary>
        /// Decides what direction player must spin in by rolling a random value between 0 and enum value count
        /// </summary>
        private void DecideDirection()
        {
            Array enumValues = Enum.GetValues(typeof(ERealisticDirection));
            int directionSize = enumValues.Length;

            int rolledNumber = UnityEngine.Random.Range(0, directionSize);
            _currentDirection = (ERealisticDirection)rolledNumber;

            _timeSinceLastDirectionChange = 0f;
            SetTextAndAnimationForDirection();
        }

        /// <summary>
        /// Sets the text and animation that tells the player what direction to spin in
        /// Paremeter bool is used to decide what text to set
        /// true = Clockwise, False = anti-clockwise
        /// </summary>
        /// <param name="isClockwise">True = clockwise, false = anti-clockwise</param>
        private void SetTextAndAnimationForDirection()
        {
            switch (_currentDirection)
            {
                case ERealisticDirection.Clockwise:
                    textDirectionHolder.GetComponentInChildren<TextMeshProUGUI>().text = "Go clockwise!";
                    break;
                case ERealisticDirection.AntiClockwise:
                    textDirectionHolder.GetComponentInChildren<TextMeshProUGUI>().text = "Go anti-clockwise!";
                    break;
                case ERealisticDirection.Stop:
                    textDirectionHolder.GetComponentInChildren<TextMeshProUGUI>().text = "Stop spinning!";
                    break;
                default:
                    textDirectionHolder.GetComponentInChildren<TextMeshProUGUI>().text = "Go clockwise!";
                    throw new InvalidOperationException("Waring: ECurrentDirection Enum was not set to an aproipreate value, has defaulted to clockwise! This happened to object: " + gameObject.name);
            }
        }

        /// <summary>
        /// Returns true if the dragable is currently being dragged in the right direction
        /// </summary>
        /// <returns>True if dragged in right direction otherwise false</returns>
        private bool CheckIfDragableInRightDirection()
        {
            ERealisticDirection dragableCurrentDirection = (ERealisticDirection)dragableScript.GetCurrentDirectionAsInt();

            if (dragableCurrentDirection == _currentDirection)
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
            SetInitialDirection();
        }

        /// <summary>
        /// Resets any variables that might change during gametime
        /// </summary>
        private void ResetGameTimeVariables()
        {
            SetBounds();
            _currentTimeScale = 1.0f;
            _progressValue = 0f;
            _timeSinceLastDirectionChange = 0f;
            progressSlider.value = _progressValue;
        }

        /// <summary>
        /// Applies any difficulty scalars based on fish difficulty
        /// </summary>
        private void DifficultyScalars()
        {
            _progressMaxValue = defaultProgressMax + (defaultProgressScaleValue * _fishDifficulty);
            progressSlider.maxValue = _progressMaxValue;
            _progressValue = Mathf.Clamp(20f, _progressMaxValue / _fishDifficulty, 10000f);
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
