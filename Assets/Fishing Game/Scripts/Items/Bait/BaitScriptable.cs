using UnityEngine;

namespace FishingGame
{
    [CreateAssetMenu(fileName = "Baititems", menuName = "Fishing Game/Baits")]
    public class BaitScriptable : ItemScriptable
    {
        public int MaxBaitCharge;
        public int MinBaitCharge;
        internal BaitScriptable(int persistentID) : base(persistentID)
        {

        }
    }
}
