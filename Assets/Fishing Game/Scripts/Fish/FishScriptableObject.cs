using FishingGame.SaveGame;
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
        public Vector2 MinMaxLength;
        public string SpeciesName;

        internal FishScriptableObject(int persistentID) : base(persistentID)
        {
        }
    }
}