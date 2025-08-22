using UnityEngine;

namespace FishingGame
{
    public class RodRePosition : MonoBehaviour
    {
        [SerializeField]
        private Transform positionToAttatchTo;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = positionToAttatchTo.position;
        }
    }
}
