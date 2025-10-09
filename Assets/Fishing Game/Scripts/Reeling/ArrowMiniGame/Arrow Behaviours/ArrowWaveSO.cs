using FishingGame.FishSystem;
using FishingGame.GameTime;
using FishingGame.SaveGame;
using System.Collections.Generic;
using UnityEngine;

namespace FishingGame.Reeling
{
        [CreateAssetMenu(fileName = "NewArrowWave", menuName = "Fishing Game/Minigames/ArrowWaves")]
        public class ArrowWaveSO : SerializableObject
        {
        [Tooltip("A list of all arrows this will spawn and at what points it will spawn them")]
        public List<ArrowWaveEntry> ArrowEntrys;
        
        [Tooltip("How long until this will loop back to the start")]
        public int MaxTimeForCycle;

        [Tooltip("The max amount of points needed to pass")]
        public float PointsNeededToPass;

        [Tooltip("The default amount of points given per arrow if perfectly timed")]
        public float PointsPerArrow;
        
        internal ArrowWaveSO(int persistentID) : base(persistentID)
        {
        }
        }
}
