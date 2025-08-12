using Brayden;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

namespace PrototypeFishingMechanics
{
    public class SliderMiniGame : MonoBehaviour, IReelingMinigame
    {
        #region Public Variables

        #endregion

        #region Private Fields


        [SerializeField]
        [Tooltip("Reference to the ReelingMaster script.")]
        private ReelingMaster ReelingMaster;

        [SerializeField]
        private GameObject SliderCanvas;

        private bool _isMinigameActive = false;

        private float _timerValue = 0f;

        private float _maxTime = 0f;

        private float _catchProgress = 50f;

        private int _catchMax = 100;

        [SerializeField]
        private int CatchIncreaseAmount;

        [SerializeField]
        private int CatchDecreaseAmount;

        [SerializeField]
        private UnityEngine.UI.Slider ProgressSlider;

        [SerializeField]
        private CatchBox UiCatchBoxScript;

        [SerializeField]
        private RectTransform CatchBox;

        [SerializeField]
        private RectTransform FishImage;

        [SerializeField]
        private int FishMoveMin;

        [SerializeField]
        private int FishMoveMax;

        [SerializeField]
        private int RotationMin;

        [SerializeField]
        private int RotationMax;

        [SerializeField]
        private float FishMaxXCord;

        [SerializeField]
        private float FishMinXCord;

        [SerializeField]
        private float CatchBoxMaxXCord;

        [SerializeField]
        private float CatchBoxMinXCord;


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

            if (UiCatchBoxScript.CheckUIOverlap(FishImage, CatchBox))
            {
                ModifyCatchProgress(CatchIncreaseAmount);
            }
            else
            {
                ModifyCatchProgress(CatchDecreaseAmount);
            }
        }


        /// <summary>
        /// Setups up any variable or field needed for the minigame to run
        /// </summary>
        public void InitializeMiniGame(int fishCatchDifficulty) 
        {
            SliderCanvas.SetActive(true);

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
        /// </summary>
        /// <param name="moveValue">The value for how far to move</param>
        public void MoveCatchIndicator(float moveValue)
        {
            Vector3 currentPosition = CatchBox.transform.localPosition;
            float yPosition = currentPosition.y += moveValue;
            Mathf.Clamp(yPosition, CatchBoxMinXCord, CatchBoxMaxXCord);
            Vector3 newPosition = new Vector3(currentPosition.x, yPosition, currentPosition.z);

            CatchBox.localPosition = newPosition;
        }

        /// <summary>
        /// Randomly moves the fish ui element
        /// </summary>
        public void MoveFish()
        {
            int randomValue = UnityEngine.Random.Range(FishMoveMin, FishMoveMax);

            Vector3 currentPosition = FishImage.transform.localPosition;
            float yPosition = Mathf.Clamp(currentPosition.y += randomValue * Time.deltaTime, FishMoveMin, FishMoveMax);
            Vector3 newPosition = new Vector3(currentPosition.x, yPosition, currentPosition.z);

            int rotationRandomValue = UnityEngine.Random.Range(RotationMin, RotationMax);
            FishImage.transform.rotation *= Quaternion.Euler(0, 0, rotationRandomValue * Time.deltaTime);

            FishImage.transform.localPosition = newPosition;
        }

        /// <summary>
        /// Modifys the progress bar for this minigame
        /// Checks if the minigame has been won
        /// </summary>
        /// <param name="valueToAdd">The value for how much to change the catch bar</param>
        private void ModifyCatchProgress(float valueToAdd)
        {
            _catchProgress += valueToAdd * Time.deltaTime;
            ProgressSlider.value = _catchProgress;

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



        // TEMP TESTING COROTINE WHILE THE MINIGAME DOSENT WORK ITSELF

        IEnumerator TempTimeForWin()
        {
            yield return new WaitForSeconds(4);
            WinMiniGame();
        }

        /// <summary>
        /// Sets ui elements to not be active
        /// Tells the Reelingmaster minigame has been won
        /// </summary>
        public void WinMiniGame()
        {
            _isMinigameActive = false;
            SliderCanvas.SetActive(false);
            ReelingMaster.EndCurrentMiniGame(true);
        }

        /// <summary>
        /// Sets ui elements to not be active
        /// Tells the Reelingmaster minigame has been lost
        /// </summary>
        public void LoseMiniGame()
        {
            _isMinigameActive = false;
            SliderCanvas.SetActive(false);
            ReelingMaster.EndCurrentMiniGame(false);
        }
    }
}
