using FishingGame.Reeling;
using System.Collections.Generic;
using UnityEngine;

namespace FishingGame.Reeling
{
    public class ArrowWaveData
    {
        private ArrowWaveSO _waveBase;
        private List<ArrowWaveEntry> _arrowEntrys;
        private int _maxTimeForCycle;
        private float _pointPerArrow;
        private float _pointsNeeded;


        public ArrowWaveData(ArrowWaveSO arrowWaveBase)
        {
            _waveBase = arrowWaveBase;
            _arrowEntrys = new List<ArrowWaveEntry>(arrowWaveBase.ArrowEntrys);
            _maxTimeForCycle = _waveBase.MaxTimeForCycle;
            _pointPerArrow = _waveBase.PointsPerArrow;
            _pointsNeeded = _waveBase.PointsNeededToPass;
        }


        public List<ArrowWaveEntry> GetArrowEntrys()
        {
            return _arrowEntrys;
        }

        public int GetMaxTimeForCycle()
        {
            return _maxTimeForCycle;
        }

        public float GetPointPerArrow() { return _pointPerArrow; }

        public float GetMaxPointsNeeded() { return _pointsNeeded; }

    }
}
