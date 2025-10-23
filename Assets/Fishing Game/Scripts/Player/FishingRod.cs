using FishingGame.FishSystem;
using FishingGame.GameManagement;
using FishingGame.Items;
using System.Collections;
using System.Runtime.CompilerServices;
using FishingGame.Items.Bait;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using System.ComponentModel.Design;
using FishingGame.Inventory;
using UnityEditor.UIElements;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Linq;
using FishingGame.Player;

namespace FishingGame.Reeling
{
    /// <summary>
    /// Used to set the text and influence how fast fish is attracted to line, based on the throw power
    /// </summary>
    internal enum ECastingResult
    {
        None = 0,
        Average = 1,
        Good = 2,
        Amazing = 3
    }

    /// <summary>
    /// Contains the logic for charging and throwing the initial fishing line
    /// </summary>
    public class FishingRod : MonoBehaviour
    {
        [Header("Scrip References")]

        [SerializeField]
        [Tooltip("Reference to the master reeling script found in the reelingcontainer")]
        private ReelingMaster reelingMasterScript;

        [SerializeField]
        [Tooltip("A reference to the initiation script attatched to player.")]
        private ReelingInitiation initiationScript;

        [SerializeField]
        [Tooltip("Reference to the player controller script")]
        private PlayerController playerController;

        [SerializeField]
        [Tooltip("A reference to the fishing hook script which is attatched to a fishing rod.")]
        private FishingHook fishingHook;

        [Header("Aiming and charging cast")]

        [SerializeField]
        [Tooltip("The characters parent, this is used for position")]
        private GameObject characterParent;

        [SerializeField]
        [Tooltip("The character model this is used for rotation")]
        private GameObject characterModel;

        [SerializeField]
        [Tooltip("Max amount of distance a cast can be")]
        private float fishingRange;

        [SerializeField]
        [Tooltip("Rodbobber shows exactly where the line will be cast to, attatched to the fishing rod")]
        private GameObject rodBobber;

        [SerializeField]
        [Tooltip("Layer that water is set to")]
        LayerMask waterLayer;

        [SerializeField]
        [Tooltip("Layer that environment is set to")]
        LayerMask blockFishingLayers;

        private int _aimingYOffset = 5;
        private bool _reverseDirection = false;
        private bool _allowControls = true;
        private bool _isCharging = false;
        private float _chargePower = 0;
        private float _maxCharge = 8;
        private float _chargePowerMinimum = 1f;
        private float _chargePowerAverageMaxValue = 3f;
        private float _chargePowerGoodMaxValue = 6f;

        private float _amazingFishMinWaitTime = 1;
        private float _amazingFishMaxWaitTime = 2;

        private ECastingResult _throwLineResult;

        private Vector3 _targetLocation;
        private Vector3 _aimStartPoint;
        private Vector3 _aimDirection;
        private Vector3 _aimLoaction;

        private InputAction _castAction;

        [Header("ReelingUIElements")]

        [SerializeField]
        [Tooltip("Scales how fast the cast bar is charged when holding right click")]
        private float chargeScalar;

        [SerializeField]
        [Tooltip("Slider for how much charge the cast bar has for reeling")]
        private UnityEngine.UI.Slider chargeSlider;

        [SerializeField]
        [Tooltip("The text that shows if you reel in too soon")]
        private TextMeshProUGUI tooSoonText;

        [Header("Misc")]

        [SerializeField]
        [Tooltip("The animator attatched to the player")]
        private Animator characterAnimator;

        [SerializeField]
        [Tooltip("Test for bait")]
        private FishTypeBaitScriptable testFishForBait;

        private IBait _currentlyEquipedBait;
        private bool _isBusy = false;
        private float _defaultMaxSliderValue;
        private LayerMask _layerMask;

        private void OnEnable()
        {
            chargeSlider.maxValue = _maxCharge;

            InputActionAsset inputActions = InputSystem.actions;
            InputActionMap playerActionMap = inputActions.FindActionMap("Player");
            InputActionMap uiActionMap = inputActions.FindActionMap("UI");
            playerActionMap.Enable();
            _castAction = playerActionMap.FindAction("Reel");

            if (_currentlyEquipedBait == null) { _currentlyEquipedBait = new NullBait(); }

            _defaultMaxSliderValue = chargeSlider.maxValue;

            _castAction.started += CastInputUsed;
            _castAction.canceled += CastInputReleased;

            GameManager.Instance.GameEvents.OnBaitEquiped += EquipBait;

            GameManager.Instance.GameEvents.OnBecomeOccupied +=
               isCurrentlyEngaged => _isBusy = isCurrentlyEngaged;

            _layerMask = waterLayer | blockFishingLayers;
        }

        private void OnDisable()
        {
            _castAction.started -= CastInputUsed;
            _castAction.canceled -= CastInputReleased;
        }

        private void Update()
        {
            if (_isCharging)
            {
                ChargeLine();
            }
        }

        /// <summary>
        /// Equips the inputed bait 
        /// </summary>
        /// <param name="baitToEquip">The bait item to equip</param>
        public void EquipBait(IBait baitToEquip)
        {
            ItemData baitAsObject = baitToEquip as ItemData;

            if (baitAsObject == null) { return; }

            if (_currentlyEquipedBait != null && _currentlyEquipedBait.GetType() != typeof(NullBait))  { baitAsObject.UnEquipItem(); }

            _currentlyEquipedBait = baitToEquip;
            baitToEquip.SetActiveFishingRod(this);
        }

        /// <summary>
        /// Returns the currently equiped bait
        /// </summary>
        /// <returns>The currently equiped bait</returns>
        public IBait GetCurrentBait()
        {
            return _currentlyEquipedBait;
        }

        /// <summary>
        /// Turns the left and right click controls for reeling off or on based on the paremeter inputed
        /// </summary>
        /// <param name="enable">Enables controls if set to true otherwise disables controls</param>
        public void AreReelingControlsActive(bool enable)
        {
            if (enable) { _allowControls = true; } 
            else { _allowControls = false; }
        }

        /// <summary>
        /// Throws the fishing line at the location shown by the bobber
        /// This is run through an animation event
        /// </summary>
        public void ThrowLine()
        {
            fishingHook.HookIsOut = true;
            fishingHook.SetUpHookTravelToFishSpot(_targetLocation);
        }

        /// <summary>
        /// Sets the currently equiped bait to be a nullbait
        /// </summary>
        public void RemoveBait()
        {
            _currentlyEquipedBait = new NullBait();
        }

        /// <summary>
        /// Hides the fishing charge slider
        /// </summary>
        public void ResetCharge()
        {
            chargeSlider.value = 0;
            _chargePower = 0;
        }

        /// <summary>
        /// Hides teh charger slider
        /// </summary>
        public void SetChargerVisibility(bool isVisible)
        {
            chargeSlider.gameObject.SetActive(isVisible);
        }

        #region Charging_and_throwing_line
        
        /// <summary>
        /// Setsup the variable for a cast being started
        /// </summary>
        private void BeginCharge()
        {
            chargeSlider.gameObject.SetActive(true);
            chargeSlider.maxValue = _defaultMaxSliderValue;
            chargeSlider.value = 0;
            _chargePower = 0;
            _isCharging = true;
            _reverseDirection = false;
            GameManager.Instance.GameEvents.SetPlayerOccupied(true);
            playerController.ToggleMovement(false);
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

            Vector3 modifiedStartAimLocation = new Vector3(_aimStartPoint.x, _aimStartPoint.y + _aimingYOffset, _aimStartPoint.z);

            if (!_reverseDirection)
            {
                _chargePower += Time.deltaTime * chargeScalar;
                _chargePower = Mathf.Clamp(_chargePower, 0, _maxCharge);

                if (_chargePower == _maxCharge) { _reverseDirection = true; }
            }
            else
            {
                _chargePower -= Time.deltaTime * chargeScalar;
                _chargePower = Mathf.Clamp(_chargePower, 0, _maxCharge);
                if (_chargePower == 0) { _reverseDirection = false; }
            }

            chargeSlider.value = _chargePower;
            _aimLoaction = modifiedStartAimLocation + (_aimDirection * _chargePower);
            SetAimPoint(_aimLoaction);
        }

        /// <summary>
        /// Fires a downwards ray from the inputed location, then sets the rodbobber to where the rod hits
        /// </summary>
        /// <param name="locationToUse"> The location that will be raycasted from</param>
        private void SetAimPoint(Vector3 locationToUse)
        {
            RaycastHit hit;
            float maxDistance = fishingRange;
            Vector3 locationWithYOffset = new Vector3(locationToUse.x, locationToUse.y, locationToUse.z);


            if (Physics.Raycast(locationWithYOffset, Vector3.down, out hit, maxDistance, _layerMask))
            {
                rodBobber.transform.position = hit.point;
            }
        }

        /// <summary>
        /// Makes the throw line animation play
        /// </summary>
        private void SetThrowAnimation()
        {
            if (_chargePower >= _chargePowerMinimum)
            {
                GameManager.Instance.GameEvents.SetPlayerOccupied(true);
                _targetLocation = rodBobber.transform.position;
                characterAnimator.SetTrigger("ThrowTrigger");
                characterAnimator.SetBool("isReeling", true);
                reelingMasterScript.DisableControls(true);

                if (_chargePower > _chargePowerGoodMaxValue) { _throwLineResult = ECastingResult.Amazing; }
                else if (_chargePower > _chargePowerAverageMaxValue) { _throwLineResult = ECastingResult.Good; ; }
                else { _throwLineResult = ECastingResult.Average; }
                SetChargeResultData();
            }
            else { _isCharging = false; SetChargerVisibility(false); ResetCharge(); }
        }

        /// <summary>
        /// Runs the game event that shows the status text based on the ECastingResult enum
        /// Also sets the reeling initation scripts wait time based on this value
        /// </summary>
        private void SetChargeResultData()
        {
            switch (_throwLineResult)
            {
                case ECastingResult.Amazing:
                    GameManager.Instance.GameEvents.ShowStatusText("Amazing!", 2f, Color.green);
                    initiationScript.SetFishWaitTimes(_amazingFishMinWaitTime, _amazingFishMaxWaitTime);
                    break;
                case ECastingResult.Good:
                    GameManager.Instance.GameEvents.ShowStatusText("Good!", 2f, Color.green);
                    initiationScript.SetFishWaitTimes((_amazingFishMinWaitTime + 1) * 2, (_amazingFishMaxWaitTime + 1) * 2);
                    break;
                case ECastingResult.Average:
                    GameManager.Instance.GameEvents.ShowStatusText("Average", 2f, Color.yellow);
                    initiationScript.SetFishWaitTimes((_amazingFishMinWaitTime + 1) * 2.5f, (_amazingFishMaxWaitTime + 1) * 2.5f);
                    break;
                default:
                    GameManager.Instance.GameEvents.ShowStatusText("You bugged something this is a default case!", 2f, Color.green);
                    initiationScript.SetFishWaitTimes(_amazingFishMinWaitTime, _amazingFishMaxWaitTime);
                    break;
            }
        }

        private bool CanThrowToLocation()
        {
            RaycastHit hit;
            Vector3 locationPoint = _aimLoaction;
            float maxDistance = fishingRange;

            if (Physics.Raycast(locationPoint, Vector3.down, out hit, maxDistance, _layerMask))
            {
                if (((1 << hit.transform.gameObject.layer) & blockFishingLayers.value) >= 1)
                {
                    ResetCharge();
                    SetChargerVisibility(false);
                    return false;
                }
                return true;
            }
            return false;
        }

        #endregion

        #region MouseControlFunctions
        /// <summary>
        /// Using left click will begin a charge if there is not one ongoing
        /// </summary>
        private void CastInputUsed(InputAction.CallbackContext inputAction)
        {
            if (initiationScript.IsStageOne())
            {
                if (initiationScript.IsFishAtHook()) { initiationScript.FishCaught(); }
                else
                {
                    initiationScript.CancelStageOne();
                    string textToDisplay = "There were no fish attatched!";

                    GameManager.Instance.GameEvents.ShowNotificationText(textToDisplay, 2f, Color.red);
                }
                return;
            }

            if (_isBusy) {  return; }

            if (!_allowControls) { return; }

            if (_isCharging != true) { BeginCharge(); }
        }

        /// <summary>
        /// Releasing left click will reset the current charge and throw the rod
        /// </summary>
        private void CastInputReleased(InputAction.CallbackContext inputAction)
        {
            if (!_allowControls) { return; }

            if (_isCharging == true && CanThrowToLocation())
            {
                SetThrowAnimation();
                _isCharging = false;
            }
            else
            {
                _isCharging = false;
                GameManager.Instance.GameEvents.SetPlayerOccupied(false);
                playerController.ToggleMovement(true);
            }
            
        }

        #endregion

    }
}
