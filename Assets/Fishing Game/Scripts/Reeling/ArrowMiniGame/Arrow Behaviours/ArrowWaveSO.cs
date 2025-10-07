using FishingGame.FishSystem;
using FishingGame.GameTime;
using FishingGame.SaveGame;
using System.Collections.Generic;
using UnityEngine;

namespace FishingGame.Reeling
{
    public class ArrowWaveSO
    {
        [CreateAssetMenu(fileName = "NewArrowWave", menuName = "Fishing Game/ArrowWaves")]
        public class ArrowWaveSo : SerializableObject
        {
            [Tooltip("A list of all arrows this will spawn and at what points it will spawn them")]
            public List<ArrowWaveEntry> ArrowEntrys;

            internal ArrowWaveSo(int persistentID) : base(persistentID)
            {
            }
        }








    }
}
