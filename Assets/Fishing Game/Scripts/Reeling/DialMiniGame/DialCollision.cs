using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingGame.Reeling
{
    public class DialCollision : MonoBehaviour
    {
        public bool InTarget = false;
        public bool InTarget2 = false;

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "DialTarget")
            {
                InTarget = true;
            }
            else if (other.tag == "DialTarget2")
                 InTarget2 = true;
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "DialTarget")
            {
                InTarget = false;
            }
            else if (other.tag == "DialTarget2")
                InTarget2 = false;
        }

    }
}
