using FishingGame.Reeling;
using System.Collections.Generic;
using UnityEngine;

namespace FishingGame.Reeling
{
    public class ArrowWaveData
    {
        private ArrowWaveSO _waveBase;
        private List<ArrowWaveEntry> _arrowEntrys;
        private float _suddenDeathTimer;
        private float _pointPerArrow;
        private float _pointsNeeded;
        private float _speedOfGame;


        public ArrowWaveData(ArrowWaveSO arrowWaveBase)
        {
            _waveBase = arrowWaveBase;
            _arrowEntrys = new List<ArrowWaveEntry>(arrowWaveBase.ArrowEntrys);
            _suddenDeathTimer = _waveBase.SuddenDeathTimer;
            _pointPerArrow = _waveBase.PointsPerArrow;
            _pointsNeeded = _waveBase.PointsNeededToPass;
            _speedOfGame = _waveBase.MinigameSpeed;
        }


        public List<ArrowWaveEntry> GetArrowEntrys()
        {
            return _arrowEntrys;
        }

        public float GetMaxTimeForCycle()
        {
            int index = _arrowEntrys.Count - 1;
            return _arrowEntrys[index].GetTimeToSpawn();
        }

        public float GetSuddenDeathTimer()
        {
            return _suddenDeathTimer;
        }

        public float GetPointPerArrow() { return _pointPerArrow; }

        public float GetMaxPointsNeeded() { return _pointsNeeded; }

        public float GetSpeedOfGame() { return _speedOfGame; }

    }
}
