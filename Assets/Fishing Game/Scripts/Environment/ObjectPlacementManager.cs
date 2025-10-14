using UnityEngine;

namespace FishingGame
{
    /// <summary>
    /// Manager to mass randomise and settle environment objects.
    /// </summary>
    public class ObjectPlacementManager : MonoBehaviour
    {
        [ContextMenu("Randomise")]
        public void Randomise()
        {
            foreach(Transform child in transform)
            {
                ObjectPlacementRandomiser randomizer = child.gameObject.GetComponent<ObjectPlacementRandomiser>();
                if(randomizer)
                {
                    randomizer.Randomise();
                }
            }
            Debug.Log("All Children Randomised (" + gameObject.name + ")");
        }

        [ContextMenu("Settle")]
        public void Settle()
        {
            foreach(Transform child in transform)
            {
                ObjectPlacementRandomiser randomizer = child.gameObject.GetComponent<ObjectPlacementRandomiser>();
                if(randomizer)
                {
                    randomizer.Settle();
                }
            }
            Debug.Log("All Children Settled (" + gameObject.name + ")");
        }
    }
}
