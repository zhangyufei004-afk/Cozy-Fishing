using FishingGame.FishSystem;
using FishingGame.Inventory;
using FishingGame.Items;
using FishingGame.Reeling;
using UnityEngine;

namespace FishingGame
{
    public class NullBait : IBait
    {
        FishingRod activeFishingRod;


        public void ApplyBait(FishingRod rodToApplyTo)
        {
            activeFishingRod = rodToApplyTo;
            activeFishingRod.EquipBait(this);
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
