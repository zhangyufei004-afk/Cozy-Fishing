using FishingGame.FishSystem;
using FishingGame.GameManagement;
using FishingGame.Items;
using FishingGame.SaveGame;
using NUnit.Framework;
using PrototypeFishingMechanics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Timers;
using FishingGame.Items.Bait;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.ProBuilder.MeshOperations;

namespace FishingGame.Reeling
{
    /// <summary>
    /// This class is responsible for containing the logic behind detecting the player has hit a fishing pool or fish
    /// It contains a list of gameobjects for any colliding fish and a singular fishingpool reference for a colliding fishing pool
    /// It updates these fields based on OnTriggerEnters when the fishing rod is being used
    /// It will tell the ReelingInitiation script what type of fish is being caught and its data
    /// </summary>
    public class FishingHook : MonoBehaviour
    {
        private const float MaxRotationDegrees = 200f;
        
        #region Public Variables

        [Tooltip("This is a public variable that should initially be set to false, it is changed by both this script and others based on if the fishin line has been cast or not.")]
        public bool HookIsOut = false;

        #endregion

        #region Private Fields

        [Header("Script references")]

        [SerializeField]
        [Tooltip("Reference to the reeling master script attatched to the reeling container.")]
        private ReelingMaster reelingMaster;

        [SerializeField]
        [Tooltip("Reference to the reeling initation script attatched to the player.")]
        private ReelingInitiation initiationScript;

        [SerializeField]
        [Tooltip("Reference to the fishing rod")]
        private FishingRod fishingRodScript;

        [Header("Runtime Variables")]

        
        private Vector3 _hookResetSpot;

        private bool _headingToFishSpot = false;

        private bool _headingBackToHook = false;

        [SerializeField]
        [Tooltip("Scales the speed the hook returns to the rod.")]
        private float hookReturnSpeed;

        [SerializeField]
        [Tooltip("How long should it take in seconds for the hook to reach its target.")]
        private float castHookSpeed;

        [SerializeField]
        [Tooltip("The water splash special effect")]
        private ParticleSystem waterSplash;

        [SerializeField]
        [Tooltip("The audio component attatched to the hook")]
        private AudioSource waterSound;

        [SerializeField]
        [Tooltip("How far from the fishing spot the hook needs to be")]
        private float rangeFromFishSpot;

        [SerializeField]
        [Tooltip("Curve for the arc")]
        private AnimationCurve curve;

        private Vector3 _fishingLocation;
        private Transform _initialParent;
        private Quaternion _initialRotation;

        // Fishing pool is not a list as there should never be two fishing pools colliding at once
        // There is a small chance for multipile fish to collide at once so I have made _collidingFish a list
        private List<GameObject> _collidingFish;
        private FishingPool _collidingPool;

        private float _velocity;
        private bool _isGrappling;

        #endregion

        private void OnEnable()
        {
            _collidingFish = new List<GameObject>();
            _initialParent = transform.parent;
            _hookResetSpot = transform.localPosition;
            StartCoroutine(SetInitialHookRotation());
        }
        

        private void FixedUpdate()
        {
            if (_headingToFishSpot)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(_fishingLocation.x, transform.position.y, _fishingLocation.z), castHookSpeed * Time.fixedDeltaTime);
                transform.position += new Vector3(0, _velocity, 0);

                // float adjustmentPercent = Vector3.Distance(transform.position, _fishingLocation) / Vector3.Distance(_fishingLocation, _initialParent.transform.position);
                
                _velocity += Mathf.Clamp(Time.deltaTime * 0.06125f * (transform.position.y > _fishingLocation.y ? Physics.gravity.y/3.5f : -(Physics.gravity.y/3.6f)), -1f, 1f);
                
                // _velocity += Time.deltaTime * 0.125f * ((transform.position.y > _fishingLocation.y) ? -1 : 1);
                
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 1, 0)), MaxRotationDegrees * Time.fixedDeltaTime);

                if (Vector3.Distance(transform.position, _fishingLocation) <= rangeFromFishSpot)
                {
                    _headingToFishSpot = false;
                    if (CheckIfColliding())
                    {
                        if (!_isGrappling)
                        {
                            waterSplash.Play();
                            waterSound.Play();
                            reelingMaster.GetCurrentFishingRod().GetCurrentBait().UseBaitCharge();
                            initiationScript.BeginStageOne();
                        }
                    }
                    else
                    {
                        PullBackHook(false);
                    }
                }
            }
            if (_headingBackToHook)
            {
                transform.rotation = _initialRotation;
                Vector3 target = _initialParent.TransformPoint(_hookResetSpot);
                transform.position = Vector3.MoveTowards(transform.position, target, hookReturnSpeed * Time.fixedDeltaTime);
                _velocity = 0;
                if (transform.position == target)
                {
                    _headingBackToHook = false;
                    transform.parent = _initialParent;
                }
            }

        }

        /// <summary>
        /// OnTriggerEnter is used here to add colliding objects to either the currently colliding fish list
        /// or to the colliding pool variable.
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (HookIsOut == false) { return; }

            if (CheckIfPool(other.gameObject))
            {
                _collidingPool = other.gameObject.GetComponent<FishingPool>();
                // _velocity *= 0.3f;
            }
            else if (CheckIfFish(other.gameObject))
            {
                _collidingFish.Add(other.gameObject);
            }
        }

        /// <summary>
        /// OnTriggerExit is used here to remove objects from colliding pool or colliding fish
        /// </summary>
        private void OnTriggerExit(Collider other)
        {
            if (_collidingPool != null && other.gameObject == _collidingPool.gameObject && transform.position.y > _fishingLocation.y)
            {
                _collidingPool = null;
            }
            else if (_collidingFish.Contains(other.gameObject))
            {
                _collidingFish.Remove(other.gameObject);
            }
        }

        /// <summary>
        /// Checks if the hook is able to be pulled back
        /// </summary>
        /// <returns>Returns true if the hook is out and can be returned, else returns false</returns>
        public bool ShouldTravelBack()
        {
            if (HookIsOut && !_headingToFishSpot && !reelingMaster.IsFishing && !_isGrappling)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Use this method to attempt to fish from where the fishing rods hook currently is
        /// If valid spot is colliding the hook will return and print a debug log
        /// </summary>
        public void AttemptToFishFromCurrentLocation()
        {
            if (CheckIfColliding())
            {
                ReactToFishOnHook();
                ClearCollidingFishAndPool();
            }
            else
            {
                Debug.Log("Hook was not colliding with an object with a fishing pool script");
                PullBackHook(false);
            }
        }

        /// <summary>
        /// Pulls the fishing hook back and reenables controls if bool parameter is set to false
        /// </summary>
        /// <param name="areControlsDisabled">True means controls should be disabled</param>
        public void PullBackHook(bool areControlsDisabled)
        {
            GameManager.Instance.GameEvents.SetPlayerOccupied(false);
            SetupHookTravelBack();
            ResetHookSpot();
            reelingMaster.DisableControls(areControlsDisabled);
            initiationScript.SetIsReelingAnimation(false);
            initiationScript.SetIsFishing(false);
            fishingRodScript.ResetCharge(true);
            fishingRodScript.SetChargerVisibility(false);
            _isGrappling = false;
        }

        /// <summary>
        /// Sets variables to allow hook to head to target location
        /// </summary>
        /// <param name="targetLocation">Location to move to</param>
        /// <param name="isGrappling">Bool to indicate whether to ignore terrain collision.
        /// This is useful when grappling as the hook will travel to the grapple point which is on terrain.</param>
        public void SetUpHookTravelToFishSpot(Vector3 targetLocation, bool isGrappling)
        {
            transform.parent = null;
            Vector3 newPosition = new Vector3(targetLocation.x, targetLocation.y - 1f, targetLocation.z);

            _fishingLocation = newPosition;
            _headingToFishSpot = true;
            _isGrappling = isGrappling;
            if (isGrappling)
            {
                _fishingLocation.y += 1f;
            }
        }

        /// <summary>
        /// Clears the colliding object variables from this class
        /// </summary>
        public void ClearCollidingFishAndPool()
        {
            _collidingPool = null;
            _collidingFish.Clear();
        }

        /// <summary>
        /// Returns the pool currently colliding with this hook
        /// </summary>
        /// <returns>Returns the colliding pull unless it is null</returns>
        /// <exception cref="NullReferenceException">Throws a null error if there is no colliding pool</exception>
        public FishingPool GetPoolCurrentlyTouching()
        {
            if (_collidingPool != null)
            {
                return _collidingPool;
            }
            else
            {
                throw new NullReferenceException("Colliding pool == to NULL");
            }
            
        }
        

        /// <summary>
        /// Sets variables to allow hook to head back to its original spot
        /// </summary>
        private void SetupHookTravelBack()
        {
            _headingBackToHook = true;
        }

        /// <summary>
        /// Checks to see if gameobject tag is a fish, returns true if so false otherwise
        /// </summary>
        /// <param name="objectToCheck">Game object to check</param>
        /// <returns>Returns true if the object passed through has the fish tag, otherwise false</returns>
        private bool CheckIfFish(GameObject objectToCheck)
        {
            if (objectToCheck.CompareTag("Fish")) { return true; }
            return false;
        }

        /// <summary>
        /// Checks the object to see if it contains the FishingPool script, if so returns true, else returns false
        /// </summary>
        /// <param name="objectToCheck">Game object to check</param>
        /// <returns>Returns true if the object passed through has the fishing pool script, otherwise false</returns>
        private bool CheckIfPool(GameObject objectToCheck)
        {
            if (objectToCheck.GetComponent<FishingPool>()) { return true; }
            return false;
        }

        /// <summary>
        /// Checks what type of object is attatched to hook,
        /// If it is a fish it runs CaughtFish with a fish overload
        /// IF it is a pool it runs CaughtFish with a pool overload
        /// Else if nothing is caught it does nothing
        /// </summary>
        private void ReactToFishOnHook()
        {
            if (_collidingPool != null)
            {
                CaughtFish(_collidingPool);
            }
        }


        /// <summary>
        /// Gets fish data from the fishingpool and then gets the reelingmaster to begin catch with that data
        /// </summary>
        /// <param name="fishingPool">The pool the hook has found</param>
        private void CaughtFish(FishingPool fishingPool)
        {
            if (fishingPool.IsEmpty())
            {
                reelingMaster.CaughtNothing(fishingPool);
                return;
            }

            IBait baitBeingUsed = reelingMaster.GetCurrentFishingRod().GetCurrentBait();
            IFishAble randomPoolFish = fishingPool.GetFishableCaught(baitBeingUsed);

            GameObject fishModel = initiationScript.CreateAndReturn3DFishModel();
            HookIsOut = false;

            initiationScript.SetIsReelingAnimation(true);
            
            if (randomPoolFish.GetCatchType() == ECatchableType.Fish)
            {
                Fish fishCaught = (Fish)randomPoolFish;

                reelingMaster.BeginCatchFish(fishCaught, fishModel, fishingPool);
            }
            else if (randomPoolFish.GetCatchType() == ECatchableType.Trash)
            {
                Trash trashCaught = randomPoolFish as Trash;

                reelingMaster.BeginCatchTrash(trashCaught, fishModel, fishingPool);
            }


                ClearCollidingFishAndPool();
        }


        /// <summary>
        /// Resets the position of the hook so it is no longer colliding with fishing objects
        /// </summary>
        private void ResetHookSpot()
        {
            // TODO: This need to be physics logic soon
            HookIsOut = false;
            reelingMaster.DisableControls(false);
        }
        
        /// <summary>
        /// Checks if the hook is colliding with a relevant fishing pool, returns true if so otherwise false
        /// </summary>
        /// <returns>True if colliding else false</returns>
        private bool CheckIfColliding()
        {
            if (_collidingFish.Count > 0 || _collidingPool is not null || _isGrappling)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private IEnumerator SetInitialHookRotation()
        {
            yield return new WaitForEndOfFrame();
            _initialRotation = transform.rotation;
        }
    }
}
