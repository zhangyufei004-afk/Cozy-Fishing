using System.Collections;
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
        #region Public Variables

        #endregion

        #region Private Fields

        [SerializeField]
        private GameObject characterParent;

        [SerializeField] 
        private GameObject characterModel;

        [SerializeField]
        private Vector3 aimStartPoint;

        [SerializeField]
        private Vector3 aimDirection;

        private Vector3 _aimHorizontalEndPoint;
        private bool _isCharging = false;
        private float _chargePower = 0;
        private float _maxCharge = 10;

        [SerializeField]
        [Tooltip("Reference to the master reeling script found in the reelingcontainer")]
        private ReelingMaster reelingMasterScript;

        [SerializeField]
        [Tooltip("Reference to the fish camera this is attatched to the hook")]
        private CinemachineCamera fishCamera;

        [SerializeField]
        [Tooltip("Reference to the default camera that is used")]
        private CinemachineCamera mainCamera;

        // TODO: I have currently disabled this visually but still use it to calculate the final location of the hook
        [SerializeField]
        [Tooltip("Reference to a trajectorline attatched to player, used for calculating line casting")]
        private LineRenderer _playerTrajectoryLine;

        [SerializeField]
        [Tooltip("Scales how fast the cast bar is charged when holding right click")]
        private float chargeScalar;

        [SerializeField]
        [Tooltip("Slider for how much charge the cast bar has for reeling")]
        private UnityEngine.UI.Slider chargeSlider;

        // TODO: See if I can just remove this probably not needed
        [SerializeField]
        [Tooltip("End point of the rod")]
        private GameObject rodEndPoint;

        [SerializeField]
        [Tooltip("Rodbobber shows exactly where the line will be cast to, attatched to the fishing rod")]
        private GameObject rodBobber;

        [SerializeField]
        [Tooltip("Contains logic for detecting if a fish or pool is touching the hook, gameobject is attatched to the fishing rod")]
        private FishingHook fishingHook;

        [SerializeField]
        [Tooltip("A temporary field that is currently used to general a generic 3D model for reeling visuailization")]
        private GameObject fishModelPrefab;

        private InputAction _castAction;
        private InputAction _reelAction;


        


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
            if (_reelAction.WasPressedThisFrame())
            {
                LeftClick();
            }

            // TODO: THis can likely be done better and should be changed once this is setup to use new input system
            if (fishingHook.HookIsOut == true)
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
        /// Holding down right click charges the cast line of the rod.
        /// This will update the ui element representing the charge
        /// and also show the tragectory line if the player were to release
        /// </summary>
        private void ChargeLine()
        {
            aimDirection = characterModel.transform.forward;
            aimStartPoint = characterParent.transform.position;

            _chargePower += Time.deltaTime * chargeScalar;
            _chargePower = Mathf.Min(_chargePower, _maxCharge);

            chargeSlider.value = _chargePower;

            Vector3 aimLocation = aimStartPoint + (aimDirection * _chargePower);
            SetAimPoint(aimLocation);
        }

        /// <summary>
        /// When using the left click the line should cast if it is currently being charged
        /// otherwise currently do nothing
        /// If currently charged this will throw the fish line and then reset the charge
        /// </summary>
        private void LeftClick()
        {
            if (_isCharging == true)
            {
                ThrowLine();
                ResetCharge();
            }  
            else if (fishingHook.HookIsOut && reelingMasterScript.IsFishing == false)
            {
                fishingHook.PullBackHook();
            }
        }

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
        /// Releasing right click will reset the current charge
        /// </summary>
        private void RightClickReleased()
        {
            ResetCharge();
        }

        /// <summary>
        /// Throws the fishing line at the location shown by the bobber
        /// </summary>
        private void ThrowLine()
        {
            fishingHook.HookIsOut = true;
            reelingMasterScript.DisableControls(true);

            Vector3 targetLocation = rodBobber.transform.position;
            fishingHook.SetUpHookTravelToFishSpot(targetLocation);
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
        public GameObject CreateAndReturn3DFishModel()
        {
            GameObject fishModel = Instantiate(fishModelPrefab, fishingHook.gameObject.transform.position, Quaternion.Euler(90, 0, 0));
            return fishModel;
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
        /// This is a public function that enables the camera that tracks the fish during reeling
        /// This camera follows a hook gameobject that will always be ontop of the fish
        /// </summary>
        /// <param name="enable">True if you want to enable fish perspective camera, otherwise false</param>
        public void ShouldEnableFishPerspective(bool enable)
        {
            if (enable) 
            {
                fishCamera.gameObject.SetActive(true);
              //  MainCamera.gameObject.SetActive(false);
            }
            else 
            { 
                fishCamera.gameObject.SetActive(false);
              //  MainCamera.gameObject.SetActive(true);
            }
        }

        private void AimFishingRod()
        {

        }


        private void SetAimPoint(Vector3 locationToUse)
        {
            RaycastHit hit;
            float maxDistance = 200f; // Set a maximum distance for the raycast
            LayerMask whatToHit = 1; // Define which layers the raycast should interact with

            if (Physics.Raycast(locationToUse, Vector3.down, out hit, maxDistance, whatToHit))
            {
                // The ray hit something!
                Debug.Log("Hit " + hit.collider.name + " at " + hit.point);
                // You can use hit.point for bullet trails, target indicators, etc.
            }
            else
            {
                // The ray didn't hit anything within the maxDistance
                Debug.Log("Raycast did not hit anything.");
            }

            Debug.DrawRay(locationToUse, Vector3.down * maxDistance, Color.red);
            rodBobber.transform.position = hit.point;
        }


    }
}
