using UnityEngine;

namespace FishingGame
{
    [CreateAssetMenu(fileName = "Baititems", menuName = "Fishing Game/Baits")]
    public class BaitScriptable : ItemScriptable
    {
        public Sprite Texture;
        public string Id;
        public string BaitName;
        internal BaitScriptable(int persistentID) : base(persistentID)
        {

        }
    }
}
