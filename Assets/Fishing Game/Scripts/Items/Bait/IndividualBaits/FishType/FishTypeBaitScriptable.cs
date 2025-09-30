using FishingGame.FishSystem;
using FishingGame.SaveGame;
using UnityEngine;

namespace FishingGame
{
    [CreateAssetMenu(fileName = "Baititems", menuName = "Fishing Game/Items/Baits/NewSpecificFishBait")]
    public class FishTypeBaitScriptable : BaitScriptable
    {
        public FishScriptableObject FishAttractType;



        internal FishTypeBaitScriptable(int persistentID) : base(persistentID)
        {
        }
    }
}
