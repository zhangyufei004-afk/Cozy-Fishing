using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishingGame.GameManagement;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace FishingGame.Reeling
{

    /// <summary>
    /// This class contains the logic that allows the player to begin reeling.
    /// The main functionality for stage 1 of reeling is done here, once that stage is completed
    /// the class then lets ReelingMaster.cs do the rest
    /// </summary>
    public class ReelingInitiation : MonoBehaviour
    {
        #region Private Fields
        [Header("Reeling Scripts")]

        [SerializeField]
        [Tooltip("Reference to the master reeling script found in the reelingcontainer")]
        private ReelingMaster reelingMasterScript;

        [SerializeField]
        [Tooltip("Contains logic for detecting if a fish or pool is touching the hook, gameobject is attatched to the fishing rod")]
        private FishingHook fishingHook;

        [Header("ReelingUIElements")]

        [SerializeField]
        [Tooltip("Scales how fast the cast bar is charged when holding right click")]
        private float chargeScalar;

        [SerializeField]
        [Tooltip("Slider for how much charge the cast bar has for reeling")]
        private UnityEngine.UI.Slider chargeSlider;

        [SerializeField]
        [Tooltip("The button that is clicked when the fish is ready to be caught")]
        private List<UnityEngine.UI.Image> catchFishButtons;

        [SerializeField]
        [Tooltip("The fish iamge that follows the path the player goes")]
        private UnityEngine.UI.Image fishImage;

        [SerializeField]
        [Tooltip("The hook UI image element")]
        private UnityEngine.UI.Image hookImage;

        [SerializeField]
        [Tooltip("A image that is the child of the hookimage variable")]
        private UnityEngine.UI.Image hookPoint;

        [SerializeField]
        [Tooltip("The scalar for how fast the UI fish moves")]
        private float uiFishMoveSpeedScalar;

        [SerializeField]
        [Tooltip("The default sprite for stageone images")]
        private Sprite normalStageOneSprite;

        [Header("Stageone MiniGame variables")]

        [SerializeField]
        [Tooltip("The max amount of seconds a player would have to wait for a catch")]
        private int maxFishWaitTime;

        [SerializeField]
        [Tooltip("The min amount of seconds a player would have to wait for a catch")]
        private int minFishWaitTime;

        [SerializeField]
        [Tooltip("The particle effect played over the UI when a button is correctly pressed")]
        private ParticleSystem splashEffect;

        private bool _gameActive = false;
        private UnityEngine.UI.Image _activeButton;
        private int _currentNumber;
        private UnityEngine.UI.Image currentTravelToTarget;

        private bool _fishShouldMove = false;
        private bool _isStageOne = false;
        private int _stageOneDifficulty = 0;
        private int _catchSecondsToWait;
        private int _numbersPressed = 0;

        private InputAction _numberAction;

        [Header("Aiming and Charging cast")]

        [SerializeField]
        [Tooltip("The characters parent, this is used for position")]
        private GameObject characterParent;

        [SerializeField]
        [Tooltip("The character model this is used for rotation")]
        private GameObject characterModel;

        [SerializeField]
        [Tooltip("Rodbobber shows exactly where the line will be cast to, attatched to the fishing rod")]
        private GameObject rodBobber;

        [SerializeField]
        [Tooltip("Max amount of distance a cast can be")]
        private float fishingRange;

        private bool _reverseDirection = false;
        private bool _allowControls = true;
        private bool _isCharging = false;
        private float _chargePower = 0;
        private float _maxCharge = 8;

        private Vector3 _targetLocation;
        private Vector3 _aimStartPoint;
        private Vector3 _aimDirection;

        private InputAction _castAction;

        [Header("Misc")]

        [SerializeField]
        [Tooltip("Reference to the fish camera this is attatched to the hook")]
        private CinemachineCamera fishCamera;

        [SerializeField]
        [Tooltip("Reference to the default camera that is used")]
        private CinemachineCamera mainCamera;

        [SerializeField]
        [Tooltip("The animator attatched to the player")]
        private Animator characterAnimator;

        [SerializeField]
        [Tooltip("A temporary field that is currently used to general a generic 3D model for reeling visuailization")]
        private GameObject fishModelPrefab;
        
        #endregion

        public void OnEnable()
        {
            chargeSlider.maxValue = _maxCharge;
            fishCamera.gameObject.SetActive(false);

            InputActionAsset inputActions = InputSystem.actions;
            InputActionMap playerActionMap = inputActions.FindActionMap("Player");
            InputActionMap uiActionMap = inputActions.FindActionMap("UI");
            playerActionMap.Enable();
            _castAction = playerActionMap.FindAction("Cast");
            _numberAction = uiActionMap.FindAction("NumberKeys");
        }

        public void Update()
        {
            if (_numberAction.WasPressedThisFrame() && _activeButton != null && _gameActive == true)
            {
                if (_numberAction.ReadValue<float>() == _currentNumber)
                {
                    CorrectNumberPress();
                }
            }

            if (_fishShouldMove)
            {
                MoveFishToTarget();
            }

            if (fishingHook.HookIsOut == true || _allowControls == false)
            {
                return;
            }

            if (_castAction.IsPressed())
            {
                RightClickHeld();
            }

            if (_castAction.WasPressedThisFrame())
            {
                RightClickUsed();
            }

            if (_castAction.WasReleasedThisFrame())
            {
                RightClickReleased();
            }
        }

        #region Public Methods

        /// <summary>
        /// Sets appropreate values for Stage one of fishing and then runs the required functions
        /// </summary>
        public void BeginStageOne()
        {
            GameManager.Instance.GameEvents.SetPlayerOccupied(true);
            _isStageOne = true;
            fishImage.gameObject.SetActive(false);
            hookImage.gameObject.SetActive(true);
            _numbersPressed = 0;
            reelingMasterScript.SetCancelButtonVisibilty(true);
            FishingPool currentPool = fishingHook.GetPoolCurrentlyTouching();
            _stageOneDifficulty = currentPool.GetADifficultyInRange();
            _stageOneDifficulty = Mathf.Clamp(_stageOneDifficulty, 2, catchFishButtons.Count);
            _catchSecondsToWait = UnityEngine.Random.Range(minFishWaitTime, maxFishWaitTime);

            _activeButton = ChooseNextActiveSlot();
            SetupNextNumber();
            SetButtonVisible(_activeButton);

            StartCoroutine(StageOneCycle());
        }

        /// <summary>
        /// A public function that calls the private enable fish perspective function with a true value
        /// </summary>
        public void InitiateFishingPerspective()
        {
            ShouldEnableFishPerspective(true);
        }

        /// <summary>
        /// Creates a 3D fish model for reeling vizualization and then returns it
        /// </summary>
        /// <returns>Returns the 3D fish model that has been created</returns>
        public GameObject CreateAndReturn3DFishModel()
        {
            GameObject fishModel = Instantiate(fishModelPrefab, fishingHook.gameObject.transform.position, Quaternion.Euler(90, 0, 0));
            return fishModel;
        }
        
        /// <summary>
        /// This is a public function that enables the camera that tracks the fish during reeling
        /// This camera follows a hook gameobject that will always be ontop of the fish
        /// </summary>
        /// <param name="enable">True if you want to enable fish perspective camera, otherwise false</param>
        public void ShouldEnableFishPerspective(bool enable)
        {
            if (enable)
            {
                fishCamera.gameObject.SetActive(true);
            }
            else
            {
                fishCamera.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Turns the left and right click controls for reeling off or on based on the paremeter inputed
        /// </summary>
        /// <param name="enable">Enables controls if set to true otherwise disables controls</param>
        public void AreReelingControlsActive(bool enable)
        {
            if (enable)
            {
                _allowControls = true;
            }
            else
            {
                _allowControls = false;
            }
        }

        /// <summary>
        /// This can be called to cancel stage one of fishing, hiding the ui and restoring player controls
        /// </summary>
        public void CancelStageOne()
        {
            StopAllCoroutines();
            _fishShouldMove = false;
            _activeButton = null;
            foreach (var button in catchFishButtons)
            {
                button.gameObject.SetActive(false);
            }

            _isStageOne = false;
            fishImage.gameObject.SetActive(false);
            hookImage.gameObject.SetActive(false);
            fishingHook.PullBackHook();
            GameManager.Instance.GameEvents.SetPlayerOccupied(false);

        }

        #endregion

        #region Charging_and_throwing_line

        /// <summary>
        /// Setsup the variable for a cast being started
        /// </summary>
        private void BeginCharge()
        {
            chargeSlider.gameObject.SetActive(true);
            chargeSlider.value = 0;
            _chargePower = 0;
            _isCharging = true;
            rodBobber.SetActive(true);
            _reverseDirection = false;
        }

        /// <summary>
        /// Resets the variables when a cast is cancelled or completed
        /// </summary>
        private void ResetCharge()
        {
            chargeSlider.gameObject.SetActive(false);
            _isCharging = false;
            chargeSlider.value = 0;
            _chargePower = 0;
            rodBobber.SetActive(false);
        }

        /// <summary>
        /// Holding down right click charges the cast line of the rod.
        /// This will update the ui element representing the charge
        /// and also show the tragectory line if the player were to release
        /// </summary>
        private void ChargeLine()
        {
            _aimDirection = characterModel.transform.forward;
            _aimStartPoint = characterParent.transform.position;

            if (!_reverseDirection)
            {
                _chargePower += Time.deltaTime * chargeScalar;
                _chargePower = Mathf.Clamp(_chargePower, 0, _maxCharge);

                if (_chargePower == _maxCharge)
                {
                    _reverseDirection = true;
                }
            }
            else
            {
                _chargePower -= Time.deltaTime * chargeScalar;
                _chargePower = Mathf.Clamp(_chargePower, 0, _maxCharge);
                if (_chargePower == 0)
                {
                    _reverseDirection = false;
                }
            }
            
            chargeSlider.value = _chargePower;
            Vector3 aimLocation = _aimStartPoint + (_aimDirection * _chargePower);
            SetAimPoint(aimLocation);
        }

        /// <summary>
        /// Fires a downwards ray from the inputed location, then sets the rodbobber to where the rod hits
        /// </summary>
        /// <param name="locationToUse"> The location that will be raycasted from</param>
        private void SetAimPoint(Vector3 locationToUse)
        {
            RaycastHit hit;
            float maxDistance = fishingRange;
            LayerMask whatToHit = 1;

            Vector3 locationWithYOffset = new Vector3(locationToUse.x, locationToUse.y += 10, locationToUse.z);


            if (Physics.Raycast(locationWithYOffset, Vector3.down, out hit, maxDistance, whatToHit))
            {
                rodBobber.transform.position = hit.point;
            }
        }

        /// <summary>
        /// Makes the throw line animation play
        /// </summary>
        private void SetThrowAnimation()
        {
            _targetLocation = rodBobber.transform.position;
            characterAnimator.SetTrigger("ThrowTrigger");
            reelingMasterScript.DisableControls(true);
            AreReelingControlsActive(false);
        }

        /// <summary>
        /// Throws the fishing line at the location shown by the bobber
        /// This is run through an animation event
        /// </summary>
        private void ThrowLine()
        {
            fishingHook.HookIsOut = true;

            fishingHook.SetUpHookTravelToFishSpot(_targetLocation);
        }

        #endregion

        #region MouseControlFunctions

        /// <summary>
        /// Using right click will begin a charge if there is not one ongoing
        /// </summary>
        private void RightClickUsed()
        {
            if (_isCharging != true)
            {
                BeginCharge();
            }
        }

        /// <summary>
        /// Holding the right mouse button will continiously charge the line
        /// </summary>
        private void RightClickHeld()
        {
            ChargeLine();
        }

        /// <summary>
        /// Releasing right click will reset the current charge and throw the rod
        /// </summary>
        private void RightClickReleased()
        {
            if (_isCharging == true)
            {
                SetThrowAnimation();
            }
            ResetCharge();
        }

        #endregion

        #region StageoneReelingGame

        /// <summary>
        /// Keeps track of the number of buttons that have been correctly inputed,
        /// If enough numbers have been pressed, sets the next fishing stage
        /// Removes number from UI
        /// Calls for next number
        /// </summary>
        private void CorrectNumberPress()
        {
            _numbersPressed += 1;
            _activeButton.gameObject.SetActive(false);

            if (_numbersPressed == 1)
            {
                currentTravelToTarget = _activeButton;
                fishImage.gameObject.SetActive(true);
                fishImage.transform.position = _activeButton.transform.position;
            }
            else
            {
                if (fishImage.transform.position != currentTravelToTarget.transform.position)
                {
                    fishImage.transform.position = currentTravelToTarget.transform.position;
                }

                currentTravelToTarget = _activeButton;
                _fishShouldMove = true;
            }

            if (_numbersPressed == _stageOneDifficulty)
            {
                _isStageOne = false;
                _gameActive = false;
                _activeButton = null;
                StopAllCoroutines();
                StartCoroutine(WaitToReachHook());
            }
            else 
            {
                SetupNextButton();
            }
        }

        /// <summary>
        /// Runs the required functions that setup a new button and number
        /// </summary>
        private void SetupNextButton()
        {
            _activeButton = ChooseNextActiveSlot();
            SetupNextNumber();
            SetButtonVisible(_activeButton);
            SetNumberPressable();
        }

        /// <summary>
        /// Randomly chooses the next button that will be used
        /// </summary>
        /// <returns>Returns the next button to be used</returns>
        private UnityEngine.UI.Image ChooseNextActiveSlot()
        {
            bool validNumberFound = false;
            int slot = 0;

            while (validNumberFound != true)
            {
                slot = UnityEngine.Random.Range(0, catchFishButtons.Count());
                if (catchFishButtons[slot] != _activeButton)
                {
                    validNumberFound = true;
                }
            }

            return catchFishButtons[slot];
        }

        /// <summary>
        /// Sets a random button to be clickable to complete stage 1
        /// </summary>
        private void SetNumberPressable()
        {
            _activeButton.color = Color.green;
            _gameActive = true;
        }

        /// <summary>
        /// Sets the next number that needs to be pressed
        /// </summary>
        private void SetupNextNumber()
        {
            _currentNumber = UnityEngine.Random.Range(0, 10);
        }

        /// <summary>
        /// Sets the inputed image to be visible and updates its text to reflect the number it requires
        /// </summary>
        /// <param name="buttonToModify"></param>
        private void SetButtonVisible(UnityEngine.UI.Image buttonToModify)
        {
            TextMeshProUGUI buttonText = buttonToModify.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = _currentNumber.ToString();
            buttonToModify.gameObject.SetActive(true);
            buttonToModify.color = Color.grey;
            buttonToModify.sprite = normalStageOneSprite;
        }

        /// <summary>
        /// If currently in stageone this will roll a random value, if the random value is high enough the player will be prompted
        /// to select a clickable random button.
        /// If the roll is not high enough the chance for the next roll to be high enough is increased and the timer until this method is called again
        /// is restarted
        /// </summary>
        private void BeginFishing()
        {
            if (_isStageOne)
            {
                SetNumberPressable();
            }
        }

        /// <summary>
        /// Moves fish UI image towards the current travel target unless the fish is already there
        /// </summary>
        private void MoveFishToTarget()
        {
            if (currentTravelToTarget.transform.position != fishImage.transform.position)
            {
                fishImage.transform.position = Vector3.MoveTowards(fishImage.transform.position, currentTravelToTarget.transform.position, uiFishMoveSpeedScalar * Time.deltaTime);
            }
        }
        
        /// <summary>
        /// Run when the fish should be at the hook, will cause the gameobjects to dissapear visually
        /// and begin the minigame portion of reeling
        /// </summary>
        private void FishAtHook()
        {
            _fishShouldMove = false;
            fishImage.gameObject.SetActive(false);
            hookImage.gameObject.SetActive(false);
            fishingHook.AttempToFishFromCurrentLocation();
        }

        /// <summary>
        /// Will run BeginFishing after variable stageOneCycleSecondsToWait seconds
        /// </summary>
        /// <returns>When timer is finished BeginFishing() is run</returns>
        private IEnumerator StageOneCycle()
        {
            yield return new WaitForSeconds(_catchSecondsToWait);
            BeginFishing();
        }

        /// <summary>
        /// A short 2 second timer split into two parts
        /// After 1 second the current travel to target changes to the hook image
        /// After 1 more second the game will transition into the minigame portion fo reeling
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitToReachHook()
        {
            yield return new WaitForSeconds(1f);
            currentTravelToTarget = hookPoint;
            yield return new WaitForSeconds(1f);
            if (_fishShouldMove)
            {
                FishAtHook();
            }
        }
        #endregion
    }
}
