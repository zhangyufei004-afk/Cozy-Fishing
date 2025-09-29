using FishingGame.GameTime;
using FishingGame.Inventory;
using FishingGame.SaveGame;
using System;
using UnityEngine;

namespace FishingGame.FishSystem
{
    public class Trash : IStorable, IFishAble
    {
        private ECatchableType _catchAbleType;
        private TrashScriptable _trashBase;
        private string _trashName;
        private float _weight;
        private ETimeOfDay _caughtTime;
        private string _caughtLocation;
        private int _trashCatchDifficulty;
        private Sprite _trashTexture;


        public Trash(TrashScriptable newTrashBase, ETimeOfDay time, string location)
        {
            _catchAbleType = ECatchableType.Trash;
            _trashBase = newTrashBase;
            _weight = UnityEngine.Random.Range(_trashBase.MinMaxWeight.x, _trashBase.MinMaxWeight.y);
            // Round weight to 2 decimal places
            _weight = Mathf.Round(_weight * 100) / 100;
            _caughtTime = time;
            _caughtLocation = location;
            _trashCatchDifficulty = _trashBase.TrashDifficulty;
            _trashTexture = _trashBase.Texture;
            _trashName = _trashBase.TrashName;
        }

        /// <summary>
        /// Gets the catchtype of the object
        /// </summary>
        /// <returns>The catchtype of object</returns>
        public ECatchableType GetCatchType()
        {
            return _catchAbleType;
        }

        /// <summary>
        /// Gets the base ScriptableObject of the trash.
        /// </summary>
        /// <returns>The base ScriptableObject of object</returns>
        public TrashScriptable GetTrashBase()
        {
            return _trashBase;
        }

        /// <summary>
        /// Gets the time of day the trash was caught.
        /// </summary>
        public ETimeOfDay GetCaughtTime()
        {
            return _caughtTime;
        }

        /// <summary>
        /// Gets the location where the trash was caught.
        /// </summary>
        public string GetCaughtLocation()
        {
            return _caughtLocation;
        }

        /// <summary>
        /// Gets the weight of the trash.
        /// </summary>
        public float GetWeight()
        {
            return _weight;
        }

        /// <summary>
        /// Returns the type of item this is
        /// </summary>
        /// <returns>The type of item this is</returns>
        public EItemType GetItemType()
        {
            return EItemType.Trash;
        }

        /// <summary>
        /// Returns the type of scriptable object base this is
        /// </summary>
        /// <param name="dataClassType"></param>
        /// <returns>The type of scriptable object</returns>
        public SerializableObject GetDataObject(out Type dataClassType)
        {
            dataClassType = typeof(TrashScriptable);
            return _trashBase;
        }

        /// <summary>
        /// Gets the trash catch difficulty.
        /// </summary>
        public int GetCatchDifficulty()
        {
            return _trashCatchDifficulty;
        }

        /// <summary>
        /// Gets the trash texture
        /// </summary>
        public Sprite GetTexture()
        {
            return _trashTexture;
        }

        /// <summary>
        /// Gets the trash name
        /// </summary>
        public string GetName()
        {
            return _trashName;
        }

        public ItemScriptable GetItemBase()
        {
            throw new NotImplementedException();
        }

        public string GetItemName()
        {
            throw new NotImplementedException();
        }
    }
}
