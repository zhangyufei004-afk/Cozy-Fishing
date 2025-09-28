using FishingGame.FishSystem;
using FishingGame.Reeling;
using UnityEngine;

namespace FishingGame.Items
{
    public interface IBait
    {
        public void ApplyBait(FishingRod rodToApplyTo);

        public void UseBaitCharge();

        public FishScriptableObject GetForcedFishType();

        public void BaitMinigameBehaviour();
    }
}
 