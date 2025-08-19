using NUnit.Framework;
using PrototypeFishingMechanics;
using System.Collections.Generic;
using UnityEngine;

namespace FishingGame.Reeling
{
    /// <summary>
    /// This class uses an OnCollisionEnter function to check determine when the players
    /// fishing line has hit a fish
    /// </summary>
    public class FishingHook : MonoBehaviour
    {
        #region Public Variables

        public bool CanCatchFish = false;

        #endregion

        #region Private Fields

        [SerializeField]
        private ReelingMaster reelingMaster;

        [SerializeField]
        private ReelingInitiation initiationScript;

        [SerializeField]
        [Tooltip("The spot where the hook will default back to after casting. NOTE: For current implementation make sure the y is 0 or above.")]
        private Vector3 hookResetSpot;
        #endregion

        public void OnTriggerEnter(Collider other)
        {
            if (CanCatchFish == false) { return; }

            if (other.gameObject.GetComponent<FishingPool>())
            {
                GameObject fishModel = initiationScript.CreateAndReturn3DFishModel();
                CanCatchFish = false;
                FishingPool fishingPoolScript = other.gameObject.GetComponent<FishingPool>();

                reelingMaster.BeginCatch(fishingPoolScript.DetermineFishCaught(), fishModel, fishingPoolScript);
            }
        }

        /// <summary>
        /// Resets the position of the hook so it is no longer colliding with fishing objects
        /// </summary>
        public void ResetHookSpot()
        {
            gameObject.transform.position = hookResetSpot;
        }
    }
}
