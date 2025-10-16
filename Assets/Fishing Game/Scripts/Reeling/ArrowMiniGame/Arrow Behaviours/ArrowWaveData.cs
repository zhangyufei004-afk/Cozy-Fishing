using FishingGame.Reeling;
using System.Collections.Generic;
using UnityEngine;

namespace FishingGame.Reeling
{
    /// <summary>
    /// This data is constructed from a ArrowWaveSO, it contains a list of arrow entrys which
    /// map out how the minigame will play out
    /// </summary>
    public class ArrowWaveData
    {
        private ArrowWaveSO _waveBase;
        private List<ArrowWaveEntry> _arrowEntrys;
        private float _suddenDeathTimer;
        private float _pointPerArrow;
        private float _pointsNeeded;
        private float _speedOfGame;


        /// <summary>
        /// The constructor for this arrowwavebase requires the ArrowWaveSO it is based of
        /// </summary>
        /// <param name="arrowWaveBase">The scriptable object this data is based of</param>
        public ArrowWaveData(ArrowWaveSO arrowWaveBase)
        {
            _waveBase = arrowWaveBase;
            _arrowEntrys = new List<ArrowWaveEntry>(arrowWaveBase.ArrowEntrys);
            _suddenDeathTimer = _waveBase.SuddenDeathTimer;
            _pointPerArrow = _waveBase.PointsPerArrow;
            _pointsNeeded = _waveBase.PointsNeededToPass;
            _speedOfGame = _waveBase.MinigameSpeed;
        }

        /// <summary>
        /// Returns a list of Arrow Entrys
        /// </summary>
        /// <returns>The list of arrow entrys this data has</returns>
        public List<ArrowWaveEntry> GetArrowEntrys()
        {
            return _arrowEntrys;
        }

        /// <summary>
        /// Returns the final indexes time to spawn its arrow
        /// </summary>
        /// <returns>The timer value for the final arrowentry</returns>
        public float GetMaxTimeForCycle()
        {
            int index = _arrowEntrys.Count - 1;
            return _arrowEntrys[index].GetTimeToSpawn();
        }

        /// <summary>
        /// Returns the sudden death timer
        /// </summary>
        /// <returns>The sudden death timer for this data</returns>
        public float GetSuddenDeathTimer()
        {
            return _suddenDeathTimer;
        }

        /// <summary>
        /// Returns the points per arrow for this data
        /// </summary>
        /// <returns>The points per arrow</returns>
        public float GetPointPerArrow() { return _pointPerArrow; }

        /// <summary>
        /// Returns the max points needed for this data
        /// </summary>
        /// <returns>The max points needed</returns>
        public float GetMaxPointsNeeded() { return _pointsNeeded; }

        /// <summary>
        /// Returns the speed of the game
        /// </summary>
        /// <returns>A float value representing the speed of the arrows for this game</returns>
        public float GetSpeedOfGame() { return _speedOfGame; }

    }
}
