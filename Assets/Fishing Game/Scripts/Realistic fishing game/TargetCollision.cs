using UnityEngine;

public class TargetCollision : MonoBehaviour
{
    public bool TargetHit = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Cursor")
        {
            TargetHit = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Cursor")
        {
            TargetHit = false;
        }
    }
}
