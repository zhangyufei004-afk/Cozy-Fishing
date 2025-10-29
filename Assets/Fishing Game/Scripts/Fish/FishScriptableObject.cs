using FishingGame.GameTime;
using FishingGame.Reeling;
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
    public class FishScriptableObject : FishableScriptable
    {
        public bool IsInvasive;
        public int BasePrice = 10;
        private string _id;
        private static int _idNumber;

        public string ID => Name;

        public FishScriptableObject() : this(0)
        {
            // _id = Name + $"{_idNumber}";
            // _idNumber++;
        }
        
        internal FishScriptableObject(int persistentID) : base(persistentID)
        {
            _id = Name + $"{_idNumber}";
            _idNumber++;
        }
    }
}