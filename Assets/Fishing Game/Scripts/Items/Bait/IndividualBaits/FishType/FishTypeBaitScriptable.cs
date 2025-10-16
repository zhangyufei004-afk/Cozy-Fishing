using FishingGame.FishSystem;
using FishingGame.SaveGame;
using UnityEngine;

namespace FishingGame.Items.Bait
{
    /// <summary>
    /// A bait that attracts a specific type of fish
    /// Inherits from BaitScriptable, this bait has a unique 
    /// FishScriptableObject variable that represents what type of fish
    /// this attracts
    /// </summary>
    [CreateAssetMenu(fileName = "Baititems", menuName = "Fishing Game/Items/Baits/NewSpecificFishBait")]
    public class FishTypeBaitScriptable : BaitScriptable
    {
        public FishScriptableObject FishAttractType;

        internal FishTypeBaitScriptable(int persistentID) : base(persistentID)
        {
        }
    }
}
