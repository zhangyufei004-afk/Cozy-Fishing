using FishingGame.GameTime;
using FishingGame.Inventory;
using FishingGame.SaveGame;
using System;
using UnityEngine;

namespace FishingGame.FishSystem
{
    /// <summary>
    /// Trash is made from trash scriptable objects
    /// They are almost exactly like fish
    /// Implement IStoreable and IFishable
    /// </summary>
    public class Trash : Fishable
    {
        private TrashScriptable _trashBase;

        /// <summary>
        /// Trash is made from trash scriptable objects
        /// They are almost exactly like fish
        /// Implement IStoreable and IFishable
        /// </summary>
        /// <param name="newTrashBase">The scriptable object that data will come from</param>
        /// <param name="time">The time this was caught</param>
        /// <param name="location">The location this was caught at</param>
        public Trash(TrashScriptable newTrashBase, ETimeOfDay time, string location)
        {
            _trashBase = newTrashBase;

            FishableName = _trashBase.Name;
            FishableWeight = UnityEngine.Random.Range(_trashBase.WeightRange.MinWeight, _trashBase.WeightRange.MaxWeight);
            FishableWeight = Mathf.Round(FishableWeight * 100) / 100;
            FishableDifficultyLevel = (int)_trashBase.Difficulty;
            CaughtTime = time;
            CaughtLocation = location;
            FishableType = ECatchableType.Trash;
            FishableSprite = _trashBase.Texture;
            ArrowMinigameBehaviour = _trashBase.ArrowMiniGameBehaviour;
            SliderMinigameBehaviour = _trashBase.SliderMiniGameBehaviour;
            FishableItemType = EItemType.Trash;
        }

        public override Type GetDataObject(out SerializableObject dataClass)
        {
            dataClass = _trashBase;
            return typeof(TrashScriptable);
        }

        public override float GetLength()
        {
            throw new NotImplementedException();
        }

        public override FishableScriptable GetBase()
        {
            return _trashBase;
        }
    }
}
