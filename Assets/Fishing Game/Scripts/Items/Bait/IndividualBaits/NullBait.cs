using FishingGame.FishSystem;
using FishingGame.Items;
using FishingGame.Reeling;
using UnityEngine;

namespace FishingGame
{
    public class NullBait : IBait
    {
        public void ApplyBait(FishingRod rodToApplyTo)
        {
            return;
        }

        public FishScriptableObject GetForcedFishType()
        {
            return null;
        }

        public void BaitMinigameBehaviour()
        {
            return;
        }

        public void UseBaitCharge()
        {
            return;
        }
    }
}
