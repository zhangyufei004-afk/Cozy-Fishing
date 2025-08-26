using System;
using TMPro;
using UnityEngine;
using UnityEngine.Android;

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

    public class GameTime : MonoBehaviour
    {
        [Tooltip("Tracks the current time of day as a float")]
        public float currentTimeOfDay { get; private set; }

        [SerializeField]
        [Tooltip("The time that the game will start at, in ingame hours")]
        private float initialDayTimeHour;

        [SerializeField]
        [Tooltip("A tweakable value that scales how fast game time is calculated")]
        private int gameSpeed = 1;

        private bool _timeActive = true;
        private float _secondsPerDay = 1440f;
        private float _elapsedTimeInCurrentState = 0f;
        private float _maxSecondsPerState;
        private ETimeOfDay _timeAsState;

        // UI
        [SerializeField]
        [Tooltip("The text component of the time UI")]
        private TextMeshProUGUI uiText;
        
        [SerializeField]
        [Tooltip("The UI parent")]
        private GameObject gameTimeUI;



        // 24 minutes = 1 in game day

        private void OnEnable()
        {
            int numberOfMembers = Enum.GetNames(typeof(ETimeOfDay)).Length;
            _maxSecondsPerState = _secondsPerDay / numberOfMembers;
            currentTimeOfDay = initialDayTimeHour * 60f;

            SetInitialTimeState();
            UpdateUITImer();
        }

        private void Update()
        {
            if (!_timeActive) { return;  }

            currentTimeOfDay += Time.deltaTime * gameSpeed;
            _elapsedTimeInCurrentState += Time.deltaTime * gameSpeed;

            if (CheckIfTimePeriodChange()) { SetNewDayState(); }
            if (CheckIfDayPassed()) { ProcessEndOfDay(); }
        }

        private bool CheckIfTimePeriodChange()
        {
            if (_elapsedTimeInCurrentState >= _maxSecondsPerState) { return true; }
            return false;
        }

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

        private void ProcessEndOfDay()
        {
            currentTimeOfDay = 0f;
        }

        private bool CheckIfDayPassed()
        {
            if (currentTimeOfDay >= _secondsPerDay) { return true; }
            else { return false; }
        }

        // Morning 6 - 12
        // Afternoon 12 - 18
        // Evening 18 - 24
        // Night 0 - 6
        private void SetInitialTimeState()
        {
            if (initialDayTimeHour >= 6 && initialDayTimeHour < 12)
            {
                _timeAsState = ETimeOfDay.Morning;
            }
            else if (initialDayTimeHour >= 12 && initialDayTimeHour < 18)
            {
                _timeAsState = ETimeOfDay.Afternoon;
            }
            else if (initialDayTimeHour >= 18  && initialDayTimeHour < 24)
            {
                _timeAsState = ETimeOfDay.Evening;
            }
            else
            {
                _timeAsState = ETimeOfDay.Night;
            }
        }

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

        public void ShowGameTimeUI(bool hide)
        {
            gameTimeUI.SetActive(hide);
        }

        public ETimeOfDay GetTimePeriod()
        {
            return _timeAsState;
        }

        public void SetTimeActive(bool active)
        {
            if (active) { _timeActive = true; }
            else { _timeActive = false; }
        }

        #endregion

    }
}
