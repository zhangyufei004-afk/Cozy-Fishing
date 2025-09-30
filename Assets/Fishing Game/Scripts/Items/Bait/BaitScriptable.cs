using UnityEngine;

namespace FishingGame
{
    [CreateAssetMenu(fileName = "Baititems", menuName = "Fishing Game/Items/Baits/NewGenericBait")]
    public class BaitScriptable : ItemScriptable
    {
        public int MaxBaitCharge;
        public int MinBaitCharge;
        internal BaitScriptable(int persistentID) : base(persistentID)
        {

        }
    }
}
