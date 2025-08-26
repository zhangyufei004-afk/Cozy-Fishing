using System;
using UnityEngine;
using FishingGame.GameTime;
using FishingGame.Inventory;
using FishingGame.SaveGame;
using Random = UnityEngine.Random;

namespace FishingGame.FishSystem
{
    /// <summary>
    /// <para>Represents a runtime fish object.</para>
    /// <para>Stores dynamic data including base info, length, weight, caught time, and location.</para>
    /// </summary>
    public class Fish : IStorable
    {
        private FishScriptableObject _fishBase;
        private float _length;
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
            _fishBase = newFishBase;
            _length = Random.Range(_fishBase.MinMaxLength.x, _fishBase.MinMaxLength.y);
            _weight = CalculateWeight(_length); // Simple formula based on length
            _caughtTime = time;
            _caughtLocation = location;
            _fishCatchDifficulty = _fishBase.FishCatchDifficulty;
            _fishTexture = _fishBase.Texture;
        }

        /// <summary>
        /// Calculates fish weight based on its length.
        /// </summary>
        /// <param name="length">Length of the fish</param>
        /// <returns>Weight in kilograms</returns>
        private float CalculateWeight(float length)
        {
            return length * 0.2f + Random.Range(-0.1f, 0.1f); // Example formula
        }

        /// <summary>
        /// Gets the base ScriptableObject of the fish.
        /// </summary>
        public FishScriptableObject GetFishBase()
        {
            return _fishBase;
        }

        /// <summary>
        /// Gets the length of the fish.
        /// </summary>
        public float GetLength()
        {
            return _length;
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

        public EItemType GetItemType()
        {
            return EItemType.Fish;
        }

        public SerializableObject GetDataObject(out Type dataClassType)
        {
            dataClassType = typeof(FishScriptableObject);
            return _fishBase;
        }

        /// <summary>
        /// Gets the fish catch difficulty.
        /// </summary>
        public int GetFishCatchDifficulty()
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
    }
}