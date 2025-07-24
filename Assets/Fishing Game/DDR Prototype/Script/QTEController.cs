using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class QTEController : MonoBehaviour
{
//UI
    public Image[] arrowSlots;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI fishingResultText;


//Arrow Sprite
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

//FishingBar
    public Slider fishingProgressBar;
    public float progressIncrement = 0.1f;
    public float inputTimeLimit = 3f;

//QTE
    private List<KeyCode> inputSequence = new List<KeyCode>();
    private int inputIndex = 0;
    private float inputTimer = 0f;
    private bool isQTEActive = false;
    private bool isFishingFinished = false;

    void Start()
    {
        resultText.text = "";
        fishingResultText.text = "";
        fishingProgressBar.minValue = 0f;
        fishingProgressBar.maxValue = 1f;
        fishingProgressBar.value = 0.5f;

        StartQTE();
    }

    void Update()
    {
        if (!isQTEActive || isFishingFinished) return;

        inputTimer -= Time.deltaTime;

        if (inputTimer <= 0f)
        {
            Fail("overtime");
            return;
        }

        if (Input.anyKeyDown)
        {
            if (CheckKeyPressed(out KeyCode pressedKey))
            {
                if (pressedKey == inputSequence[inputIndex])
                {
                    arrowSlots[inputIndex].color = Color.green;
                    inputIndex++;

                    if (inputIndex >= inputSequence.Count)
                    {
                        resultText.text = "success";
                        IncreaseProgress();
                        EndQTE();
                        if (!isFishingFinished) Invoke(nameof(StartQTE), 1.5f);
                    }
                }
                else
                {
                    arrowSlots[inputIndex].color = Color.red;
                    Fail("fail");
                }
            }
        }
    }
//QTE logic
    public void StartQTE()
    {
        if (isFishingFinished) return;

        resultText.text = "";
        isQTEActive = true;
        inputSequence.Clear();
        inputIndex = 0;
        inputTimer = inputTimeLimit;

        for (int i = 0; i < arrowSlots.Length; i++)
        {
            arrowSlots[i].color = Color.white;

            int rand = Random.Range(0, 4);
            KeyCode dirKey;

            switch (rand)
            {
                case 0:
                    dirKey = KeyCode.UpArrow;
                    arrowSlots[i].sprite = upSprite;
                    break;
                case 1:
                    dirKey = KeyCode.DownArrow;
                    arrowSlots[i].sprite = downSprite;
                    break;
                case 2:
                    dirKey = KeyCode.LeftArrow;
                    arrowSlots[i].sprite = leftSprite;
                    break;
                default:
                    dirKey = KeyCode.RightArrow;
                    arrowSlots[i].sprite = rightSprite;
                    break;
            }

            inputSequence.Add(dirKey);
            arrowSlots[i].enabled = true;
        }
    }

    void EndQTE()
    {
        isQTEActive = false;
        foreach (var img in arrowSlots)
        {
            img.enabled = false;
        }
    }

    void Fail(string reason)
    {
        resultText.text = reason;
        DecreaseProgress();
        EndQTE();
        if (!isFishingFinished) Invoke(nameof(StartQTE), 1.5f);
    }
//ProgressBar
    void IncreaseProgress()
    {
        fishingProgressBar.value += progressIncrement;
        CheckProgress();
    }

    void DecreaseProgress()
    {
        fishingProgressBar.value -= progressIncrement;
        CheckProgress();
    }

    void CheckProgress()
    {
        if (fishingProgressBar.value >= fishingProgressBar.maxValue)
        {
            isFishingFinished = true;
            fishingResultText.text = "Successful fishing!";
            resultText.text = "";
        }
        else if (fishingProgressBar.value <= fishingProgressBar.minValue)
        {
            isFishingFinished = true;
            fishingResultText.text = "Fishing failed!";
            resultText.text = "";
        }
    }

    bool CheckKeyPressed(out KeyCode key)
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) { key = KeyCode.UpArrow; return true; }
        if (Input.GetKeyDown(KeyCode.DownArrow)) { key = KeyCode.DownArrow; return true; }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { key = KeyCode.LeftArrow; return true; }
        if (Input.GetKeyDown(KeyCode.RightArrow)) { key = KeyCode.RightArrow; return true; }
        key = KeyCode.None;
        return false;
    }
}
