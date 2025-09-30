using FishingGame.Inventory;
using FishingGame.SaveGame;
using UnityEngine;

namespace FishingGame
{
    [CreateAssetMenu(fileName = "Baititems", menuName = "Fishing Game/Items/NewGenericItem")]
    public class ItemScriptable : SerializableObject
    {
        public string ItemName;
        public EItemType ItemType;
        public Sprite Item2DTexture;
        public float ItemWeight;
        public string ItemToolTip;

        internal ItemScriptable(int persistentID) : base(persistentID)
        {

        }

    }
}
