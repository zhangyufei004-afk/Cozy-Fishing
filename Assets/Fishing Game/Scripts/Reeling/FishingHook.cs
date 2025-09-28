using FishingGame.FishSystem;
using FishingGame.GameManagement;
using FishingGame.Items;
using FishingGame.SaveGame;
using NUnit.Framework;
using PrototypeFishingMechanics;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Timers;
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

        [Header("Runtime Variables")]

        [SerializeField]
        [Tooltip("The spot where the hook will default back to after casting. NOTE: For current implementation make sure the y is 0 or above.")]
        private Vector3 hookResetSpot;

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

        // Fishing pool is not a list as there should never be two fishing pools colliding at once
        // There is a small chance for multipile fish to collide at once so I have made _collidingFish a list
        private List<GameObject> _collidingFish;
        private FishingPool _collidingPool;
        #endregion

        private void OnEnable()
        {
            _collidingFish = new List<GameObject>();
        }

        private void Update()
        {
            if (_headingToFishSpot)
            {
                transform.position = Vector3.MoveTowards(transform.position, _fishingLocation, castHookSpeed * Time.deltaTime);

                if (Vector3.Distance(transform.position, _fishingLocation) <= rangeFromFishSpot)
                {
                    _headingToFishSpot = false;
                    if (CheckIfColliding())
                    {
                        waterSplash.Play();
                        waterSound.Play();
                        initiationScript.BeginStageOne();
                    }
                    else
                    {
                        PullBackHook();
                    }
                }
            }
            if (_headingBackToHook)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, hookResetSpot, hookReturnSpeed * Time.deltaTime);
                if (transform.localPosition == hookResetSpot)
                {
                    _headingBackToHook = false;
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
            if (other == _collidingPool)
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
            if (HookIsOut == true && _headingToFishSpot == false && reelingMaster.IsFishing == false)
            {
                return true;
            }
            else { return false; }
        }

        /// <summary>
        /// Use this method to attempt to fish from where the fishing rods hook currently is
        /// If valid spot is colliding the hook will return and print a debug log
        /// </summary>
        public void AttempToFishFromCurrentLocation()
        {
            if (CheckIfColliding())
            {
                ReactToFishOnHook();
                ClearCollidingFishAndPool();
            }
            else
            {
                Debug.Log("Hook was not colliding with an object with a fishing pool script");
                PullBackHook();
            }
        }

        /// <summary>
        /// Pulls the fishing hook back and reenables controls
        /// </summary>
        public void PullBackHook()
        {
            GameManager.Instance.GameEvents.SetPlayerOccupied(false);
            SetupHookTravelBack();
            ResetHookSpot();
            reelingMaster.DisableControls(false);
        }

        /// <summary>
        /// Sets variables to allow hook to head to target location
        /// </summary>
        /// <param name="targetLocation">Location to move to</param>
        public void SetUpHookTravelToFishSpot(Vector3 targetLocation)
        {
            Vector3 newPosition = new Vector3(targetLocation.x, targetLocation.y - 1f, targetLocation.z);

            _fishingLocation = newPosition;
            _headingToFishSpot = true;
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

            if (randomPoolFish.GetCatchType() == ECatchableType.Fish)
            {
                Fish fishCaught = (Fish)randomPoolFish;

                reelingMaster.BeginCatchFish(fishCaught, fishModel, fishingPool);
            }
            else if (randomPoolFish.GetCatchType() == ECatchableType.Trash)
            {
                Trash trashCaught = (Trash)randomPoolFish;

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
            if (_collidingFish.Count > 0 || _collidingPool != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
