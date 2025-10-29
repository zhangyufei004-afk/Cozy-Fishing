using UnityEngine;

namespace FishingGame
{
    public class GroundCheck : MonoBehaviour
    {
        public bool OnWood = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Wood Surface")
            {
                OnWood = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Wood Surface")
            {
                OnWood = false;
            }
        }
    }
}
