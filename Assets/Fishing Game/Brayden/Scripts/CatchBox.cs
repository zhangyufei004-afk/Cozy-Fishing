using System.Collections.Generic;
using UnityEngine;

namespace Brayden
{
    public class CatchBox : MonoBehaviour
    {
        // A list of all gameobjects touching this, used to check for the fish ui icon
        public List<GameObject> currentlyTouchingList;

        // Reference to the fish ui icon
        public GameObject fishIcon;

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject == fishIcon)
            {
                currentlyTouchingList.Add(collision.gameObject);
            }
        }

        public void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject == fishIcon)
            {
                currentlyTouchingList.Remove(collision.gameObject);
            }
        }
    }
}

