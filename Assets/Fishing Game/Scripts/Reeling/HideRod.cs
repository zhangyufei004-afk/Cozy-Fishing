using FishingGame.Reeling;
using UnityEngine;

namespace FishingGame
{
    /// <summary>
    /// This script is attatched to the fishing rod UI slider
    /// It has one function that is called when the animator completes its animation
    /// </summary>
    public class HideRod : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Reference to the master script")]
        private ReelingMaster reelingMasterScript;

        [SerializeField]
        [Tooltip("Reference to the fishing rod script")]
        private FishingRod fishingRodScript;

        /// <summary>
        /// Run from an animation event
        /// </summary>
        public void RodFallen()
        {
            reelingMasterScript.SetMiniGameProgressVisibility(true);
            fishingRodScript.SetChargerVisibility(false);
            reelingMasterScript.StartMiniGame();
        }
    }
}
