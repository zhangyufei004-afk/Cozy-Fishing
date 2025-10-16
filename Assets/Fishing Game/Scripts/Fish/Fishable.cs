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
        protected string _fishableName;

        protected float _fishableWeight;
        protected int _fishableDifficultyLevel;
        protected string _caughtLocation;

        protected ETimeOfDay _caughtTime;
        protected ECatchableType _fishableType;
        protected EItemType _fishableItemType;

        protected Sprite _fishableSprite;

        protected ArrowWaveSO _arrowMinigameBehaviour;
        protected SliderSO _sliderMinigameBehaviour;

        /// <summary>
        /// Get the data object for the item. The Data Object is a ScriptableObject, which can be Serailzied. It stores static
        /// non-runtime data. 
        /// </summary>
        /// <param name="dataClassType">Output parameter to give the type of the DataObject, for casting correctness. </param>
        /// <returns>The SerializableObject that the Data is stored in.</returns>
        public abstract SerializableObject GetDataObject(out Type dataClassType);

        /// <summary>
        /// Returns this fishables ArrowMiniGameBehaviour
        /// </summary>
        /// <returns>The arrowminigamebehaviour of this fishable</returns>
        public ArrowWaveSO GetArrowMinigameBehaviour()
        {
            return _arrowMinigameBehaviour;
        }

        /// <summary>
        /// Returns the catch difficulty of this fishable
        /// </summary>
        /// <returns>The difficulty of this fishable</returns>
        public int GetCatchDifficulty()
        {
            return _fishableDifficultyLevel;
        }

        /// <summary>
        /// Returns what type of catchable this is as a ECatchableType
        /// </summary>
        /// <returns>This fishables type as an ECatachableType</returns>
        public ECatchableType GetCatchType()
        {
            return _fishableType;
        }

        
        /// <summary>
        /// Returns this fishables name
        /// </summary>
        /// <returns>Name of this fishable</returns>
        public string GetName()
        {
            return _fishableName;
        }

        /// <summary>
        /// Returns this fishables item type as a EItemType
        /// </summary>
        /// <returns>Returns this fishables item type as a EItemType</returns>
        public EItemType GetItemType()
        {
            return _fishableItemType;
        }

        /// <summary>
        /// Returns this fishables slider minigame behaviour
        /// </summary>
        /// <returns>This fishables slider minigame behaviour</returns>
        public SliderSO GetSliderMinigameBehaviour()
        {
            return _sliderMinigameBehaviour;
        }

        /// <summary>
        /// Returns this fishables sprite
        /// </summary>
        /// <returns>This fishables sprite</returns>
        public Sprite GetTexture()
        {
            return _fishableSprite;
        }

        /// <summary>
        /// Returns this fishables weight
        /// </summary>
        /// <returns>This fishables weight</returns>
        public float GetWeight()
        {
            return _fishableWeight;
        }

        /// <summary>
        /// Functionaility behind using this item
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public abstract void UseItem();
    }
}
