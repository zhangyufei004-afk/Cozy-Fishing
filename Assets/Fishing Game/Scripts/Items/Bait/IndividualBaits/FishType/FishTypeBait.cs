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
    public class FishTypeBait : IBait, IStorable
    {
        private FishTypeBaitScriptable _baitBase;
        private EItemType _type = EItemType.RodAttachment;
        

        private FishScriptableObject _fishThisCatches;
        private FishingRod _activeFishingRod;

        public FishTypeBait(FishTypeBaitScriptable baitBase, FishingRod fishingRod)
        {
            _baitBase = baitBase;
            _fishThisCatches = _baitBase.FishAttractType;
            _activeFishingRod = fishingRod;
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

        public float GetWeight()
        {
            throw new NotImplementedException();
        }

        public EItemType GetItemType()
        {
            return _type;
        }

        public SerializableObject GetDataObject(out Type dataClassType)
        {
            throw new NotImplementedException();
        }

        public ItemScriptable GetItemBase()
        {
            throw new NotImplementedException();
        }

        public Sprite GetTexture()
        {
            throw new NotImplementedException();
        }

        public string GetItemName()
        {
            throw new NotImplementedException();
        }
    }
}
