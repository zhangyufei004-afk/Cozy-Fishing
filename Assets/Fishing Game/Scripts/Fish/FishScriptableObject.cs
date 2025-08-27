using FishingGame.SaveGame;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace FishingGame.FishSystem
{
    /// <summary>
    /// <para>
    /// Fish Scriptable Object.
    /// </para>
    /// <para>
    /// Stores Details including: Texture, Min and Max Length, Species name.
    /// </para>
    /// </summary>
    [CreateAssetMenu(fileName = "NewFish", menuName = "Fishing Game/Fish Data")]
    public class FishScriptableObject : SerializableObject
    {
        public Sprite Texture;
        public string Id;
        public Vector2 MinMaxWeight;
        public string SpeciesName;

        public int FishCatchDifficulty;
        public string FishBio;
        public List<EFishingLocation> LocationsFound;
        public string TimeOfDayFound;
        public bool IsInvasive;

        internal FishScriptableObject(int persistentID) : base(persistentID)
        {
        }
    }
}