using FishingGame.GameTime;
using FishingGame.Items;
using FishingGame.Reeling;
using FishingGame.SaveGame;
using System.Collections.Generic;
using UnityEngine;

namespace FishingGame.FishSystem
{
    /// <summary>
    /// The trash scriptable object that is used to create trash items
    /// </summary>
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
        public ArrowWaveSO ArrowMiniGameBehaviour;


        internal TrashScriptable(int persistentID) : base(persistentID)
        {
        }
    }
}
