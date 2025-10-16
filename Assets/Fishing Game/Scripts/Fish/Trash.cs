using FishingGame.GameTime;
using FishingGame.Inventory;
using FishingGame.SaveGame;
using System;
using FishingGame.Items;
using UnityEngine;
using FishingGame.Reeling;

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

            _fishableName = _trashBase.TrashName;
            _fishableWeight = UnityEngine.Random.Range(_trashBase.MinMaxWeight.x, _trashBase.MinMaxWeight.y);
            _fishableWeight = Mathf.Round(_fishableWeight * 100) / 100;
            _fishableDifficultyLevel = _trashBase.TrashDifficulty;
            _caughtTime = time;
            _caughtLocation = location;
            _fishableType = ECatchableType.Trash;
            _fishableSprite = _trashBase.Texture;
            _arrowMinigameBehaviour = _trashBase.ArrowMiniGameBehaviour;
            _sliderMinigameBehaviour = _trashBase.SliderMiniGameBehaviour;
            _fishableItemType = EItemType.Trash;
        }

        public override SerializableObject GetDataObject(out Type dataClassType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the base ScriptableObject of the fish.
        /// </summary>
        public TrashScriptable GetTrashBase()
        {
            return _trashBase;
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

        public override void UseItem()
        {
            throw new NotImplementedException();
        }
    }
}
