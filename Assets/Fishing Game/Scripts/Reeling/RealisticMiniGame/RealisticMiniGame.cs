using FishingGame;
using FishingGame.FishSystem;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FishingGame.Reeling
{
    /// <summary>
    /// Realistic minigame uses the IReelingMiniGame interface
    /// The realistic minigame involves the player keeping their mouse ontop of a spinning circle
    /// Player has a set time limit to complete the minigame and gains progress whenever the mouse is ontop of said circle
    /// </summary>
    public class RealisticMiniGame : MonoBehaviour, IReelingMinigame
    {
        #region Private Fields

        [SerializeField]
        [Tooltip("Reference to the ReelingMaster script.")]
        private ReelingMaster reelingMaster;

        [SerializeField]
        [Tooltip("The gameobject that holds the UI in it. This is a child of reelingUI.")]
        private GameObject realisticCanvas;

        [SerializeField]
        [Tooltip("The circle that represents where the players mouse is.")]
        private Image playerCircle;

        [SerializeField]
        [Tooltip("The circle that represents where the player needs to have their mouse over. This has a parent called MouseGoalHolder")]
        private Image goalCircle;

        [SerializeField]
        [Tooltip("The UI slider that shows the progress of the minigame.")]
        private Slider progressSlider;

        [SerializeField]
        [Tooltip("This is a script attatched to the same gameobject as goalCircle.")]
        private RealisticGoalChecker realisticGoalChecker;

        [SerializeField]
        [Tooltip("Scales how fast the progress for the minigame increases.")]
        private float progressIncreaseIncrements;

        [SerializeField]
        [Tooltip("The animator that makes the target circle move, it is attatched to the object that parents the ReelGoalCircle.")]
        private Animator animator;

        [SerializeField]
        [Tooltip("Default value for how often the speed should be changed. This will be modified on runtime but should be set to a default value in the editor")]
        private float speedChangeDelay;

        private float _animationSpeed = 0.5f;
        private bool _reverseTarget = false;
        private float _catchProgress = 0f;

        [SerializeField]
        [Tooltip("The max amount of progress for the minigame to complete.")]
        private float maxCatchProgress = 100f;

        [SerializeField]
        [Tooltip("The time limit for this minigame.")]
        private float timeLimit;

        [SerializeField]
        [Tooltip("The UI text that shows how much time is left")]
        private TextMeshProUGUI timerText;

        private Fish _fishData;
        private float _currentTimeSpent = 0f;
        private bool _isMinigameActive = false;

        #endregion

        /// <summary>
        /// Sets up the required properties for the minigame
        /// Difficulty variable from the fishscriptableobject can be used to modify stats
        /// </summary>
        /// <param name="fishScriptable">The data of fish object being caught</param>
        public void InitializeMiniGame(Fish fishScriptable)
        {
            _fishData = fishScriptable;
            realisticCanvas.SetActive(true);
            progressSlider.value = 0f;
            _catchProgress = 0f;
            _currentTimeSpent = 0f;
            progressSlider.maxValue = maxCatchProgress;
        }

        /// <summary>
        /// Begins the minigame
        /// </summary>
        public void BeginMiniGame()
        {
            _isMinigameActive = true;
            StartCoroutine(ReelSpeedChange());
        }

        /// <summary>
        /// Disables minigame ui and runs the endcurrentminigame master script as a win
        /// </summary>
        public void WinMiniGame()
        {
            _isMinigameActive = false;
            realisticCanvas.SetActive(false);
            reelingMaster.EndCurrentMiniGame(true);
        }

        /// <summary>
        /// Disables minigame ui and runs the endcurrentminigame master script as a loss
        /// </summary>
        public void LoseMiniGame()
        {
            _isMinigameActive = false;
            realisticCanvas.SetActive(false);
            reelingMaster.EndCurrentMiniGame(false);
        }

        private void Update()
        {
            if (_isMinigameActive == false)
            {
                return;
            }

            if (HasEnoughProgress())
            {
                WinMiniGame();
            }

            if (IsPastTimeLimit()) 
            { 
                LoseMiniGame();
                return;
            }

            if (IsCursorOnGoal())
            {
                IncreaseProgress();
            }


            _currentTimeSpent = UpdateTime(_currentTimeSpent);
            UpdateTimer();


            playerCircle.transform.position = GetCursorPosition();

            //target
            animator.speed = _animationSpeed;
            animator.SetFloat("direction", _reverseTarget ? -1f : 1f);
        }

        /// <summary>
        /// Update the currentime spent on minigame
        /// </summary>
        /// /// <param name="currentTime">The value that will be added to</param>
        private float UpdateTime(float currentTime)
        {
            return currentTime += Time.deltaTime;
        }

        /// <summary>
        /// Returns true if timelimit has been reached
        /// </summary>
        private bool IsPastTimeLimit()
        {
            if (_currentTimeSpent >= timeLimit) { return true; }
            else { return false; }
        }

        /// <summary>
        /// Returns the current mouse position
        /// </summary>
        private Vector3 GetCursorPosition()
        {
            Vector3 mousePos = UnityEngine.Input.mousePosition;
            return mousePos;
        }

        /// <summary>
        /// Returns true if the players mouse is over the ui goal spot
        /// </summary>
        private bool IsCursorOnGoal()
        {
            return realisticGoalChecker.IsPointerOverUIElement();
        }

        /// <summary>
        /// Returns true if the catchprogress has reached the goal
        /// </summary>
        private bool HasEnoughProgress()
        {
            if (_catchProgress >= maxCatchProgress) { return true; }
            else { return false; }
        }

        /// <summary>
        /// Increases the current catchprogress by the process increment variable
        /// </summary>
        private void IncreaseProgress()
        {
            _catchProgress += progressIncreaseIncrements * Time.deltaTime;
            progressSlider.value = _catchProgress;
        }

        /// <summary>
        /// While minigame is active this will randomly change the speed of the goal animation spin
        /// This will also occasionally change the direction of it
        /// Speedchangedelay determines how often the speed will change
        /// </summary>
        IEnumerator ReelSpeedChange()
        {
            while (_isMinigameActive)
            {
                //random speed
                _animationSpeed = Random.Range(0.15f, 0.5f);

                //1 in 6 change to reverse direction
                if (Random.Range(1, 7) == 6)
                {
                    _reverseTarget = !_reverseTarget;
                }

                yield return new WaitForSeconds(speedChangeDelay);
            }
        }

        /// <summary>
        /// Updates the ui timer
        /// </summary>
        public void UpdateTimer()
        {
            timerText.text = ("Time Remaining: " + Mathf.RoundToInt(timeLimit - _currentTimeSpent));
        }
    }
}
