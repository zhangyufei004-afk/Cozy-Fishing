using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        [SerializeField]
        private GameObject characterParent;

        [SerializeField] 
        private GameObject characterModel;

        private Vector3 _aimStartPoint;
        private Vector3 _aimDirection;

        private bool _allowControls = true;
        private bool _isCharging = false;
        private float _chargePower = 0;
        private float _maxCharge = 8;
        private bool _reverseDirection = false;
        private UnityEngine.UI.Image currentTravelToTarget;

        private int _numbersPressed = 0;
        private Dictionary<UnityEngine.UI.Image, int> _activeNumbers;
        private bool _isStageOne = false;
        private UnityEngine.UI.Image _activeNumber;
        private int _stageOneDifficulty = 0;
        private int _catchSecondsToWait;
        private float _timeToPressNumbers;

        [SerializeField]
        [Tooltip("The max amount of seconds a player would have to wait for a catch")]
        private int maxFishWaitTime;

        [SerializeField]
        [Tooltip("The min amount of seconds a player would have to wait for a catch")]
        private int minFishWaitTime;

        [SerializeField]
        [Tooltip("Reference to the master reeling script found in the reelingcontainer")]
        private ReelingMaster reelingMasterScript;

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
        [Tooltip("Scales how fast the cast bar is charged when holding right click")]
        private float chargeScalar;

        [SerializeField]
        [Tooltip("Slider for how much charge the cast bar has for reeling")]
        private UnityEngine.UI.Slider chargeSlider;

        [SerializeField]
        [Tooltip("Rodbobber shows exactly where the line will be cast to, attatched to the fishing rod")]
        private GameObject rodBobber;

        [SerializeField]
        [Tooltip("Contains logic for detecting if a fish or pool is touching the hook, gameobject is attatched to the fishing rod")]
        private FishingHook fishingHook;

        [SerializeField]
        [Tooltip("A temporary field that is currently used to general a generic 3D model for reeling visuailization")]
        private GameObject fishModelPrefab;

        [SerializeField]
        [Tooltip("Max amount of distance a cast can be")]
        private float fishingRange;

        [SerializeField]
        [Tooltip("The button that is clicked when the fish is ready to be caught")]
        private List<UnityEngine.UI.Image> catchFishButtons;

        [SerializeField]
        [Tooltip("The fish iamge that follows the path the player goes")]
        private UnityEngine.UI.Image fishImage;

        [SerializeField]
        [Tooltip("The default sprite for stageone images")]
        private Sprite normalStageOneSprite;

        [SerializeField]
        [Tooltip("The sprite used by the final number to be pressed in stage one")]
        private UnityEngine.UI.Image hookImage;

        [SerializeField]
        [Tooltip("The scalar for how fast the UI fish moves")]
        private float uiFishMoveSpeedScalar;

        private InputAction _castAction;
        private InputAction _numberAction;
        private Vector3 _targetLocation;
        #endregion

        public void OnEnable()
        {
            chargeSlider.maxValue = _maxCharge;
            fishCamera.gameObject.SetActive(false);
            _activeNumbers = new Dictionary<UnityEngine.UI.Image, int>();

            InputActionAsset inputActions = InputSystem.actions;
            InputActionMap playerActionMap = inputActions.FindActionMap("Player");
            InputActionMap uiActionMap = inputActions.FindActionMap("UI");
            playerActionMap.Enable();
            _castAction = playerActionMap.FindAction("Cast");
            _numberAction = uiActionMap.FindAction("NumberKeys");
        }

        public void Update()
        {
            if (_numberAction.WasPressedThisFrame() && _activeNumber != null)
            {
                if (_numberAction.ReadValue<float>() == _activeNumbers[_activeNumber])
                {
                    CorrectNumberPress();
                }
            }

            if (_isStageOne)
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
            _isStageOne = true;
            fishImage.gameObject.SetActive(false);
            hookImage.gameObject.SetActive(true);
            _activeNumbers.Clear();
            _numbersPressed = 0;
            reelingMasterScript.SetCancelButtonVisibilty(true);
            FishingPool currentPool = fishingHook.GetPoolCurrentlyTouching();
            _stageOneDifficulty = currentPool.GetADifficultyInRange();

            _timeToPressNumbers = _stageOneDifficulty;

            _stageOneDifficulty = Mathf.Clamp(_stageOneDifficulty, 0, catchFishButtons.Count);
            _catchSecondsToWait = UnityEngine.Random.Range(minFishWaitTime, maxFishWaitTime);

            for (int i = 0; i < _stageOneDifficulty; i++)
            {
                int numberToPress = UnityEngine.Random.Range(0, 9);
                _activeNumbers.Add(catchFishButtons[i], numberToPress);
            }

            foreach (var button in _activeNumbers)
            {
                TextMeshProUGUI buttonText = button.Key.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = button.Value.ToString();
                button.Key.gameObject.SetActive(true);
                button.Key.color = Color.grey;
                button.Key.sprite = normalStageOneSprite;
            }



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
        /// Keeps track of the number of buttons that have been correctly inputed,
        /// If enough numbers have been pressed, sets the next fishing stage
        /// Removes number from UI
        /// Calls for next number
        /// </summary>
        private void CorrectNumberPress()
        {
            _numbersPressed += 1;
            _activeNumbers.Remove(_activeNumber);
            _activeNumber.gameObject.SetActive(false);

            if (_numbersPressed == 1)
            {
                fishImage.gameObject.SetActive(true);
                fishImage.transform.position = _activeNumber.transform.position;
            }
            else
            {
                currentTravelToTarget = _activeNumber;
            }


            if (_numbersPressed == _stageOneDifficulty)
            {
                _isStageOne = false;
                _activeNumber = null;
                StopAllCoroutines();
                fishingHook.AttempToFishFromCurrentLocation();
            }
            else { SetNumberPressable(); }
        }

        /// <summary>
        /// This can be called to cancel stage one of fishing, hiding the ui and restoring player controls
        /// </summary>
        public void CancelStageOne()
        {
            StopAllCoroutines();
            _activeNumber = null;
            foreach (var button in catchFishButtons)
            {
                button.gameObject.SetActive(false);
            }

            _isStageOne = false;
            fishImage.gameObject.SetActive(false);
            hookImage.gameObject.SetActive(false);
            fishingHook.PullBackHook();
        }

        #endregion

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
        /// Sets a random button to be clickable to complete stage 1
        /// </summary>
        private void SetNumberPressable()
        {
            UnityEngine.UI.Image[] keys = _activeNumbers.Keys.ToArray();

            int randomIndex = UnityEngine.Random.Range(0, _activeNumbers.Count);
            var button = keys[randomIndex];
            _activeNumber = button;

            _activeNumber.color = Color.green;
            StartCoroutine(CatchWindow());
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
        /// Will run BeginFishing after variable stageOneCycleSecondsToWait seconds
        /// </summary>
        /// <returns>When timer is finished BeginFishing() is run</returns>
        private IEnumerator StageOneCycle()
        {
            yield return new WaitForSeconds(_catchSecondsToWait);
            BeginFishing();
        }

        /// <summary>
        /// A timer that controls how long a stage one button is clickable for
        /// If the timer is exceeded and the player hasn't completed stage one
        /// all buttons reset and stage one is restarted as the player was too slow to click
        /// Seconds used in this is based on the variable buttonClickWindowTime
        /// </summary>
        /// <returns>Resets stage one and stops buttons being clickable if player has taken too long</returns>
        private IEnumerator CatchWindow()
        {
            yield return new WaitForSeconds(_timeToPressNumbers);

            if (_isStageOne)
            {
                _activeNumber.gameObject.SetActive(true);
                _activeNumber.color = Color.grey;
                BeginStageOne();
            }
        }

        private void MoveFishToTarget()
        {
            if (currentTravelToTarget.transform.position != fishImage.transform.position)
            {
                fishImage.transform.position = Vector3.MoveTowards(fishImage.transform.position, currentTravelToTarget.transform.position, uiFishMoveSpeedScalar * Time.deltaTime);
            }
        }
    }
}
