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

        [SerializeField]
        [Tooltip("A reference to the currently used fishing rod script")]
        private FishingRod fishingRod;

        [Header("ReelingUIElements")]

        [Header("Stageone MiniGame variables")]

        [SerializeField]
        [Tooltip("The prefab of the object that swims up to the reel")]
        private GameObject reelSwimmerPrefab;

        [SerializeField]
        [Tooltip("Max distance the fish can be")]
        private int maxDistance;

        private int _fishDissapearTimeVisual = 1;
        
        private GameObject _fishSwim;
        private FishingPool _currentFishingSpot;

        private float _maxFishWaitTime;
        private float _minFishWaitTime;

        private bool _isStageOne = false;
        private bool _fishAtHook = false;

        private int _stageOneDifficulty = 0;
        private float _catchSecondsToWait;

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

        private bool _isBusy = false;
        
        #endregion

        public void OnEnable()
        {
            GameManager.Instance.GameEvents.OnBecomeOccupied +=
               isCurrentlyEngaged => _isBusy = isCurrentlyEngaged;
        }

        #region Public Methods

        /// <summary>
        /// Sets appropreate values for Stage one of fishing and then runs the required functions
        /// </summary>
        public void BeginStageOne()
        {
            SetupVariables();
            
            if (_currentFishingSpot.IsEmpty()) { StartCoroutine(NoFishTextShow(3f)); }
            else { StartCoroutine(SpawnFishTimer(_catchSecondsToWait)); }
            
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

        /// <summary>
        /// Run once the fish is at the hook transform
        /// This turns the button interactable and starts a timer for how long player has
        /// </summary>
        public void FishAtHook()
        {
            _fishAtHook = true;
            fishingHook.gameObject.GetComponent<Animator>().SetBool("isBobbing", true);

            StartCoroutine(FishCatchTimer(2));
        }

        /// <summary>
        /// Run through a button, this signals stage one was a sucsess
        /// It stops all current timers on this object and tell the master script
        /// to fish
        /// </summary>
        public void FishCaught()
        {
            if (_fishAtHook)
            {
                _fishAtHook = false;
                _isStageOne = false;
                Destroy(_fishSwim);
                StopAllCoroutines();
                fishingHook.AttempToFishFromCurrentLocation();
                fishingRod.ResetCharge();
            }
            else
            {
                CancelStageOne();
            }
        }

        /// <summary>
        /// This is called by the animation event attatched to the player
        /// </summary>
        public void ThrowRodLine()
        {
            fishingRod.ThrowLine();
        }

        /// <summary>
        /// Sets the animators isReeling value based on inputed parameter
        /// </summary>
        /// <param name="isReeling">True if the animation should player</param>
        public void SetIsReelingAnimation(bool isReeling)
        {
            characterAnimator.SetBool("isReeling", isReeling);
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
        /// This can be called to cancel stage one of fishing, hiding the ui and restoring player controls
        /// </summary>
        public void CancelStageOne()
        {
            fishingHook.ClearCollidingFishAndPool();
            StopAllCoroutines();
            fishingHook.gameObject.GetComponent<Animator>().SetBool("isBobbing", false);
            SetIsReelingAnimation(false);
            Destroy(_fishSwim);
            _fishAtHook = false;
            SetIsReelingAnimation(false);
            fishingRod.ResetCharge();

            _isStageOne = false;
            fishingHook.PullBackHook(false);
            GameManager.Instance.GameEvents.SetPlayerOccupied(false);
        }

        /// <summary>
        /// Returns true if reeling is in stage one, otherwise false
        /// </summary>
        /// <returns>True if in stage one otherwise false</returns>
        public bool IsStageOne()
        {
            return _isStageOne;
        }

        /// <summary>
        /// Returns true if fish is at hook otherwise false
        /// </summary>
        /// <returns>True if fish at hook otherwise false</returns>
        public bool IsFishAtHook()
        {
            return _fishAtHook;
        }

        public void SetFishWaitTimes(float minWaitTime, float maxWaitTime)
        {
            _minFishWaitTime = minWaitTime;
            _maxFishWaitTime = maxWaitTime;
        }

        #endregion

        #region StageoneReelingGame

        /// <summary>
        /// Spawns the fish shadow object that will move up to the hook
        /// This is a prefab that should have the StageOneSwimmer class attatched to it
        /// </summary>
        private void SpawnFishShadow()
        {
            _fishSwim = Instantiate(reelSwimmerPrefab, SetFishSpawnLocation(), Quaternion.Euler(90, 0, 0));
            _fishSwim.GetComponent<StageOneSwimmer>().SetupVariables(fishingHook.transform.position, this);
        }

        /// <summary>
        /// Returns a vector3 that can be used for the location the fish swimmer should spawn at
        /// This vector3 is modified with a random int for the z and x axis
        /// </summary>
        /// <returns>A vector3 location</returns>
        private Vector3 SetFishSpawnLocation()
        {
            int zToAdd = UnityEngine.Random.Range(-maxDistance, maxDistance);
            int xToAdd = UnityEngine.Random.Range(-maxDistance, maxDistance);

            Vector3 currentHookLocation = fishingHook.gameObject.transform.position;
            Vector3 trialLocation = new Vector3(currentHookLocation.x += xToAdd, currentHookLocation.y - 1, currentHookLocation.z += zToAdd);

            return trialLocation;
        }

        /// <summary>
        /// This is run when the catch window is not pressed before the fish escapes
        /// It restarts the stageone cycle, makes button uninteractable and tells the fish to swim off
        /// </summary>
        private void FishGotAway()
        {
            _fishAtHook = false;
            fishingHook.gameObject.GetComponent<Animator>().SetBool("isBobbing", false);

            _fishSwim.GetComponent<StageOneSwimmer>().SetupVariables(SetFishSpawnLocation(), this, true);
            StartCoroutine(DespawnFishTimer(_fishDissapearTimeVisual));
        }

        /// <summary>
        /// This timer will spawn a fish shadow once completed
        /// </summary>
        /// <param name="waitTime">The amount of seconds to wait before spawning</param>
        /// <returns>Spawns the fish object</returns>
        private IEnumerator SpawnFishTimer(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            SpawnFishShadow();
        }

        /// <summary>
        /// This timer represents howlong the player has until the fish swims off
        /// After inputed seconds the fish will swim away
        /// </summary>a
        /// <param name="waitTime">The amount of seconds until fish swims away</param>
        /// <returns>The fish swims away</returns>
        private IEnumerator FishCatchTimer(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            if (_isStageOne == true) { FishGotAway(); }
        }

        /// <summary>
        /// This timer will display to the player that the fishing pool is empty of fish or trash after inputed wait time
        /// </summary>
        /// <param name="waitTime">How long until the text should display</param>
        /// <returns>Shows text telling the player that the pool is empty</returns>
        private IEnumerator NoFishTextShow(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            CancelStageOne();
            string textToDisplay = "This pool is empty!";

            GameManager.Instance.GameEvents.ShowNotificationText(textToDisplay, 2f, Color.red);
        }

        /// <summary>
        /// This time despawns the fish after x seconds
        /// It will then start a new spawn fish timer
        /// </summary>
        /// <param name="waitTime">The amount of seconds to wait before despawning</param>
        /// <returns>Despawns the fish and starts timer for new one to spawn</returns>
        private IEnumerator DespawnFishTimer(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            Destroy(_fishSwim);
            StartCoroutine(SpawnFishTimer(_catchSecondsToWait));
        }

        /// <summary>
        /// Sets up variables for stage one
        /// </summary>
        private void SetupVariables()
        {
            GameManager.Instance.GameEvents.SetPlayerOccupied(true);
            _isStageOne = true;
            _fishAtHook = false;
            _currentFishingSpot = fishingHook.GetPoolCurrentlyTouching();
            _stageOneDifficulty = _currentFishingSpot.GetADifficultyInRange();

            _catchSecondsToWait = UnityEngine.Random.Range(_minFishWaitTime, _maxFishWaitTime);
        }

        #endregion
    }
}
