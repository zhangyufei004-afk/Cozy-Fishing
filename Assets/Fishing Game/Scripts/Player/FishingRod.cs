using FishingGame.GameManagement;
using FishingGame.Items;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingGame.Reeling
{
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

        private bool _reverseDirection = false;
        private bool _allowControls = true;
        private bool _isCharging = false;
        private float _chargePower = 0;
        private float _maxCharge = 8;

        private Vector3 _targetLocation;
        private Vector3 _aimStartPoint;
        private Vector3 _aimDirection;

        private InputAction _castAction;

        [Header("ReelingUIElements")]

        [SerializeField]
        [Tooltip("Scales how fast the cast bar is charged when holding right click")]
        private float chargeScalar;

        [SerializeField]
        [Tooltip("Slider for how much charge the cast bar has for reeling")]
        private UnityEngine.UI.Slider chargeSlider;


        [Header("Misc")]

        [SerializeField]
        [Tooltip("The animator attatched to the player")]
        private Animator characterAnimator;

        private IBait _currentlyEquipedBait;
        private bool _isBusy = false;

        private void OnEnable()
        {
            chargeSlider.maxValue = _maxCharge;

            InputActionAsset inputActions = InputSystem.actions;
            InputActionMap playerActionMap = inputActions.FindActionMap("Player");
            InputActionMap uiActionMap = inputActions.FindActionMap("UI");
            playerActionMap.Enable();
            _castAction = playerActionMap.FindAction("Reel");

            GameManager.Instance.GameEvents.OnBecomeOccupied +=
               isCurrentlyEngaged => _isBusy = isCurrentlyEngaged;
        }

        private void Update()
        {
            if (_isBusy == true)
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


        /// <summary>
        /// Equips the inputed bait 
        /// </summary>
        /// <param name="baitToEquip">The bait item to equip</param>
        public void EquipBait(IBait baitToEquip)
        {
            _currentlyEquipedBait = baitToEquip;
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
        /// Throws the fishing line at the location shown by the bobber
        /// This is run through an animation event
        /// </summary>
        public void ThrowLine()
        {
            fishingHook.HookIsOut = true;

            fishingHook.SetUpHookTravelToFishSpot(_targetLocation);
        }

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

    }
}
