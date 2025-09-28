using FishingGame.FishSystem;
using FishingGame.Reeling;
using UnityEngine;

namespace FishingGame.Items
{
    /// <summary>
    /// When created this bait contains a reference to a type of fish it attracts
    /// When using this bait the player will always catch that type of fish unless it is not valid
    /// in the current pools environment
    /// </summary>
    public class FishTypeBait : IBait
    {
        FishScriptableObject fishThisCatches;
        FishingRod activeFishingRod;

        public void ApplyBait(FishingRod rodToApplyTo)
        {
            throw new System.NotImplementedException();
        }

        public FishScriptableObject GetForcedFishType()
        {
            return fishThisCatches;
        }

        public void BaitMinigameBehaviour()
        {
            throw new System.NotImplementedException();
        }

        public void UseBaitCharge()
        {
            throw new System.NotImplementedException();
        }

        
    }
}
