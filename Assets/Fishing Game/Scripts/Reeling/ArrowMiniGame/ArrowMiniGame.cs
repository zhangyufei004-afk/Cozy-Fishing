using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeFishingMechanics
{
    /// <summary>
    /// ArrowMiniGame uses the IReelingMinigame interface
    /// The arrowminigame involves showing a randomized set of arrows
    /// The player was press the correct arrows in the correct sequence
    /// Pressing the wrong arrow or taking too long causes a loss
    /// </summary>
    public class ArrowMiniGame : MonoBehaviour, IReelingMinigame
    {
        #region Private Properties

        [SerializeField]
        [Tooltip("Reference to the ReelingMaster script.")]
        private ReelingMaster reelingMaster;

        //UI
        [SerializeField]
        [Tooltip("The image gameobjects that will be where each arrow is placed, left most is slot 1.")]
        private Image[] arrowSlots;

        private Vector2[] _arrowSlotPositions;

        [SerializeField]
        [Tooltip("The parent object in the ui ")]
        private GameObject fishingCanvas;

        //Arrow Sprites
        [SerializeField]
        [Tooltip("Up arrow sprite")]
        private Sprite upSprite;
        [SerializeField]
        [Tooltip("Down arrow sprite")]
        private Sprite downSprite;
        [SerializeField]
        [Tooltip("Left arrow sprite")]
        private Sprite leftSprite;
        [SerializeField]
        [Tooltip("Right arrow sprite")]
        private Sprite rightSprite;

        

        //FishingBar
        [SerializeField]
        private float inputTimeLimit = 3f;

        //QTE
        private List<KeyCode> _inputSequence = new List<KeyCode>();
        private int _inputIndex = 0;
        private float _inputTimer = 0f;
        private bool _isQTEActive = false;
        private bool _isFishingFinished = false;

        #endregion

        private void Start()
        {
            _arrowSlotPositions = new Vector2[arrowSlots.Length];

            for (int i = 0; i <arrowSlots.Length; i++)
            {
                _arrowSlotPositions[i] = arrowSlots[i].gameObject.transform.localPosition;
            }
        }


        /// <summary>
        /// Setsup any properties/variables needed for the minigame
        /// Parameter fishCatchDifficulty can be used to modify difficulty of minigame
        /// </summary>
        /// <param name="fishCatchDifficulty">The difficulty of the fish caught</param>
        public void InitializeMiniGame(int fishCatchDifficulty)
        {
            fishingCanvas.SetActive(true);
            for (int i = 0; i < arrowSlots.Length; i++)
            {
                arrowSlots[i].gameObject.transform.localPosition = _arrowSlotPositions[i];
            }
        }

        /// <summary>
        /// Begins the minigame
        /// </summary>
        public void BeginMiniGame()
        {
            StartQTE();
        }


        void Update()
        {
            if (!_isQTEActive || _isFishingFinished) return;

            _inputTimer -= Time.deltaTime;

            if (_inputTimer <= 0f)
            {
                Fail();
                return;
            }

            if (Input.anyKeyDown)
            {
                if (CheckKeyPressed(out KeyCode pressedKey))
                {
                    if (pressedKey == _inputSequence[_inputIndex])
                    {
                        FlickAnimation(arrowSlots[_inputIndex]);
                        //arrowSlots[_inputIndex].gameObject.SetActive(false);
                        _inputIndex++;

                        if (_inputIndex >= _inputSequence.Count)
                        {

                            WinMiniGame();
                        }
                    }
                    else
                    {
                        arrowSlots[_inputIndex].color = Color.red;
                        Fail();
                    }
                }
            }
        }

        /// <summary>
        /// Starts the arrow quick time event
        /// Sets up a randomized arrow order and ties them to their respective key
        /// </summary>
        private void StartQTE()
        {
            if (_isFishingFinished) return;

            _isQTEActive = true;
            _inputSequence.Clear();
            _inputIndex = 0;
            _inputTimer = inputTimeLimit;

            for (int i = 0; i < arrowSlots.Length; i++)
            {
                Animator animatorToUse;
                animatorToUse = arrowSlots[i].GameObject().GetComponent<Animator>();

                arrowSlots[i].gameObject.SetActive(true);
                arrowSlots[i].color = Color.white;
                animatorToUse.SetInteger("ArrowSlot", i + 1);

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

                _inputSequence.Add(dirKey);
                arrowSlots[i].enabled = true;
            }
        }

        private void FlickAnimation(Image arrowSprite)
        {
            Animator animatorToUse;
            animatorToUse = arrowSprite.GameObject().GetComponent<Animator>();

            animatorToUse.SetBool("IsActive", true);
        }


        /// <summary>
        /// Disables the quick time event, cleaing the arrowslots
        /// </summary>
        private void EndQTE()
        {
            _isQTEActive = false;
            foreach (var img in arrowSlots)
            {
                Animator animatorToUse;
                animatorToUse = img.GameObject().GetComponent<Animator>();
                animatorToUse.SetBool("IsActive", false);
                img.enabled = false;
            }
        }

        /// <summary>
        /// Calls the end QTE event function and the lose minigame
        /// </summary>
        private void Fail()
        {
            EndQTE();
            LoseMiniGame();
        }

        /// <summary>
        /// Detects if a key has been pressed for any of the arrows
        /// </summary>
        /// /// <param name="key">The arrowkey pressed</param>
        private bool CheckKeyPressed(out KeyCode key)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) { key = KeyCode.UpArrow; return true; }
            if (Input.GetKeyDown(KeyCode.DownArrow)) { key = KeyCode.DownArrow; return true; }
            if (Input.GetKeyDown(KeyCode.LeftArrow)) { key = KeyCode.LeftArrow; return true; }
            if (Input.GetKeyDown(KeyCode.RightArrow)) { key = KeyCode.RightArrow; return true; }
            key = KeyCode.None;
            return false;
        }

        /// <summary>
        /// Wins the minigame, disables canvas and calls the end event functions
        /// </summary>
        public void WinMiniGame()
        {
            fishingCanvas.SetActive(false);
            EndQTE();
            reelingMaster.EndCurrentMiniGame(true);
        }

        /// <summary>
        /// Loses the minigame, disables canvas and calls the end event functions
        /// </summary>
        public void LoseMiniGame()
        {
            fishingCanvas.SetActive(false);
            EndQTE();
            reelingMaster.EndCurrentMiniGame(false);
        }
    }
}
