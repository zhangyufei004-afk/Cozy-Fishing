using UnityEngine;

namespace FishingGame
{
    /// <summary>
    /// Script to randomise and settle environment objects.
    /// </summary>
    public class ObjectPlacementRandomiser : MonoBehaviour
    {
        [SerializeField] private Vector2 sizeRanges = new Vector2(0.6f, 1.0f);

        [ContextMenu("Randomise")]
        public void Randomise()
        {
            transform.rotation = Quaternion.Euler(new Vector3(Random.Range(-5, 5), Random.Range(0, 360), Random.Range(-5, 5)));

            float scale = Random.Range(sizeRanges.x, sizeRanges.y);
            transform.localScale = new Vector3(scale, scale, scale);
        }

        [ContextMenu("Settle")]
        public void Settle()
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 3))
            {
                transform.position = hit.point;
            }
            else
            {
                Debug.LogWarning("Object has failed to Settle. (" + gameObject.name + ")");
            }
        }
    }
}
