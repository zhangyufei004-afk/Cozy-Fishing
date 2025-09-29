using FishingGame.FishSystem;
using FishingGame.GameTime;
using FishingGame.Inventory;
using FishingGame.Reeling;
using FishingGame.SaveGame;
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace FishingGame.Items
{
    /// <summary>
    /// When created this bait contains a reference to a type of fish it attracts
    /// When using this bait the player will always catch that type of fish unless it is not valid
    /// in the current pools environment
    /// </summary>
    public class FishTypeBait : ItemData, IBait, IStorable
    {
        private FishTypeBaitScriptable _baitBase;
        private EItemType _type = EItemType.RodAttachment;

        private int _baitCharge;
        

        private FishScriptableObject _fishThisCatches;
        private FishingRod _activeFishingRod;

        public FishTypeBait(FishTypeBaitScriptable baitBase, FishingRod fishingRod) : base(baitBase)
        {
            _baitBase = baitBase;
            _fishThisCatches = _baitBase.FishAttractType;
            _activeFishingRod = fishingRod;

            _baitCharge = UnityEngine.Random.Range(baitBase.MinBaitCharge, baitBase.MaxBaitCharge);
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
            if (IsBaitUsedUp()) { UsedUpBait(); return; }
            _baitCharge -= 1;
            Debug.Log(_baitCharge);
        }

        public void UsedUpBait()
        {
            _activeFishingRod.RemoveBait();
        }

        public override void UseItem()
        {
            ApplyBait(_activeFishingRod);
        }


        public bool IsBaitUsedUp()
        {
            if (_baitCharge <= 0) { return true; }
            else { return false; }
        }
    }
}
