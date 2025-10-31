using System;
using TMPro;
using UnityEngine;

namespace FishingGame.GameTime
{
    /// <summary>
    /// Enum to represent time of day.
    /// </summary>
    public enum ETimeOfDay
    {
        Morning = 0,
        Afternoon = 1,
        Evening = 2,
        Night = 3
    }

    /// <summary>
    /// This class tracks the current time of the day.
    /// It tracks these as both a float value and an enum representing the state of the day
    /// </summary>
    public class InGameTime : MonoBehaviour
    {
        [Tooltip("Tracks the current time of day as a float")]
        public float CurrentTimeOfDay { get; private set; }
        public float DayLength => secondsPerDay;

        [Header("Time Properties")]
        [Tooltip("The length of day in game as seconds.")]
        [SerializeField] private float secondsPerDay = 1440f;
        
        [SerializeField]
        [Tooltip("The time period that the game ewill start in")]
        private ETimeOfDay initialStateOfDay;

        [SerializeField]
        [Tooltip("A tweakable value that scales how fast game time is calculated")]
        private int gameSpeed = 1;
        
        private bool _timeActive = true;
        
        private float _elapsedTimeInCurrentState = 0f;
        private float _maxSecondsPerState;
        private ETimeOfDay _timeAsState;

        [Header("UI Elements")]
        [SerializeField]
        [Tooltip("The text component of the time UI")]
        private TextMeshProUGUI uiText;
        
        [SerializeField]
        [Tooltip("The UI parent")]
        private GameObject gameTimeUI;
        
        private void OnEnable()
        {
            int numberOfMembers = Enum.GetNames(typeof(ETimeOfDay)).Length;
            _maxSecondsPerState = secondsPerDay / numberOfMembers;

            SetInitialTimeState();
            UpdateUITImer();
        }

        private void Update()
        {
            if (!_timeActive) { return;  }

            CurrentTimeOfDay += Time.deltaTime * gameSpeed;
            _elapsedTimeInCurrentState += Time.deltaTime * gameSpeed;

            if (CheckIfTimePeriodChange()) { SetNewDayState(); }
            if (CheckIfDayPassed()) { ProcessEndOfDay(); }
        }

        /// <summary>
        /// Returns true if the time spent in the current day state is greater than the 
        /// </summary>
        /// <returns></returns>
        private bool CheckIfTimePeriodChange()
        {
            if (_elapsedTimeInCurrentState >= _maxSecondsPerState) { return true; }
            return false;
        }

        /// <summary>
        /// Sets the day state to be equal to the next state in the day cycle
        /// </summary>
        private void SetNewDayState()
        {
            _elapsedTimeInCurrentState = 0f;
            if (_timeAsState == ETimeOfDay.Night)
            {
                _timeAsState = ETimeOfDay.Morning;
            }
            else
            {
                _timeAsState = _timeAsState += 1;
            }

            UpdateUITImer();
        }

        /// <summary>
        /// Sets the current time of day float value to be 0
        /// </summary>
        private void ProcessEndOfDay()
        {
            CurrentTimeOfDay = 0f;
        }

        /// <summary>
        /// Returns true if current time of day is greater than the seconds per day
        /// </summary>
        /// <returns>True if the day has passed, false otherwise</returns>
        private bool CheckIfDayPassed()
        {
            if (CurrentTimeOfDay >= secondsPerDay) { return true; }
            else { return false; }
        }

        /// <summary>
        /// Sets the state of the day enum to be equal to the initial state of day enum
        /// </summary>
        private void SetInitialTimeState()
        {
            _timeAsState = initialStateOfDay;
        }

        /// <summary>
        /// Sets the timer UI text to say what state of day it is
        /// </summary>
        private void UpdateUITImer()
        {
            switch (_timeAsState)
            {
                case ETimeOfDay.Morning:
                    uiText.text = "Morning";
                    break;
                case ETimeOfDay.Afternoon:
                    uiText.text = "Afternoon";
                    break;
                case ETimeOfDay.Evening:
                    uiText.text = "Evening";
                    break;
                case ETimeOfDay.Night:
                    uiText.text = "Night";
                    break;
            }
        }

        #region Public Functions

        /// <summary>
        /// Shows or hides the game time UI
        /// </summary>
        /// <param name="hide">True will set the UI to appear, false will make it be hidden</param>
        public void ShowGameTimeUI(bool hide)
        {
            gameTimeUI.SetActive(hide);
        }

        /// <summary>
        /// Returns the time period of the current day
        /// </summary>
        /// <returns>Time period of current day</returns>
        public ETimeOfDay GetTimePeriod()
        {
            return _timeAsState;
        }

        /// <summary>
        /// Sets the days timer to be either active or inactive
        /// </summary>
        /// <param name="active">True will set time to be active, false will set it to not</param>
        public void SetTimeActive(bool active)
        {
            if (active) { _timeActive = true; }
            else { _timeActive = false; }
        }

        #endregion

    }
}
