using FishingGame.FishSystem;
using FishingGame.GameManagement;
using FishingGame.GameTime;
using FishingGame.Inventory;
using FishingGame.Reeling;
using FishingGame.SaveGame;
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace FishingGame.Items.Bait
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

        /// <summary>
        /// When created this bait contains a reference to a type of fish it attracts
        /// When using this bait the player will always catch that type of fish unless it is not valid
        /// in the current pools environment
        /// </summary>
        /// <param name="baitBase">The Scriptable object this is created from</param>
        /// <param name="fishingRod">The players fishing rod</param>
        public FishTypeBait(FishTypeBaitScriptable baitBase) : base(baitBase)
        {
            _baitBase = baitBase;
            _fishThisCatches = _baitBase.FishAttractType;

            _baitCharge = UnityEngine.Random.Range(baitBase.MinBaitCharge, baitBase.MaxBaitCharge);
        }

        /// <summary>
        /// Sets this bait to be the currently used bait by the inputed fishing rod
        /// Updates the currently active fishing rod to be what this is applied to
        /// </summary>
        /// <param name="rodToApplyTo">The fishing rod being applied to</param>
        public void SetActiveFishingRod(FishingRod rodToApplyTo)
        {
            _activeFishingRod = rodToApplyTo;
        }

        /// <summary>
        /// Returns the scriptable object that created this
        /// </summary>
        /// <returns>The base scriptable object for this data</returns>
        public FishScriptableObject GetForcedFishType()
        {
            return _fishThisCatches;
        }

        /// <summary>
        /// This has no function for this type of bait
        /// </summary>
        public void BaitMinigameBehaviour()
        {
            return;
        }

        /// <summary>
        /// Uses up a bait charge
        /// Checks beforehand that the bait has not been used up
        /// </summary>
        public void UseBaitCharge()
        {
            if (IsBaitUsedUp()) { UsedUpBait(); return; }
            _baitCharge -= 1;
        }

        /// <summary>
        /// Run when this baits charges have been used up
        /// This calls an ItemUsedUp event and tells the active fishing rod
        /// to unequip this bait
        /// </summary>
        public void UsedUpBait()
        {
            _activeFishingRod.RemoveBait();
            UnEquipItem();
        }

        /// <summary>
        /// Run when the use item button is clicked
        /// This will apply this bait to the current fishing rod
        /// Or unequip it if the bait is already equiped
        /// This will also call base which simply sets this item as being equiped
        /// </summary>
        public override void UseItem()
        {
            base.UseItem();

            if (!_currentlyEquiped) 
            {
                _activeFishingRod.RemoveBait();
                UnEquipItem();
                return;
            }

            GameManager.Instance.GameEvents.EquipBait(this);
        }

        /// <summary>
        /// Returns the current charge of the bait
        /// </summary>
        /// <returns>The current charge of the bait</returns>
        public override int GetCurrentUseCharge()
        {
            return _baitCharge;
        }

        /// <summary>
        /// Returns true if the bait has been used up
        /// </summary>
        /// <returns>True if bait charges are 0, otherwise false</returns>
        public bool IsBaitUsedUp()
        {
            if (_baitCharge <= 0) { return true; }
            else { return false; }
        }
    }
}
