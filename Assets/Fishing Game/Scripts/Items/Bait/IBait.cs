using FishingGame.FishSystem;
using FishingGame.Reeling;
using UnityEngine;

namespace FishingGame.Items
{
    /// <summary>
    /// This is an interface that all baits should use
    /// Baits must implement all of these but they can choose to simply return null and do nothing
    /// for any and all functions here except for ApplyBait()
    /// </summary>
    public interface IBait
    {
        /// <summary>
        /// This method should take a fishingrod script
        /// Set the bait to have that script as a reference
        /// and then set that rods current bait to be this bait
        /// </summary>
        /// <param name="rodToApplyTo">The rod being applied to</param>
        public void ApplyBait(FishingRod rodToApplyTo);

        /// <summary>
        /// This method should use up a baits charge
        /// </summary>
        public void UseBaitCharge();

        /// <summary>
        /// This method should be run when a bait has used up all of its charges
        /// </summary>
        public void UsedUpBait();

        /// <summary>
        /// This method is run to check if the bait has reached 0 charges
        /// </summary>
        /// <returns>True if at 0 charges otherwise false</returns>
        public bool IsBaitUsedUp();

        /// <summary>
        /// Returns the forced fish type if this bait forces a fish type
        /// </summary>
        /// <returns></returns>
        public FishScriptableObject GetForcedFishType();

        /// <summary>
        /// Contains functionality for how this bait might change a minigame if it does
        /// </summary>
        public void BaitMinigameBehaviour();
    }
}
 