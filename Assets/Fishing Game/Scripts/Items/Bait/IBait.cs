using FishingGame.FishSystem;
using FishingGame.Reeling;
using UnityEngine;

namespace FishingGame.Items
{
    public interface IBait
    {
        public void ApplyBait(FishingRod rodToApplyTo);

        public void UseBaitCharge();

        public void UsedUpBait();

        public bool IsBaitUsedUp();

        public FishScriptableObject GetForcedFishType();

        public void BaitMinigameBehaviour();
    }
}
 