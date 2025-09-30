using FishingGame.Inventory;
using FishingGame.SaveGame;
using UnityEngine;

namespace FishingGame
{
    /// <summary>
    /// The base scriptable object for an item
    /// Has the variables that are required for all items
    /// Unique types of items like baits can inherit from this
    /// </summary>
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
