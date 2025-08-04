using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class RealisticFishingGame : MonoBehaviour
{
    public Material RightTriggerMat;
    public Material AButtonMat;
    bool rightTrigger;
    bool aButton;


    public GameObject Cursor;
    Vector3 startPosition;

    public Animator Animator;
    [Range(-0f, 0.5f)]
    public float AnimationSpeed;
    public bool ReverseTarget;
    public float SpeedChangeDelay;

    public TargetCollision TargetCollision;

    public GameObject Fish;
    Vector3 fishStartPosition;

    public float ReelProgress = 0;
    public float ReelSpeed;
    public float ReelDrag;

    public GameObject CatchText;
    public GameObject WinText;
    public GameObject FishSprite;

    void Start()
    {
        //save start pos for later
        startPosition = Cursor.transform.position;
        fishStartPosition = Fish.transform.position;

        StartCoroutine(ReelSpeedChange());
    }

    void Update()
    {

        //cursor
        float cursorX = startPosition.x + (Input.GetAxis("Horizontal") * 1.8f);
        float cursorY = startPosition.y + (Input.GetAxis("Vertical") * 1.8f);
        if(aButton && rightTrigger) Cursor.transform.position = new Vector3(cursorX, cursorY, Cursor.transform.position.z);
        else Cursor.transform.position = startPosition;

        //target
        Animator.speed = AnimationSpeed;
        Animator.SetFloat("direction", ReverseTarget ? -1f : 1f);

        //target collision
        if (TargetCollision.TargetHit == true && ReelProgress < 100)
        {
            ReelProgress += ReelSpeed * Time.deltaTime;

            Gamepad.current.SetMotorSpeeds(0.15f, 0.55f);
        }
        else if (TargetCollision.TargetHit == false && ReelProgress > 0)
        {
            ReelProgress -= ReelDrag * Time.deltaTime;
            Gamepad.current.SetMotorSpeeds(0f, 0f);
        }

        //fish position
        Fish.transform.position = new Vector3(fishStartPosition.x, fishStartPosition.y + (ReelProgress * 0.075f), fishStartPosition.z);

        //controller buttons
        if (Input.GetButton("Fire1"))
        {
            AButtonMat.color = Color.green;
            aButton = true;
        }
        else
        {
            AButtonMat.color = new Color(0.96f, 0.55f, 0.52f);
            aButton = false;
        }

        if (Input.GetAxis("Fire2") == 1)
        {
            RightTriggerMat.color = Color.green;
            rightTrigger = true;
        }
        else
        {
            RightTriggerMat.color = new Color(0.96f, 0.55f, 0.52f);
            rightTrigger = false;
        }

        //victory
        if (ReelProgress >= 95) CatchText.SetActive(true);
        else CatchText.SetActive(false);

        if (Input.GetButtonDown("Fire3") && ReelProgress >= 95)
        {
            WinText.SetActive(true);
            FishSprite.SetActive(false);
            StartCoroutine(Reload());
        }
    }

    IEnumerator ReelSpeedChange()
    {
        while (true) {

            //random speed
            AnimationSpeed = Random.Range(0.15f, 0.5f);

            //1 in 6 change to reverse direction
            if(Random.Range(1,7) == 6)
            {
                ReverseTarget = !ReverseTarget;
            }

            yield return new WaitForSeconds(SpeedChangeDelay);
        }
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("RealisticFishing");
    }
}
