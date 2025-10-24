using FishingGame.FishSystem;
using FishingGame.GameTime;
using FishingGame.Inventory;
using FishingGame.Items;
using FishingGame.Reeling;
using FishingGame.SaveGame;
using System;
using UnityEngine;

namespace FishingGame.FishSystem
{
    /// <summary>
    /// A base class that all fishable objects should inherit from
    /// </summary>
    public abstract class Fishable : IFishAble, IStorable
    {
        protected string FishableName;

        protected float FishableWeight;
        protected int FishableDifficultyLevel;
        protected string CaughtLocation;

        protected ETimeOfDay CaughtTime;
        protected ECatchableType FishableType;
        protected EItemType FishableItemType;

        protected Sprite FishableSprite;

        protected ArrowWaveSO ArrowMinigameBehaviour;
        protected SliderSO SliderMinigameBehaviour;

        
        public abstract Type GetDataObject(out SerializableObject dataClass);

        /// <summary>
        /// Returns this fishables ArrowMiniGameBehaviour
        /// </summary>
        /// <returns>The arrowminigamebehaviour of this fishable</returns>
        public ArrowWaveSO GetArrowMinigameBehaviour()
        {
            return ArrowMinigameBehaviour;
        }

        /// <summary>
        /// Returns the catch difficulty of this fishable
        /// </summary>
        /// <returns>The difficulty of this fishable</returns>
        public int GetCatchDifficulty()
        {
            return FishableDifficultyLevel;
        }

        /// <summary>
        /// Returns what type of catchable this is as a ECatchableType
        /// </summary>
        /// <returns>This fishables type as an ECatachableType</returns>
        public ECatchableType GetCatchType()
        {
            return FishableType;
        }
        
        /// <summary>
        /// Returns this fishables name
        /// </summary>
        /// <returns>Name of this fishable</returns>
        public string GetName()
        {
            return FishableName;
        }

        /// <summary>
        /// Returns this fishables item type as a EItemType
        /// </summary>
        /// <returns>Returns this fishables item type as a EItemType</returns>
        public EItemType GetItemType()
        {
            return FishableItemType;
        }

        /// <summary>
        /// Returns this fishables slider minigame behaviour
        /// </summary>
        /// <returns>This fishables slider minigame behaviour</returns>
        public SliderSO GetSliderMinigameBehaviour()
        {
            return SliderMinigameBehaviour;
        }

        /// <summary>
        /// Returns this fishables sprite
        /// </summary>
        /// <returns>This fishables sprite</returns>
        public Sprite GetTexture()
        {
            return FishableSprite;
        }

        /// <summary>
        /// Returns this fishables weight
        /// </summary>
        /// <returns>This fishables weight</returns>
        public float GetWeight()
        {
            return FishableWeight;
        }

        /// <summary>
        /// Functionaility behind using this item
        /// </summary>
        public virtual void UseItem()
        {
        }

        /// <summary>
        /// Gets the location where the fishable was caught.
        /// </summary>
        public string GetCaughtLocation()
        {
            return CaughtLocation;
        }

        /// <summary>
        /// Gets the time of day the fish was caught.
        /// </summary>
        public ETimeOfDay GetCaughtTime()
        {
            return CaughtTime;
        }

        public abstract float GetLength();

        public abstract FishableScriptable GetBase();

    }
}
