using FishingGame.SaveGame;
using System;
using UnityEngine;

namespace FishingGame.FishSystem
{
    public enum ECatchableType
    {
        None,
        Fish,
        Trash
    }

    public interface IFishAble
    {
        public ECatchableType GetCatchType();
        public SerializableObject GetDataObject(out Type dataClassType);
    }
}
