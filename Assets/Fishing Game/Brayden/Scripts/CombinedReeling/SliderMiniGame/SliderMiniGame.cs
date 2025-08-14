using Brayden;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

namespace PrototypeFishingMechanics
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

        [SerializeField]
        private int catchIncreaseAmount;

        [SerializeField]
        private int catchDecreaseAmount;

        [SerializeField]
        private UnityEngine.UI.Slider progressSlider;

        [SerializeField]
        private CatchBox uiCatchBoxScript;

        [SerializeField]
        private RectTransform catchBox;

        [SerializeField]
        private RectTransform fishImage;

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

            // Check if timer complete
            if (_timerValue >= _maxTime)
            {
                LoseMiniGame();
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                MoveCatchIndicator(10 * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                MoveCatchIndicator(-10 * Time.deltaTime);
            }

            MoveFish();

            if (uiCatchBoxScript.CheckUIOverlap(fishImage, catchBox))
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
        /// </summary>
        public void InitializeMiniGame(int fishCatchDifficulty) 
        {
            sliderCanvas.SetActive(true);

            _timerValue = 0f;
            _catchProgress = 50;

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
            Mathf.Clamp(yPosition, catchBoxMinXCord, catchBoxMaxXCord);
            Vector3 newPosition = new Vector3(currentPosition.x, yPosition, currentPosition.z);

            catchBox.localPosition = newPosition;
        }

        /// <summary>
        /// Randomly moves the fish ui element
        /// Limits the y position based on the Min and Max fishMove values
        /// </summary>
        public void MoveFish()
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
