using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeFishingMechanics
{
    public class RealisticMiniGame : MonoBehaviour, IReelingMinigame
    {
        #region Public Variables

        #endregion

        #region Private Fields

        [SerializeField]
        private ReelingMaster ReelingMaster;

        [SerializeField]
        private GameObject RealisticCanvas;

        [SerializeField]
        private Image PlayerCircle;

        [SerializeField]
        private Image GoalCircle;

        [SerializeField]
        private Slider ProgressSlider;

        public Animator Animator;
        [Range(-0f, 0.5f)]
        public float AnimationSpeed;
        public bool ReverseTarget;
        public float SpeedChangeDelay;

        private float _catchProgress = 0f;

        [SerializeField]
        private float MaxCatchProgress = 100f;

        [SerializeField]
        private float TimeLimit;

        public float _currentTimeSpent = 0f;

        private bool _isMinigameActive = false;

        #endregion


        public void InitializeMiniGame(int fishCatchDifficulty)
        {
            RealisticCanvas.SetActive(true);
            ProgressSlider.value = 0f;
            _catchProgress = 0f;
            _currentTimeSpent = 0f;
        }

        public void BeginMiniGame()
        {
            _isMinigameActive = true;
        }

        public void WinMiniGame()
        {
            _isMinigameActive = false;
            RealisticCanvas.SetActive(false);
            ReelingMaster.EndCurrentMiniGame(true);
        }

        public void LoseMiniGame()
        {
            _isMinigameActive = false;
            RealisticCanvas.SetActive(false);
            ReelingMaster.EndCurrentMiniGame(false);
        }
        
        public void Start()
        {
            StartCoroutine(ReelSpeedChange()); 
        }

        private void Update()
        {
            if (_isMinigameActive == false)
            {
                return;
            }

            if (IsPastTimeLimit()) 
            { 
                LoseMiniGame();
                return;
            }

            _currentTimeSpent = UpdateTime(_currentTimeSpent);


            PlayerCircle.transform.position = GetCursorPosition();

            //target
            Animator.speed = AnimationSpeed;
            Animator.SetFloat("direction", ReverseTarget ? -1f : 1f);
        }

        private float UpdateTime(float currentTime)
        {
            return currentTime += Time.deltaTime;
        }

        private bool IsPastTimeLimit()
        {
            if (_currentTimeSpent >= TimeLimit) { return true; }
            else { return false; }
        }

        private Vector3 GetCursorPosition()
        {
            Vector3 mousePos = Input.mousePosition;
            return mousePos;
        }

        IEnumerator ReelSpeedChange()
        {
            while (true)
            {

                //random speed
                AnimationSpeed = Random.Range(0.15f, 0.5f);

                //1 in 6 change to reverse direction
                if (Random.Range(1, 7) == 6)
                {
                    ReverseTarget = !ReverseTarget;
                }

                yield return new WaitForSeconds(SpeedChangeDelay);
            }
        }

    }
}
