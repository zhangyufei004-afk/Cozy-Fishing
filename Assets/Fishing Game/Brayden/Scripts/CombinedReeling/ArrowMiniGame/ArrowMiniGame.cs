using System.Collections.Generic;
using TMPro;
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
        #region Public Variables

        #endregion

        #region Private Properties

        [SerializeField]
        [Tooltip("Reference to the ReelingMaster script.")]
        private ReelingMaster reelingMaster;

        //UI
        [SerializeField]
        private Image[] arrowSlots;

        [SerializeField]
        [Tooltip("The parent object in the ui ")]
        private GameObject fishingCanvas;


        //Arrow Sprites
        [SerializeField]
        private Sprite upSprite;
        [SerializeField]
        private Sprite downSprite;
        [SerializeField]
        private Sprite leftSprite;
        [SerializeField]
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

        /// <summary>
        /// Setsup any properties/variables needed for the minigame
        /// Parameter fishCatchDifficulty can be used to modify difficulty of minigame
        /// </summary>
        /// <param name="fishCatchDifficulty">The difficulty of the fish caught</param>
        public void InitializeMiniGame(int fishCatchDifficulty)
        {
            fishingCanvas.SetActive(true);
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
                        arrowSlots[_inputIndex].color = Color.green;
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
        public void StartQTE()
        {
            if (_isFishingFinished) return;

            _isQTEActive = true;
            _inputSequence.Clear();
            _inputIndex = 0;
            _inputTimer = inputTimeLimit;

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

                _inputSequence.Add(dirKey);
                arrowSlots[i].enabled = true;
            }
        }

        /// <summary>
        /// Disables the quick time event, cleaing the arrowslots
        /// </summary>
        void EndQTE()
        {
            _isQTEActive = false;
            foreach (var img in arrowSlots)
            {
                img.enabled = false;
            }
        }

        /// <summary>
        /// Calls the end QTE event function and the lose minigame
        /// </summary>
        void Fail()
        {
            EndQTE();
            LoseMiniGame();
        }

        /// <summary>
        /// Detects if a key has been pressed for any of the arrows
        /// </summary>
        /// /// <param name="key">The arrowkey pressed</param>
        bool CheckKeyPressed(out KeyCode key)
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
