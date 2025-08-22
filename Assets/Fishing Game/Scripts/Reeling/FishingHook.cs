using FishingGame.FishSystem;
using NUnit.Framework;
using PrototypeFishingMechanics;
using System.Collections.Generic;
using System.Net;
using System.Threading;
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

        public Vector3 hookGoal;

        #endregion

        #region Private Fields

        [SerializeField]
        [Tooltip("Reference to the reeling master script attatched to the reeling container.")]
        private ReelingMaster reelingMaster;

        [SerializeField]
        [Tooltip("Reference to the reeling initation script attatched to the player.")]
        private ReelingInitiation initiationScript;

        [SerializeField]
        [Tooltip("The spot where the hook will default back to after casting. NOTE: For current implementation make sure the y is 0 or above.")]
        private Vector3 hookResetSpot;

        [SerializeField]
        private Animator rodAnimator;

        private bool _headingToFishSpot = false;

        private bool _headingBackToHook = false;

        [SerializeField]
        [Tooltip("Scales the speed the hook returns to the rod.")]
        private float hookReturnSpeed;

        [SerializeField]
        [Tooltip("Scales the speed the hook heads to the target.")]
        private float castHookSpeed;

        [SerializeField]
        [Tooltip("The water splash special effect")]
        private ParticleSystem waterSplash;

        [SerializeField]
        [Tooltip("The audio component attatched to the hook")]
        private AudioSource waterSound;

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
                if (transform.position == _fishingLocation)
                {
                    _headingToFishSpot = false;
                    waterSplash.Play();
                    waterSound.Play();
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
        /// This public function is called when the right click is used while the hook has been cast
        /// It runs the ResetHookSpot function, runs the logic to react to whatever is currently caught
        /// and then clears the colliding object variables from this class.
        /// </summary>
        public void PullBackHook()
        {
            if (_collidingFish.Count > 0 || _collidingPool != null)
            {
                ReactToFishOnHook();

                ClearCollidingFishAndPool();
            }
            else
            {
                SetupHookTravelBack();
                ResetHookSpot();
                reelingMaster.DisableOrEnableControls(true);
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
        /// Sets variables to allow hook to head to target location
        /// </summary>
        /// /// <param name="targetLocation">Location to move to</param>
        public void SetUpHookTravelToFishSpot(Vector3 targetLocation)
        {
            _fishingLocation = targetLocation;
            _headingToFishSpot = true;
        }

        /// <summary>
        /// Clears the colliding object variables from this class
        /// </summary>
        private void ClearCollidingFishAndPool()
        {
            _collidingPool = null;
            _collidingFish.Clear();
        }

        /// <summary>
        /// Checks to see if gameobject tag is a fish, returns true if so false otherwise
        /// </summary>
        /// <param name="objectToCheck">Game object to check</param>
        private bool CheckIfFish(GameObject objectToCheck)
        {
            if (objectToCheck.CompareTag("Fish")) { return true; }
            return false;
        }

        /// <summary>
        /// Checks the object to see if it contains the FishingPool script, if so returns true, else returns false
        /// </summary>
        /// <param name="objectToCheck">Game object to check</param>
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
            if (_collidingFish.Count > 0)
            {
                // TODO: Make this work with individual fish once individual fish have been setup
                /*
                CaughtFish(_collidingFish[0]);
                */
            }
            else if (_collidingPool != null)
            {
                CaughtFish(_collidingPool);
            }
        }

        /// <summary>
        /// Gets the data needed from the fish, begins the reelingmaster minigame script
        /// </summary>
        /// /// <param name="fishCaught">The fish that has been caught</param>
        private void CaughtFish(Fish fishCaught)
        {
            GameObject fishModel = initiationScript.CreateAndReturn3DFishModel();
            reelingMaster.BeginCatch(fishCaught, fishModel);
        }

        /// <summary>
        /// Gets fish data from the fishingpool and then gets the reelingmaster to begin catch with that data
        /// </summary>
        /// <param name="fishingPool">The pool the hook has found</param>
        private void CaughtFish(FishingPool fishingPool)
        {
            Fish randomPoolFish = fishingPool.DetermineFishCaught();
            GameObject fishModel = initiationScript.CreateAndReturn3DFishModel();

            reelingMaster.BeginCatch(randomPoolFish, fishModel, fishingPool);
        }


        /// <summary>
        /// Resets the position of the hook so it is no longer colliding with fishing objects
        /// </summary>
        public void ResetHookSpot()
        {
            // TODO: This need to be physics logic soon
          //  gameObject.transform.localPosition = hookResetSpot;
            HookIsOut = false;
            reelingMaster.DisableOrEnableControls(true);
        }
    }
}
