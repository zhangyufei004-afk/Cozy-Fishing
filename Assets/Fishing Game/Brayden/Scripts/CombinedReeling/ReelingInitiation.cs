using ReelingMasterScript;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PrototypeFishingMechanics
{

    /// <summary>
    /// This class contains the logic that allows the player to begin reeling.
    /// The main functionality for stage 1 of reeling is done here, once that stage is completed
    /// the class then lets ReelingMaster.cs do the rest
    /// </summary>
    public class ReelingInitiation : MonoBehaviour
    {
        #region Public Variables

        public ReelingMaster ReelingMasterScript;
        public bool AllowControls = true;

        #endregion

        #region Private Fields

        private Vector3 _aimPoint;
        private Vector3 _initialPosition;
        private Vector3 _CastDirection;
        private bool _isCharging = false;
        private float _chargePower = 0;
        private float _maxCharge = 10;
        

        [SerializeField]
        private LineRenderer _playerTrajectoryLine;

        [SerializeField]
        private int ChargeScalar;

        [SerializeField]
        private Slider ChargeSlider;

        [SerializeField]
        private GameObject RodEndPoint;

        [SerializeField]
        private GameObject RodBobber;

        [SerializeField]
        private FishingHook FishingHook;

        private Vector3 _fishingHookInitialLocalPosition;


        #endregion

        public void Start()
        {
            ChargeSlider.maxValue = _maxCharge;
        }


        public void Update()
        {
            if (AllowControls == false)
            {
                return;
            }

            if (Input.GetKey(KeyCode.Mouse1))
            {
                RightClickHeld();
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                RightClickUsed();
            }

            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                RightClickReleased();
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                LeftClick();
            }
        }

        /// <summary>
        /// Holding down right click charges the cast line of the rod.
        /// This will update the ui element representing the charge
        /// and also show the tragectory line if the player were to release
        /// </summary>
        private void ChargeLine()
        {
            _chargePower += Time.deltaTime * ChargeScalar;
            ChargeSlider.value = _chargePower;

            
            _initialPosition = RodEndPoint.transform.position;
            _CastDirection = gameObject.transform.forward;
            Vector3 castVelocity = (_CastDirection + _CastDirection).normalized * Mathf.Min(_chargePower, _maxCharge);
            ShowTrajectory(_initialPosition + _CastDirection, castVelocity);
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

                ReelingMasterScript.TEMPSTART();
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
            _fishingHookInitialLocalPosition = FishingHook.gameObject.transform.localPosition;
            FishingHook.CanCatchFish = true;

            Vector3 targetLocation = RodBobber.transform.position;

            // TODO: This is a temporary soloution to make the fishinghook apear where it is needed
            // Eventually this will be turned into a proper cast animation with the hook flying to
            // The target spot
            FishingHook.gameObject.transform.position = targetLocation;

            StartCoroutine(TempTimeForHook());
        }

        private void ReturnHook()
        {
            FishingHook.CanCatchFish = false;

            // TODO: This is a temporary soloution to make the fishinghook appear back where it is needed
            // Eventually this will be turned into a proper return animation with the hook flying
            // to the target spot
            FishingHook.gameObject.transform.localPosition = _fishingHookInitialLocalPosition;
        }

        IEnumerator TempTimeForHook()
        {
            yield return new WaitForSeconds(2);
            ReturnHook();
        }

        /// <summary>
        /// Setsup the variable for a cast being started
        /// </summary>
        private void BeginCharge()
        {
            ChargeSlider.enabled = true;
            ChargeSlider.value = 0;
            _chargePower = 0;
            _isCharging = true;
            _playerTrajectoryLine.enabled = true;
            RodBobber.SetActive(true);
        }

        /// <summary>
        /// Resets the variables when a cast is cancelled or completed
        /// </summary>
        private void ResetCharge()
        {
            ChargeSlider.enabled = false;
            _isCharging = false;
            _playerTrajectoryLine.enabled = false;
            ChargeSlider.value = 0;
            _chargePower = 0;
            RodBobber.SetActive(false);
        }


        private void ShowTrajectory(Vector3 origin, Vector3 speed)
        {
            Vector3[] points = new Vector3[10];
            _playerTrajectoryLine.positionCount = points.Length;
            for (int i = 0; i < points.Length; i++)
            {
                float time = i * 0.1f;
                points[i] = origin + speed * time + 0.5f * Physics.gravity * time * time;

                // This here is currently how I cut down how far the rod goes
                // And try to somewhat accuratley place the bobber
                // TODO: Figure out how to replace this with something better
                if (points[i].y < this.transform.position.y - 2)
                {
                    points[i] = points[i-1];
                }
            }

            RodBobber.transform.position = points[points.Length - 1];

            _playerTrajectoryLine.SetPositions(points);
        }



    }
}
