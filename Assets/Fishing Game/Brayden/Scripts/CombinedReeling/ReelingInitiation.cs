using ReelingMasterScript;
using System.Collections;
using Unity.Cinemachine;
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
        public CinemachineCamera FishCamera;
        public CinemachineCamera MainCamera;

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

        [SerializeField]
        private GameObject TestFishObject;


        #endregion

        public void Start()
        {
            ChargeSlider.maxValue = _maxCharge;
            FishCamera.gameObject.SetActive(false);
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

            GameObject testFish = Instantiate(TestFishObject, FishingHook.gameObject.transform.position, Quaternion.Euler(90, 0, 0));
            ShouldEnableFishPerspective(true);
            ReelingMasterScript.TEMPSTART(testFish);


            // StartCoroutine(TempTimeForHook());
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
            ChargeSlider.gameObject.SetActive(true);
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
            ChargeSlider.gameObject.SetActive(false);
            _isCharging = false;
            _playerTrajectoryLine.enabled = false;
            ChargeSlider.value = 0;
            _chargePower = 0;
            RodBobber.SetActive(false);
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
                FishCamera.gameObject.SetActive(true);
              //  MainCamera.gameObject.SetActive(false);
            }
            else 
            { 
                FishCamera.gameObject.SetActive(false);
              //  MainCamera.gameObject.SetActive(true);
            }
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
