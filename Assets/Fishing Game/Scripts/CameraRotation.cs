using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace PrototypeFishingMechanics
{
    public class CameraRotation : MonoBehaviour
    {
        private void Update()
        {
            float rotationAmount = Input.GetAxis("Mouse X");
            
            this.transform.Rotate(Vector3.up, rotationAmount*5);
        }
    }
}