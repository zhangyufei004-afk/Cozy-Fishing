using System;
using UnityEngine;
using FishingGame.GameTime;
using FishingGame.Inventory;
using FishingGame.Items;
using FishingGame.SaveGame;
using Random = UnityEngine.Random;

namespace FishingGame.FishSystem
{
    /// <summary>
    /// Represents a runtime fish object.
    /// Stores dynamic data including base info, weight, caught time, and location.
    /// </summary>
    public class Fish : IStorable, IFishAble
    {
        private ECatchableType catchAbleType;
        private FishScriptableObject _fishBase;
        private string _speciesName;
        private float _weight;
        private ETimeOfDay _caughtTime;
        private string _caughtLocation;
        private int _fishCatchDifficulty;
        private Sprite _fishTexture;

        /// <summary>
        /// Constructor for generating a new fish instance.
        /// </summary>
        /// <param name="newFishBase">Reference to the base fish ScriptableObject</param>
        /// <param name="time">Time of day when caught</param>
        /// <param name="location">Location where the fish was caught</param>
        public Fish(FishScriptableObject newFishBase, ETimeOfDay time, string location)
        {
            catchAbleType = ECatchableType.Fish;
            _fishBase = newFishBase;
            _weight = Random.Range(_fishBase.MinMaxWeight.x, _fishBase.MinMaxWeight.y);
            _weight = Mathf.Round(_weight * 100) / 100;
            _caughtTime = time;
            _caughtLocation = location;
            _fishCatchDifficulty = _fishBase.FishCatchDifficulty;
            _fishTexture = _fishBase.Texture;
            _speciesName = _fishBase.SpeciesName;
        }

        /// <summary>
        /// Gets the base ScriptableObject of the fish.
        /// </summary>
        public FishScriptableObject GetFishBase() => _fishBase;

        /// <summary>
        /// Gets the time of day the fish was caught.
        /// </summary>
        public ETimeOfDay GetCaughtTime() => _caughtTime;

        /// <summary>
        /// Gets the location where the fish was caught.
        /// </summary>
        public string GetCaughtLocation() => _caughtLocation;

        /// <summary>
        /// Gets the weight of the fish.
        /// </summary>
        public float GetWeight() => _weight;

        /// <summary>
        /// Returns EItemType.Fish.
        /// </summary>
        public EItemType GetItemType() => EItemType.Fish;

        /// <summary>
        /// Get the data object for the item. The Data Object is a ScriptableObject, which can be serialized. It stores static
        /// non-runtime data.
        /// </summary>
        public SerializableObject GetDataObject(out Type dataClassType)
        {
            dataClassType = typeof(FishScriptableObject);
            return _fishBase;
        }

        /// <summary>
        /// Gets the fish catch difficulty.
        /// </summary>
        public int GetCatchDifficulty() => _fishCatchDifficulty;

        /// <summary>
        /// Gets the fish texture.
        /// </summary>
        public Sprite GetTexture() => _fishTexture;

        /// <summary>
        /// Gets the species name.
        /// </summary>
        public string GetName() => _speciesName;

        /// <summary>
        /// Gets the catchable type of this object.
        /// </summary>
        public ECatchableType GetCatchType() => catchAbleType;

        /// <summary>
        /// Returns the selling price of the fish.
        /// Example: price based on weight and difficulty.
        /// </summary>
        public int GetSellPrice()
        {
            return Mathf.CeilToInt(_weight * 10 + _fishCatchDifficulty * 2);
        }

        /// <summary>
        /// Not used for Fish.
        /// </summary>
        public ItemScriptable GetItemBase() => throw new NotImplementedException();

        /// <summary>
        /// Not used for Fish.
        /// </summary>
        public string GetItemName() => throw new NotImplementedException();

        /// <summary>
        /// Uses the item
        /// Not used for Fish
        /// </summary>
        public void UseItem() => throw new NotImplementedException();
    }
}

