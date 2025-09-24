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
        public int GetCatchDifficulty();

        public String GetName();

        public Sprite GetTexture();

        public float GetWeight();

        public ECatchableType GetCatchType();
        public SerializableObject GetDataObject(out Type dataClassType);
    }
}
