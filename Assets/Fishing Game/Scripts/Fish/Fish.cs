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
    /// <para>Represents a runtime fish object.</para>
    /// <para>Stores dynamic data including base info, length, weight, caught time, and location.</para>
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
            // Round weight to 2 decimal places
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
        /// Gets the weight of the fish.
        /// </summary>
        public float GetWeight()
        {
            return _weight;
        }

        /// <summary>
        /// Returns a EItemType.Fish value
        /// </summary>
        /// <returns>Returns a EItemType.Fish value</returns>
        public EItemType GetItemType()
        {
            return EItemType.Fish;
        }

        /// <summary>
        /// Get the data object for the item. The Data Object is a ScriptableObject, which can be Serailzied. It stores static
        /// non-runtime data. 
        /// </summary>
        /// <param name="dataClassType">Output parameter to give the type of the DataObject, for casting correctness. </param>
        /// <returns>The SerializableObject that the Data is stored in.</returns>
        public SerializableObject GetDataObject(out Type dataClassType)
        {
            dataClassType = typeof(FishScriptableObject);
            return _fishBase;
        }

        /// <summary>
        /// Gets the fish catch difficulty.
        /// </summary>
        public int GetCatchDifficulty()
        {
            return _fishCatchDifficulty;
        }

        /// <summary>
        /// Gets the fish texture
        /// </summary>
        public Sprite GetTexture()
        {
            return _fishTexture;
        }

        /// <summary>
        /// Gets the species name
        /// </summary>
        public string GetName()
        {
            return _speciesName;
        }

        /// <summary>
        /// Gets the catchable type of this object
        /// </summary>
        /// <returns>The catchable type of this object</returns>
        public ECatchableType GetCatchType()
        {
            return catchAbleType;
        }

        /// <summary>
        /// Returns the item base
        /// Currently not used for fish
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">Not used for Fish</exception>
        public ItemScriptable GetItemBase()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the item name
        /// Current not used for fish
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">Not used for Fish</exception>
        public string GetItemName()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Uses the item
        /// Not used for Fish
        /// </summary>
        /// <exception cref="NotImplementedException">Not used for Fish</exception>
        public void UseItem()
        {
            throw new NotImplementedException();
        }
    }
}