using Unity.VisualScripting;
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

        [SerializeField] private Renderer renderer;

        private ReelingInitiation _initiationScript;

        private Vector3 _goalLocation;
        private bool _atLocation;
        private float _opacity;
        private bool _isLeaving = false;

        void Awake()
        {
            Material runtimeMaterial = new Material(renderer.material);
            renderer.sharedMaterial = runtimeMaterial;
            renderer.sharedMaterial.SetColor("_BaseColor", new Color(0, 0, 0, 0));
        }

        void Update()
        {
            if (!_atLocation)
            {
                transform.position = Vector3.MoveTowards(transform.position, _goalLocation, speed * Time.deltaTime);
                transform.LookAt(_goalLocation);

                _opacity += (Time.deltaTime * 2f) * (_isLeaving ? -1f : 1f);

                _opacity = Mathf.Clamp(_opacity, 0, 1);

                renderer.sharedMaterial.SetColor("_BaseColor", new Color(0, 0, 0, _opacity));

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
        public void SetupVariables(Vector3 goalLocation, ReelingInitiation initiationScript, bool isLeaving = false)
        {
            _goalLocation = goalLocation;
            _initiationScript = initiationScript;
            _atLocation = false;
            _isLeaving = isLeaving;
        }
    }
}
