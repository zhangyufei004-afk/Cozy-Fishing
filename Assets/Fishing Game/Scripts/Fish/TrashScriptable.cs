using FishingGame.GameTime;
using FishingGame.SaveGame;
using System.Collections.Generic;
using UnityEngine;

namespace FishingGame.FishSystem
{
    [CreateAssetMenu(fileName = "NewTrash", menuName = "Fishing Game/Trash Data")]
    public class TrashScriptable : SerializableObject
    {
        public Sprite Texture;
        public string Id;
        public Vector2 MinMaxWeight;
        public string TrashName;

        public int TrashDifficulty;
        public string TrashBio;
        public List<EFishingLocation> LocationsFound;
        public List<ETimeOfDay> TimesFound;


        internal TrashScriptable(int persistentID) : base(persistentID)
        {
        }
    }
}
