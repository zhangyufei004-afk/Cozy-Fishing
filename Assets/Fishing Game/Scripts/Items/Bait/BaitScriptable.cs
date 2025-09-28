using UnityEngine;

namespace FishingGame
{
    [CreateAssetMenu(fileName = "Baititems", menuName = "Fishing Game/Baits")]
    public class BaitScriptable : ItemScriptable
    {
        internal BaitScriptable(int persistentID) : base(persistentID)
        {

        }
    }
}
