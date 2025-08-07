using UnityEngine;

public class QTEStarter : MonoBehaviour
{
    public GameObject canvasA;
    public GameObject canvasB;
    public QTEController qteController;

    private bool hasStarted = false;

    void Update()
    {
        if (!hasStarted && Input.GetKeyDown(KeyCode.Space))
        {
            hasStarted = true;
            canvasA.SetActive(false);
            canvasB.SetActive(true);
            qteController.StartQTE();
        }
    }
}
