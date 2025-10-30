using UnityEngine;

namespace PrototypeFishingMechanics
{
    public class CameraRotation : MonoBehaviour
    {
        public bool AllowRotation = true;


        private void Update()
        {
            if (AllowRotation)
            {
                float rotationAmount = Input.GetAxis("Mouse X");

                this.transform.Rotate(Vector3.up, rotationAmount * 5);
            }
        }
    }
}