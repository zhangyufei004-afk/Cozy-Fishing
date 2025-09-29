using FishingGame.SaveGame;
using UnityEngine;

namespace FishingGame
{
    public class ItemScriptable : SerializableObject
    {
        public string ItemName;

        internal ItemScriptable(int persistentID) : base(persistentID)
        {

        }

    }
}
