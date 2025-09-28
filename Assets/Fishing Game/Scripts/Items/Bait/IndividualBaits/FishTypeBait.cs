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
        private FishScriptableObject _fishThisCatches;
        private FishingRod _activeFishingRod;

        public FishTypeBait (FishScriptableObject fishTypeToSet)
        {
            _fishThisCatches = fishTypeToSet;
        }

        public void ApplyBait(FishingRod rodToApplyTo)
        {
            _activeFishingRod = rodToApplyTo;
            _activeFishingRod.EquipBait(this);
        }

        public FishScriptableObject GetForcedFishType()
        {
            return _fishThisCatches;
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
