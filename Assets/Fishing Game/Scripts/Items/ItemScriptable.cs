using FishingGame.Inventory;
using FishingGame.SaveGame;
using UnityEngine;

namespace FishingGame
{
    public class ItemScriptable : SerializableObject
    {
        public string ItemName;
        public EItemType ItemType;
        public Sprite Item2DTexture;
        public float ItemWeight;

        internal ItemScriptable(int persistentID) : base(persistentID)
        {

        }

    }
}
