using FishingGame.FishSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FishingGame.Reeling
{
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

        private int _fishDifficulty;
        private float _progressValue;
        private float _progressMaxValue;

        private bool _goClockWise = false;
        private bool _miniGameActive = false;

        private void OnEnable()
        {

        }

        private void Update()
        {
            if (_miniGameActive != true) { return; }

            if (CheckIfDragableInRightDirection() && dragableScript.GetIsMovingValue() != false)
            {
                float speed = dragableScript.GetSpeed() * Time.deltaTime;
                AddToProgressSlider(speed);
            }
            else
            {
                RemoveFromProgressSlider(defaultDecayValue * Time.deltaTime);
            }
        }

        public void InitializeMiniGame(Fish fishScriptable)
        {
            _fishDifficulty = fishScriptable.GetFishCatchDifficulty();
            realisticCanvas.SetActive(true);

            InitializeRunTimeData();
        }

        public void BeginMiniGame()
        {
            _miniGameActive = true;
        }

        public void LoseMiniGame()
        {
            EndMiniGame(false);
        }

        public void WinMiniGame()
        {
            EndMiniGame(true);
        }

        public Image GetCentreImage()
        {
            return centerPoint;
        }

        private void AddToProgressSlider(float progressToAdd)
        {
            if (_goClockWise)
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

        private void EndMiniGame(bool didWin)
        {
            _miniGameActive = false;
            realisticCanvas.SetActive(false);
            reelingMaster.EndCurrentMiniGame(didWin);
        }

        private void RemoveFromProgressSlider(float progressToRemove)
        {
            progressSlider.value -= progressToRemove;
            _progressValue -= progressToRemove;
        }

        private void SetTextForDirection(bool isClockwise)
        {
            if (isClockwise)
            {
                textDirectionHolder.GetComponentInChildren<TextMeshProUGUI>().text = "Go clockwise!";
            }
            else
            {
                textDirectionHolder.GetComponentInChildren<TextMeshProUGUI>().text = "Go anti-clockwise!";
            }
        }

        private bool CheckIfDragableInRightDirection()
        {
            bool goingClockwise = dragableScript.GetCurrentDirection();

            if (goingClockwise == _goClockWise)
            {
                return true;
            }
            return false;
        }

        private bool CheckIfWon()
        {
            if (_progressValue >= _progressMaxValue) { return true; }
            else { return false; }
        }

        private void DecideDirection()
        {
            int rolledNumber = Random.Range(0, 2);
            Debug.Log(rolledNumber);
            if (rolledNumber == 0)
            {
                _goClockWise = true;
            }
            else { _goClockWise = false; }
        }

        private void InitializeRunTimeData()
        {
            ResetGameTimeVariables();
            DifficultyScalars();
            DecideDirection();
            SetTextForDirection(_goClockWise);
        }

        private void ResetGameTimeVariables()
        {
            _progressValue = 0f;
            progressSlider.value = _progressValue;
        }

        private void DifficultyScalars()
        {
            _progressMaxValue = 100;
            progressSlider.maxValue = _progressMaxValue;
        }
    }
}
