using UnityEngine;

namespace FishingGame.Reeling
{
    /// <summary>
    /// Creates and Manages the Line renderer for displaying the fishing rod line.
    /// </summary>

    [ExecuteInEditMode]
    public class FishingLineRenderer : MonoBehaviour
    {
        // Public Variables

        // Serialized Private Variables
        [SerializeField] private AnimationCurve lineCurve;
        [SerializeField] [Range(2, 100)] private int lineSegmentCount;
        [SerializeField] private Transform startTransform;
        [SerializeField] private Transform endTransform;

        // Private Variables
        private LineRenderer lineRenderer;


        void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();

            lineRenderer.positionCount = lineSegmentCount;
        }

        void Update()
        {
            if (!startTransform || !endTransform) return;
            if (!lineRenderer) lineRenderer = GetComponent<LineRenderer>();

            lineRenderer.positionCount = lineSegmentCount;

            for (int segmentIndex = 0; segmentIndex < lineSegmentCount; segmentIndex++)
            {
                float t = (float) segmentIndex / (float) (lineSegmentCount - 1);
                Vector3 positionModifier = new Vector3(0, lineCurve.Evaluate(t), 0);
                lineRenderer.SetPosition(segmentIndex, Vector3.Lerp(startTransform.position, endTransform.position, t) + positionModifier);
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.DrawSphere(startTransform.position, 0.5f);
            Gizmos.DrawSphere(endTransform.position, 0.5f);
        }
    }
}
