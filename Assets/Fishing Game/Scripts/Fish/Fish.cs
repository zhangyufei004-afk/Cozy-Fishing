using System;
using UnityEngine;
using FishingGame.GameTime;
using FishingGame.Inventory;
using FishingGame.Items;
using FishingGame.SaveGame;
using Random = UnityEngine.Random;
using FishingGame.Reeling;

namespace FishingGame.FishSystem
{
    /// <summary>
    /// <para>Represents a runtime fish object.</para>
    /// <para>Stores dynamic data including base info, length, weight, caught time, and location.</para>
    /// </summary>
    public class Fish : Fishable
    {
        private FishScriptableObject _fishBase;

        /// <summary>
        /// Constructor for generating a new fish instance.
        /// </summary>
        /// <param name="newFishBase">Reference to the base fish ScriptableObject</param>
        /// <param name="time">Time of day when caught</param>
        /// <param name="location">Location where the fish was caught</param>
        public Fish(FishScriptableObject newFishBase, ETimeOfDay time, string location)
        {
            _fishBase = newFishBase;

            _fishableName = _fishBase.SpeciesName;
            _fishableWeight = Random.Range(_fishBase.MinMaxWeight.x, _fishBase.MinMaxWeight.y);
            _fishableWeight = Mathf.Round(_fishableWeight * 100) / 100;
            _fishableDifficultyLevel = _fishBase.FishCatchDifficulty;
            _caughtTime = time;
            _caughtLocation = location;
            _fishableType = ECatchableType.Fish;
            _fishableSprite = _fishBase.Texture;
            _arrowMinigameBehaviour = _fishBase.ArrowMiniGameBehaviour;
            _sliderMinigameBehaviour = _fishBase.SliderMiniGameBehaviour;
            _fishableItemType = EItemType.Fish;
        }

        public override void UseItem()
        {
        }

        /// <summary>
        /// Outputs the type as FishScriptableObject and returns this fishes base
        /// </summary>
        /// <param name="dataClassType">The type of scriptableobject this is</param>
        /// <returns>The scriptable object</returns>
        public override SerializableObject GetDataObject(out Type dataClassType)
        {
            dataClassType = typeof(FishScriptableObject);
            return _fishBase;
        }

        /// <summary>
        /// Gets the base ScriptableObject of the fish.
        /// </summary>
        public FishScriptableObject GetFishBase()
        {
            return _fishBase;
        }

        /// <summary>
        /// Gets the time of day the fish was caught.
        /// </summary>
        public ETimeOfDay GetCaughtTime()
        {
            return _caughtTime;
        }

        /// <summary>
        /// Gets the location where the fish was caught.
        /// </summary>
        public string GetCaughtLocation()
        {
            return _caughtLocation;
        }

        /// <summary>
        /// Gets the sell price 
        /// </summary>
        /// <returns>An integer of the sell price</returns>
        public int GetSellPrice()
        {
            return _fishBase.BasePrice;
        }
    }
}