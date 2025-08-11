using NUnit.Framework;
using PrototypeFishingMechanics;
using System.Collections.Generic;
using UnityEngine;

namespace ReelingMasterScript
{
    public class FishingBobber : MonoBehaviour
    {
        #region Public Variables

        public bool CanCatchFish = false;

        #endregion

        #region Private Fields

        [SerializeField]
        private ReelingMaster ReelingMaster;

        

        #endregion


        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.GetComponent<FishScritableObject>() && CanCatchFish)
            {
                CanCatchFish = false;
                ReelingMaster.BeginCatch(collision.gameObject.GetComponent<FishScritableObject>());
            }
        }
    }
}
