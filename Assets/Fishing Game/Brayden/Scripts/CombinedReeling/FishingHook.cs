using NUnit.Framework;
using PrototypeFishingMechanics;
using System.Collections.Generic;
using UnityEngine;

namespace ReelingMasterScript
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
        #endregion


        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.GetComponent<FishScritableObject>() && CanCatchFish)
            {
                CanCatchFish = false;
                
                reelingMaster.BeginCatch(collision.gameObject.GetComponent<FishScritableObject>());
            }
        }
    }
}
