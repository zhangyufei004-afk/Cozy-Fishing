using UnityEngine;

public class SetActive : MonoBehaviour
{
    public GameObject targetCanvas;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (targetCanvas != null)
            {
                targetCanvas.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }
}
