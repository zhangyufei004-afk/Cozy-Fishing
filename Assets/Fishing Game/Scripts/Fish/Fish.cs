using System;
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

            FishableName = _fishBase.Name;
            FishableWeight = Random.Range(_fishBase.WeightRange.MinWeight, _fishBase.WeightRange.MaxWeight);
            FishableWeight = (float)Math.Round(FishableWeight, 2);
            FishableDifficultyLevel = (int)_fishBase.Difficulty;
            CaughtTime = time;
            CaughtLocation = location;
            FishableType = ECatchableType.Fish;
            FishableSprite = _fishBase.Texture;
            ArrowMinigameBehaviour = _fishBase.ArrowMiniGameBehaviour;
            SliderMinigameBehaviour = _fishBase.SliderMiniGameBehaviour;
            FishableItemType = EItemType.Fish;
        }

        public override void UseItem()
        {
        }

        public override float GetLength()
        {
            throw new NotImplementedException();
        }

        public override FishableScriptable GetBase()
        {
            return _fishBase;
        }

        public override Type GetDataObject(out SerializableObject dataClass)
        {
            dataClass = _fishBase;
            return typeof(FishScriptableObject);
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