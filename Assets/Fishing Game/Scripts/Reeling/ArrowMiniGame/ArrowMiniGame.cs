using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace FishingGame.Reeling
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

        private FishScriptableObject _fishData;

        [SerializeField]
        [Tooltip("Reference to the ReelingMaster script.")]
        private ReelingMaster reelingMaster;

        //UI
        [SerializeField]
        [Tooltip("The image gameobjects that will be where each arrow is placed, left most is slot 1.")]
        private Image[] arrowSlots;

        private Vector2[] _arrowSlotPositions;

        [SerializeField]
        [Tooltip("The parent object in the ui")]
        private GameObject fishingCanvas;

        [SerializeField]
        [Tooltip("The bar object in the ui")]
        private GameObject uiBar;

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
        [Tooltip("The amount of time player has to beat the minigame")]
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
        /// Difficulty variable from the fishscriptableobject can be used to modify stats
        /// </summary>
        /// <param name="fishScriptable">The data of fish object being caught</param>
        public void InitializeMiniGame(FishScriptableObject fishScriptable)
        {
            _fishData = fishScriptable;
            uiBar.SetActive(true);
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

            if (UnityEngine.Input.anyKeyDown)
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
            if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow)) { key = KeyCode.UpArrow; return true; }
            if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow)) { key = KeyCode.DownArrow; return true; }
            if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow)) { key = KeyCode.LeftArrow; return true; }
            if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow)) { key = KeyCode.RightArrow; return true; }
            key = KeyCode.None;
            return false;
        }

        /// <summary>
        /// Wins the minigame, disables canvas and calls the end event functions
        /// </summary>
        public void WinMiniGame()
        {
            EndQTE();
            uiBar.SetActive(false);
            reelingMaster.EndCurrentMiniGame(true);
            StartCoroutine(HideUI());
        }

        /// <summary>
        /// Loses the minigame, disables canvas and calls the end event functions
        /// </summary>
        public void LoseMiniGame()
        {
            uiBar.SetActive(false);
            EndQTE();
            reelingMaster.EndCurrentMiniGame(false);
            StartCoroutine(HideUI());
        }

        /// <summary>
        /// A timer that hides the fishing canvas after 2 seconds
        /// This is run seperatly so the arrows can complete their animation
        /// Before beind hidden
        /// </summary>
        IEnumerator HideUI()
        {
            yield return new WaitForSeconds(2f);
            fishingCanvas.SetActive(false);

            foreach (var img in arrowSlots)
            {
                Animator animatorToUse;
                animatorToUse = img.GameObject().GetComponent<Animator>();
                animatorToUse.SetBool("IsActive", false);
                //  img.enabled = false;
            }
        }
    }

    
}
