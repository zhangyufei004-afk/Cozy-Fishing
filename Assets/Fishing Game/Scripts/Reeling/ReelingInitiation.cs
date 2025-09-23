using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishingGame.GameManagement;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
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
        [Tooltip("The Button that is pressed to catch fish")]
        private UnityEngine.UI.Button catchButton;

        [Header("Stageone MiniGame variables")]

        [SerializeField]
        [Tooltip("The max amount of seconds a player would have to wait for a catch")]
        private int maxFishWaitTime;

        [SerializeField]
        [Tooltip("The min amount of seconds a player would have to wait for a catch")]
        private int minFishWaitTime;

        [SerializeField]
        [Tooltip("The prefab of the object that swims up to the reel")]
        private GameObject reelSwimmerPrefab;

        [SerializeField]
        [Tooltip("Max distance the fish can be")]
        private int maxDistance;

        private int _fishDissapearTimeVisual = 1;
        
        private GameObject _fishSwim;

        private bool _fishAtHook = false;
        private bool _gameActive = false;
        private bool _isStageOne = false;
        private int _stageOneDifficulty = 0;
        private int _catchSecondsToWait;

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
            _castAction = playerActionMap.FindAction("Reel");
            _numberAction = uiActionMap.FindAction("NumberKeys");
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
            SetupVariables();
            
            StartCoroutine(SpawnFishTimer(_catchSecondsToWait));
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
            GameObject fishModel = Instantiate(fishModelPrefab, fishingHook.gameObject.transform.position, Quaternion.Euler(0, 90, 90));
            return fishModel;
        }

        public void FishAtHook()
        {
            _fishAtHook = true;
            SetButtonInteractable(true);

            StartCoroutine(FishCatchTimer(5));
        }

        public void FishCaught()
        {
            _isStageOne = false;
            StageOneUICleanup();
            Destroy(_fishSwim);
            StopAllCoroutines();

            fishingHook.AttempToFishFromCurrentLocation();
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
            StageOneUICleanup();
            Destroy(_fishSwim);

            _isStageOne = false;
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

        private void SpawnFishShadow()
        {
            _fishSwim = Instantiate(reelSwimmerPrefab, SetFishSpawnLocation(), Quaternion.Euler(90, 0, 0));
            _fishSwim.GetComponent<StageOneSwimmer>().SetupVariables(fishingHook.transform.position, this);
        }

        private Vector3 SetFishSpawnLocation()
        {
            int zToAdd = UnityEngine.Random.Range(-maxDistance, maxDistance);
            int xToAdd = UnityEngine.Random.Range(-maxDistance, maxDistance);

            Vector3 currentHookLocation = fishingHook.gameObject.transform.position;
            Vector3 trialLocation = new Vector3(currentHookLocation.x += xToAdd, currentHookLocation.y - 1, currentHookLocation.z += zToAdd);

            return trialLocation;
        }

        private void FishGotAway()
        {
            _fishAtHook = false;
            SetButtonInteractable(false);

            _fishSwim.GetComponent<StageOneSwimmer>().SetupVariables(SetFishSpawnLocation(), this);
            StartCoroutine(DespawnFishTimer(_fishDissapearTimeVisual));
        }

        private IEnumerator SpawnFishTimer(int waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            SpawnFishShadow();
        }

        private IEnumerator FishCatchTimer(int waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            if (_isStageOne == true) { FishGotAway(); }
            
        }

        private IEnumerator DespawnFishTimer(int waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            Destroy(_fishSwim);
            StartCoroutine(SpawnFishTimer(_catchSecondsToWait));
        }    

        private void SetButtonInteractable(bool isInteractable)
        {
            if (isInteractable)
            {
                catchButton.image.color = Color.green;
                catchButton.interactable = true;
            }
            else
            {
                catchButton.image.color = Color.grey;
                catchButton.interactable = false;
            }
        }

        private void StageOneUICleanup()
        {
            catchButton.gameObject.SetActive(false);
        }

        private void SetupVariables()
        {
            GameManager.Instance.GameEvents.SetPlayerOccupied(true);
            _isStageOne = true;
            FishingPool currentPool = fishingHook.GetPoolCurrentlyTouching();
            _stageOneDifficulty = currentPool.GetADifficultyInRange();

            reelingMasterScript.SetCancelButtonVisibilty(true);
            catchButton.gameObject.SetActive(true);
            SetButtonInteractable(false);


            _catchSecondsToWait = UnityEngine.Random.Range(minFishWaitTime, maxFishWaitTime);
        }

        #endregion
    }
}
