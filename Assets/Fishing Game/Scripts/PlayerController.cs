using UnityEditor.UI;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MovementSpeed;
    public GameObject Body;
    float xInput;
    float zInput;


    void Start()
    {
        
    }

    void Update()
    {
        xInput = Input.GetAxis("Vertical");
        zInput = -Input.GetAxis("Horizontal");

        
    }

    void FixedUpdate()
    {
        //get direction without weight
        Vector3 direction = new Vector3(xInput, 0, zInput).normalized;

        //see wether the weight of x or z movement is higher and store it
        float movementWeight = Mathf.Abs(Input.GetAxis("Vertical")) > Mathf.Abs(Input.GetAxis("Horizontal")) ? Input.GetAxis("Vertical") : Input.GetAxis("Horizontal");

        //movement speed is scaled by the weight of input
        transform.position += direction * (MovementSpeed * Mathf.Abs(movementWeight)) * Time.deltaTime;

        if (xInput != 0 && zInput != 0)
        {
            Body.transform.rotation = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(0, -90f, 0);
        }

    }
}
