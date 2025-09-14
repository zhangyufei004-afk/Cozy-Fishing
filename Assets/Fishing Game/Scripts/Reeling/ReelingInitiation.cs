using System.Collections;
using System.Collections.Generic;
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

        private int _chanceToCatchFishThisTick = 1;
        private int _maxChanceToCatchFish = 10;
        private bool _isStageOne = false;
        private UnityEngine.UI.Button _activeButton;

        [SerializeField]
        [Tooltip("The amount of seconds to wait inbetween a chance roll in stage 1 of fishing")]
        private int stageOneCycleSecondsToWait;

        [SerializeField]
        [Tooltip("How many seconds the fish button is clickable for")]
        private int buttonClickWindowTime;

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
        private List<UnityEngine.UI.Button> catchFishButtons;

        private InputAction _castAction;
        private InputAction _reelAction;
        private Vector3 _targetLocation;
        #endregion

        public void OnEnable()
        {
            chargeSlider.maxValue = _maxCharge;
            fishCamera.gameObject.SetActive(false);

            InputActionAsset inputActions = InputSystem.actions;
            InputActionMap playerActionMap = inputActions.FindActionMap("Player");
            playerActionMap.Enable();
            _castAction = playerActionMap.FindAction("Cast");
            _reelAction = playerActionMap.FindAction("Reel");
        }

        public void Update()
        {
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
            reelingMasterScript.SetCancelButtonVisibilty(true);

            foreach (var button in catchFishButtons)
            {
                button.interactable = false;
                button.gameObject.SetActive(true);
                button.image.color = Color.grey;
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

        public void ButtonClicked()
        {
            _isStageOne = false;
            fishingHook.AttempToFishFromCurrentLocation();

            foreach (var button in catchFishButtons)
            {
                button.gameObject.SetActive(false);
            }
        }

        public void CancelStageOne()
        {
            foreach (var button in catchFishButtons)
            {
                button.gameObject.SetActive(false);
            }

            _isStageOne = false;
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

        private void SetCatchButtonClickable()
        {
            int randomIndex = Random.Range(0, catchFishButtons.Count);

            var button = catchFishButtons[randomIndex];
            _activeButton = button;

            _chanceToCatchFishThisTick = 1;
            _activeButton.interactable = true;
            _activeButton.image.color = Color.green;
            StartCoroutine(CatchWindow());
        }

        private void IncreaseChanceToCatch()
        {
            if (_isStageOne)
            {
                Mathf.Clamp(_chanceToCatchFishThisTick += 1, 0, _maxChanceToCatchFish);
            }
        }

        private void AttemptToCatch()
        {
            if (_isStageOne)
            {
                int rolledNumber = Random.Range(0, _maxChanceToCatchFish + 1);

                if (_chanceToCatchFishThisTick > rolledNumber)
                {
                    SetCatchButtonClickable();
                }
                else
                {
                    IncreaseChanceToCatch();
                    StartCoroutine(StageOneCycle());
                }
            }
        }

        private IEnumerator StageOneCycle()
        {
            yield return new WaitForSeconds(stageOneCycleSecondsToWait);
            AttemptToCatch();
        }

        private IEnumerator CatchWindow()
        {
            yield return new WaitForSeconds(buttonClickWindowTime);

            if (_isStageOne)
            {
                _activeButton.interactable = false;
                _activeButton.gameObject.SetActive(true);
                _activeButton.image.color = Color.grey;
                BeginStageOne();
            }
        }
    }
}
