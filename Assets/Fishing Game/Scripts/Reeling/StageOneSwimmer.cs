using UnityEngine;

namespace FishingGame.Reeling
{
    /// <summary>
    /// This class is used for the stage one prefab that swims up to the hook
    /// It contains the movement logic for that object
    /// </summary>
    public class StageOneSwimmer : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Speed of the object")]
        private int speed;

        [SerializeField]
        [Tooltip("How far away from goal you can be")]
        private float acceptanceDistance;

        private ReelingInitiation _initiationScript;

        private Vector3 _goalLocation;
        private bool _atLocation;

        void Update()
        {
            if (!_atLocation)
            {
                transform.position = Vector3.MoveTowards(transform.position, _goalLocation, speed * Time.deltaTime);

                if (Vector3.Distance(transform.position, _goalLocation) <= acceptanceDistance)
                {
                    _atLocation = true;
                    _initiationScript.FishAtHook();
                }
            }
        }

        /// <summary>
        /// Sets up required variables for this object to move to
        /// </summary>
        /// <param name="goalLocation">The location this object should move to</param>
        /// <param name="initiationScript">The initiation script this can reference for when at hook</param>
        public void SetupVariables(Vector3 goalLocation, ReelingInitiation initiationScript)
        {
            _goalLocation = goalLocation;
            _initiationScript = initiationScript;
            _atLocation= false;
        }
    }
}
