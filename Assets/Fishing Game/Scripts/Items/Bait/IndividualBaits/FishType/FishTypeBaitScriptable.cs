using FishingGame.FishSystem;
using FishingGame.SaveGame;
using UnityEngine;

namespace FishingGame
{
    [CreateAssetMenu(fileName = "NewFishTypeBait", menuName = "Fishing Game/Fish Type Bait")]
    public class FishTypeBaitScriptable : BaitScriptable
    {
        public Sprite Texture;
        public string Id;
        public FishScriptableObject FishAttractType;



        internal FishTypeBaitScriptable(int persistentID) : base(persistentID)
        {
        }
    }
}
