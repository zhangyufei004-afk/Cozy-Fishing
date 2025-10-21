using FishingGame.FishSystem;
using FishingGame.GameTime;
using FishingGame.SaveGame;
using System.Collections.Generic;
using UnityEngine;

namespace FishingGame.Reeling
{
    /// <summary>
    /// This scriptable object is used to create a custom behaviour for the arrow minigame
    /// It contains al ist of Arrow Entrys and different variables to be setup for how this minigame will playout
    /// </summary>
        [CreateAssetMenu(fileName = "NewArrowWave", menuName = "Fishing Game/Minigames/ArrowWaves")]
        public class ArrowWaveSO : SerializableObject
        {
        [Tooltip("A list of all arrows this will spawn and at what points it will spawn them")]
        public List<ArrowWaveEntry> ArrowEntrys;
        
        [Tooltip("How long until this will activate sudden death")]
        public float SuddenDeathTimer;

        [Tooltip("The max amount of points needed to pass")]
        public float PointsNeededToPass;

        [Tooltip("The default amount of points given per arrow if perfectly timed")]
        public float PointsPerArrow;

        [Tooltip("The speed for the arrows, set to 0 for default speed found on minigame master")]
        public float MinigameSpeed;
        
        internal ArrowWaveSO(int persistentID) : base(persistentID)
        {
        }
        }
}
