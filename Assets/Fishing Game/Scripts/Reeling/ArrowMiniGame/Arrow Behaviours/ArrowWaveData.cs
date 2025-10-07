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


        public ArrowWaveData(ArrowWaveSO arrowWaveBase)
        {
            _waveBase = arrowWaveBase;
            _arrowEntrys = new List<ArrowWaveEntry>();
            _maxTimeForCycle = _waveBase.MaxTimeForCycle;
        }


        public List<ArrowWaveEntry> GetArrowEntrys()
        {
            return _arrowEntrys;
        }

        public int GetMaxTimeForCycle()
        {
            return _maxTimeForCycle;
        }

    }
}
