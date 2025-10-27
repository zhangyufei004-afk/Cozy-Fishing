using FishingGame.FishSystem;
using FishingGame.Reeling;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace FishingGame.Reeling
{
    public class DialMiniGame : MonoBehaviour, IReelingMinigame
    {
        [SerializeField]
        [Tooltip("Reference to the ReelingMaster script.")]
        private ReelingMaster reelingMaster;

        [SerializeField]
        [Tooltip("The UI gameobject that parents the UI.")]
        private GameObject dialCanvas;

        [SerializeField]
        [Tooltip("The slider that shows the total progress of this minigame")]
        private UnityEngine.UI.Slider progressSlider;

        [SerializeField]
        private float baseSpeed;
        [SerializeField]
        private float difficultyMultiplier;
        [SerializeField]
        private DialCollision dialCollision;
        [SerializeField]
        private Animator dialAnimator;
        [SerializeField]
        private AudioSource rightSound;
        [SerializeField]
        private AudioSource wrongSound;
        [SerializeField]
        private GameObject target;
        [SerializeField]
        private GameObject target2;
        [SerializeField]
        private int startingProgress;
        [SerializeField]
        private int goalProgress;
        [SerializeField]
        private float maxTime;

        private InputAction _mouseInput;
        private bool _gameActive = false;

        private int _catchProgress;
        private float _dialSpeed;

        public void InitializeMiniGame(IFishAble fishScriptable)
        {
            dialCanvas.SetActive(true);

            int difficulty = fishScriptable.GetCatchDifficulty();
            _dialSpeed = baseSpeed + (difficulty * difficultyMultiplier);
            dialAnimator.speed = _dialSpeed;

            _catchProgress = startingProgress;
            progressSlider.value = _catchProgress;
            progressSlider.maxValue = goalProgress;

            target.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
            target2.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        }

        public void BeginMiniGame()
        {
            _gameActive = true;
        }

        public void WinMiniGame()
        {
            _gameActive = false;
            dialCanvas.SetActive(false);
            reelingMaster.EndCurrentMiniGame(true);
        }

        public void LoseMiniGame()
        {
            _gameActive = false;
            dialCanvas.SetActive(false);
            reelingMaster.EndCurrentMiniGame(false);
        }

        private void OnEnable()
        {
            InputActionAsset inputAsset = InputSystem.actions;
            InputActionMap uiActionMap = inputAsset.FindActionMap("UI");
            uiActionMap.Enable();
            _mouseInput = uiActionMap.FindAction("Click");
        }

        private void Update()
        {
            if (_gameActive)
            {
                if (dialCollision.InTarget && _mouseInput.WasPressedThisFrame())
                {
                    _catchProgress++;
                    rightSound.Play();
                    target.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
                }
                else if (dialCollision.InTarget2 && _mouseInput.WasPressedThisFrame())
                {
                    _catchProgress++;
                    rightSound.Play();
                    target2.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
                }
                else if (!dialCollision.InTarget && !dialCollision.InTarget2 && _mouseInput.WasPressedThisFrame())
                {
                    _catchProgress--;
                    wrongSound.Play();
                    if (_catchProgress < 0) _catchProgress = 0;
                }

                progressSlider.value = _catchProgress;

                if (_catchProgress >= goalProgress) WinMiniGame();
                if (_catchProgress == 0) LoseMiniGame();
            }
        }

    }
}
