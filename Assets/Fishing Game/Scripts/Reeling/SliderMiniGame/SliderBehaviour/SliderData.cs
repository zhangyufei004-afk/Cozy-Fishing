using System.Collections.Generic;
using UnityEngine;

namespace FishingGame.Reeling
{
    /// <summary>
    /// The data created from Slider Scriptable objects
    /// This data contains a list of slider entrys and is used to map out the path and behaviour
    /// of the slider
    /// </summary>
    public class SliderData
    {
        private SliderSO _sliderBase;
        private List<SliderBehaviour> _sliderEntrys;
        private float _startingLocation;
        private float _suddenDeathTimer;
        private float _pointsPerSecond;
        private float _pointsNeeded;

        /// <summary>
        /// Constructor for the slider data requires the slider scriptable object it is based of
        /// </summary>
        /// <param name="sliderBase">The scriptable object to base this data of</param>
        public SliderData(SliderSO sliderBase)
        {
            _sliderBase = sliderBase;
            _sliderEntrys = new List<SliderBehaviour>(sliderBase.SliderBehaviourList);
            _startingLocation = _sliderBase.StartingLocation;
            _suddenDeathTimer = _sliderBase.SuddenDeathTimer;
            _pointsPerSecond = _sliderBase.PointsPerSecond;
            _pointsNeeded = _sliderBase.PointsNeededToPass;
        }

        /// <summary>
        /// Returns all slider entrys in this data
        /// </summary>
        /// <returns>The slider entrys this data has</returns>
        public List<SliderBehaviour> GetSliderBehaviours()
        {
            return _sliderEntrys;
        }

        /// <summary>
        /// Returns the time required for the sudden death timer
        /// </summary>
        /// <returns>Float valuer representing the sudden death timer</returns>
        public float GetSuddenDeathTimer()
        {
            return _suddenDeathTimer;
        }

        /// <summary>
        /// Returns the fishes starting location
        /// </summary>
        /// <returns>The fishes starting location</returns>
        public float GetStartingLocation() { return _startingLocation; }

        /// <summary>
        /// Returns the points per second for this game
        /// </summary>
        /// <returns>The points per second for this game</returns>
        public float GetPointPerSecond() { return _pointsPerSecond; }

        /// <summary>
        /// Returns the max points needed for this game
        /// </summary>
        /// <returns>The max points needed for this game</returns>
        public float GetMaxPointsNeeded() { return _pointsNeeded; }

    }
}
