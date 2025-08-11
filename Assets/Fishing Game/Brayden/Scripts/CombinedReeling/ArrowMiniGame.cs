using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeFishingMechanics
{
    public class ArrowMiniGame : MonoBehaviour, IReelingMinigame
    {
        public void InitializeMiniGame()
        {
            FishingCanvas.SetActive(true);
        }

        public void BeginMiniGame()
        {
            StartQTE();
        }

        [SerializeField]
        [Tooltip("Reference to the ReelingMaster script.")]
        private ReelingMaster ReelingMaster;

        //UI
        public Image[] ArrowSlots;
        public GameObject FishingCanvas;


        //Arrow Sprite
        public Sprite UpSprite;
        public Sprite DownSprite;
        public Sprite LeftSprite;
        public Sprite RightSprite;

        //FishingBar
        public float InputTimeLimit = 3f;

        //QTE
        private List<KeyCode> inputSequence = new List<KeyCode>();
        private int inputIndex = 0;
        private float inputTimer = 0f;
        private bool isQTEActive = false;
        private bool isFishingFinished = false;

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
                        ArrowSlots[inputIndex].color = Color.green;
                        inputIndex++;

                        if (inputIndex >= inputSequence.Count)
                        {

                            WinMiniGame();
                        }
                    }
                    else
                    {
                        ArrowSlots[inputIndex].color = Color.red;
                        Fail("fail");
                    }
                }
            }
        }
        //QTE logic
        public void StartQTE()
        {
            if (isFishingFinished) return;

            isQTEActive = true;
            inputSequence.Clear();
            inputIndex = 0;
            inputTimer = InputTimeLimit;

            for (int i = 0; i < ArrowSlots.Length; i++)
            {
                ArrowSlots[i].color = Color.white;

                int rand = Random.Range(0, 4);
                KeyCode dirKey;

                switch (rand)
                {
                    case 0:
                        dirKey = KeyCode.UpArrow;
                        ArrowSlots[i].sprite = UpSprite;
                        break;
                    case 1:
                        dirKey = KeyCode.DownArrow;
                        ArrowSlots[i].sprite = DownSprite;
                        break;
                    case 2:
                        dirKey = KeyCode.LeftArrow;
                        ArrowSlots[i].sprite = LeftSprite;
                        break;
                    default:
                        dirKey = KeyCode.RightArrow;
                        ArrowSlots[i].sprite = RightSprite;
                        break;
                }

                inputSequence.Add(dirKey);
                ArrowSlots[i].enabled = true;
            }
        }

        void EndQTE()
        {
            isQTEActive = false;
            foreach (var img in ArrowSlots)
            {
                img.enabled = false;
            }
        }

        void Fail(string reason)
        {
            EndQTE();
            LoseMiniGame();
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

        public void WinMiniGame()
        {
            FishingCanvas.SetActive(false);
            EndQTE();
            ReelingMaster.EndCurrentMiniGame(true);
        }

        public void LoseMiniGame()
        {
            FishingCanvas.SetActive(false);
            EndQTE();
            ReelingMaster.EndCurrentMiniGame(false);
        }
    }
}
