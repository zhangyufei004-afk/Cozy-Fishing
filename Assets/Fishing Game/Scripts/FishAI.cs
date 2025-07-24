using System.Collections.Generic;
using UnityEngine;

namespace PrototypeFishingMechanics
{
    public class FishAI : MonoBehaviour
    {
        #region Constants

        private const int FISH_SPEED = 4;

        #endregion
        
        #region Public Properties

        public bool ShouldPath
        {
            set => _shouldWePath = value;
        }

        #endregion
        
        #region Private Fields

        private List<Transform> _pathWaypoints;
        private bool _shouldWePath = false;
        private int _currentWaypoint = 0;

        #endregion

        private void Awake()
        {
            _pathWaypoints = new List<Transform>();
        }

        public void AddPathWaypoint(Transform waypoint)
        {
            _pathWaypoints.Add(waypoint);
        }

        private void Update()
        {
            // Return if we shouldn't path
            if (!_shouldWePath) return;
            
            transform.position = Vector3.MoveTowards(transform.position, _pathWaypoints[_currentWaypoint].position, FISH_SPEED * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(_pathWaypoints[_currentWaypoint].position - transform.position);
            if (Vector3.Distance(transform.position, _pathWaypoints[_currentWaypoint].position) < 0.1f)
            {
                _currentWaypoint++;
            }
            
            // Return if we haven't reached the end of the path, otherwise continue execution to de-spawn
            if (_currentWaypoint != _pathWaypoints.Count) return;
            _shouldWePath = false;
            Destroy(this.gameObject);
        }
    }
}