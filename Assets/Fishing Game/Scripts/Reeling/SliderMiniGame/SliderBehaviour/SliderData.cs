using System.Collections.Generic;
using UnityEngine;

namespace FishingGame.Reeling
{
    public class SliderData
    {
        private SliderSO _sliderBase;
        private List<SliderBehaviour> _sliderEntrys;
        private float _startingLocation;
        private float _suddenDeathTimer;
        private float _pointsPerSecond;
        private float _pointsNeeded;

        public SliderData(SliderSO sliderBase)
        {
            _sliderBase = sliderBase;
            _sliderEntrys = new List<SliderBehaviour>(sliderBase.SliderBehaviourList);
            _startingLocation = _sliderBase.StartingLocation;
            _suddenDeathTimer = _sliderBase.SuddenDeathTimer;
            _pointsPerSecond = _sliderBase.PointsPerSecond;
            _pointsNeeded = _sliderBase.PointsNeededToPass;
        }

        public List<SliderBehaviour> GetSliderBehaviours()
        {
            return _sliderEntrys;
        }

        public float GetMaxTimeForCycle()
        {
            int index = _sliderEntrys.Count - 1;
            return _sliderEntrys[index].GetTimeToStart();
        }

        public float GetSuddenDeathTimer()
        {
            return _suddenDeathTimer;
        }

        public float GetStartingLocation() { return _startingLocation; }

        public float GetPointPerSecond() { return _pointsPerSecond; }

        public float GetMaxPointsNeeded() { return _pointsNeeded; }

    }
}
