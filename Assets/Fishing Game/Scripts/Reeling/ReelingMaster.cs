using FishingGame.FishSystem;
using FishingGame.Inventory;
using FishingGame.Player;
using FishingGame.UI.Inventory;
using System.Collections;
using System.Collections.Generic;
using FishingGame.GameManagement;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using FishingGame.Items;
using FishingGame.Items.Bait;
using UnityEngine.InputSystem;

namespace FishingGame.Reeling
{

    /// <summary>
    /// This class manages what minigames are currently active during the reeling process.
    /// It contains a list of all available minigame types, controls what one is currently playing
    /// and contains the logic to switch from one minigame to another.
    /// </summary>
    public class ReelingMaster : MonoBehaviour
    {
        private static readonly int Fishing = Animator.StringToHash("IsFishing");
        private static readonly int IsReeling = Animator.StringToHash("isReeling");

        #region Private Variables

        [Header("Script References")]

        [SerializeField]
        [Tooltip("A reference to the initiation script attatched to player.")]
        private ReelingInitiation initiationScript;

        [SerializeField]
        [Tooltip("A reference to the fishing hook script which is attatched to a fishing rod.")]
        private FishingHook fishingHook;

        [SerializeField]
        [Tooltip("A reference to the inventory system.")]
        private InventorySystem inventoryScript;

        [SerializeField]
        [Tooltip("A reference to the character controller")]
        private PlayerController characterController;

        [SerializeField]
        [Tooltip("A reference to the current fishing rod")]
        private FishingRod fishingRodScript;

        private IBait _currentBaitBeingUsed;

        [Header("Minigame Variables")]

        // Unity dosen't support making interface types a list so this is a gameobject list
        [SerializeField]
        [Tooltip("A list of all potential minigames.")]
        private List<GameObject> miniGameTypes;

        private List<GameObject> _currentPoolOfMiniGames;

        private GameObject _currentMinigame;
        private Fishable _currentlyReelingObject;
        private FishingPool _currentFishPool;
        private GameObject _current3DObject;

        private int _miniGameWinsRequired = 1;
        private int _currentMiniGameWins;
        private int _catchDifficulty;

        private bool _hasWon = false;

        [Header("UI elements")]

        [SerializeField]
        [Tooltip("The image UI element shown if succsesfully fishing.")]
        private Image caughtFishImage;

        [SerializeField]
        [Tooltip("The textbox that is displayed after fishing")]
        private TextMeshProUGUI fishingFinishedText;

        [SerializeField]
        [Tooltip("The timer UI element contained in ReelingUI.")]
        private GameObject timerObject;

        [SerializeField]
        [Tooltip("The sliders animator")]
        private Animator sliderAnimator;

        [SerializeField]
        [Tooltip("The slider for minigame progress")]
        private Slider minigameProgressSlider;

        [Header("Misc")]

        [Tooltip("This is a public variable that can be referenced to check if the player is currently fishing")]
        public bool IsFishing { get; private set; }

        [Header("Misc")]

        [SerializeField]
        [Tooltip("The animator attatched to the player")]
        private Animator characterAnimator;

        private InputAction _cancelFishingAction;
        [SerializeField]
        private AudioSource winSound;

        #endregion

        private void OnEnable()
        {
            InputActionAsset inputAsset = InputSystem.actions;
            InputActionMap uiActionMap = inputAsset.FindActionMap("UI");

            _cancelFishingAction = uiActionMap.FindAction("CancelFishing");
        }

        private void OnDisable()
        {
            if (IsFishing) { _cancelFishingAction.performed -= CancelFishing; }
        }

        #region Public Methods

        /// <summary>
        /// BeginCatchFish is run once a player succsesfully lands the fishing rod on a pool
        /// There are too versions of this function, one that takes a fish and one that takes a trash object instead
        /// This is the fish version
        /// </summary>
        /// <param name="fishCaught">The Fish Scriptable Object which was caught</param>
        /// <param name="visual3DObject">The 3D object of the fish</param>
        /// <param name="fishPool">The fish pool being fished from</param>
        public void BeginCatch(Fish fishCaught, GameObject visual3DObject, FishingPool fishPool)
        {
            _currentlyReelingObject = fishCaught;

            ConsistentBeginCatchLogic(visual3DObject, fishPool);
        }

        /// <summary>
        /// This is run once a player succsesfully completes the initial stage of reeling
        /// There are too versions of this function, one that takes a fish and one that takes a trash object instead
        /// This is the trash version
        /// </summary>
        /// <param name="trashCaught">The data of the trash caught</param>
        /// <param name="visual3DObject">Visual 3D object of what is being reeled</param>
        /// <param name="fishPool">The pool this was caught from</param>
        public void BeginCatch(Trash trashCaught, GameObject visual3DObject, FishingPool fishPool)
        {
            _currentlyReelingObject = trashCaught;

            ConsistentBeginCatchLogic(visual3DObject, fishPool);
        }

        /// <summary>
        /// Ends the current catch and runs logic based on if the player won the minigame or did not
        /// If player won the minigame this checks if they have won enough to have completed the catch
        /// If player lost the catch is immeaditly ended with a loss
        /// </summary>
        /// <param name="didWin">Represents if the player won the minigame or not</param>
        public void EndCurrentMiniGame(bool didWin)
        {
            if (didWin == false)
            {
                EndCatch(false);
                return;
            }

            _currentMiniGameWins += 1;
            Vector3 fishPosition = _current3DObject.transform.position;
            _current3DObject.transform.position = new Vector3(fishPosition.x, fishPosition.y += 0.20f, fishPosition.z);
            _hasWon = CheckIfWonEnough();

            if (_hasWon)
            {
                EndCatch(true);
                return;
            }
            else
            {
                SetNextMiniGame();
            }
        }

        /// <summary>
        /// Run when a fishing location is empty when fished from, rather than begin catch. This will display a text to the player
        /// saying the fishing location is empty. 
        /// </summary>
        /// <param name="fishingLocation">The fishing location that is empty</param>
        public void CaughtNothing(FishingPool fishingLocation)
        {
            string textToDisplay = $"The {fishingLocation.gameObject.name} is empty of fish!";

            GameManager.Instance.GameEvents.ShowNotificationText(textToDisplay, 3f, Color.red);
            fishingHook.PullBackHook(false);
            StartCoroutine(HideUIAfterCatch(2));
        }

        /// <summary>
        /// Enables or disables controls on the character controller
        /// Public function
        /// </summary>
        /// <param name="isDisabled">True means the controls should be disabled, otherwise false</param>
        public void DisableControls(bool isDisabled)
        {
            characterController.ToggleMovement(!isDisabled);
            fishingRodScript.AreReelingControlsActive(!isDisabled);
        }

        /// <summary>
        /// Run by a button, this will cancel fishing
        /// Does this differently based on fishing is in stage one or the minigame section
        /// </summary>
        public void CancelFishing(InputAction.CallbackContext inputAction)
        {
            fishingHook.ClearCollidingFishAndPool();
            fishingRodScript.SetChargerVisibility(false);
            _cancelFishingAction.performed -= CancelFishing;
            StartCoroutine(AnimatorResetTimer(2));
            if (IsFishing == true)
            {
                _currentMinigame.GetComponent<IReelingMinigame>().LoseMiniGame();
            }
            else
            {
                if (GetCurrentFishingRod().GetCurrentBait().IsBaitUsedUp() == true) { GetCurrentFishingRod().GetCurrentBait().UsedUpBait(); }
                initiationScript.CancelStageOne();
            }
        }

        /// <summary>
        /// Sets the visibility of the minigame progress slider
        /// True makes it visible false makes it not visible
        /// </summary>
        /// <param name="isVisible">True will set the slider to visible, otherwise false</param>
        public void SetMiniGameProgressVisibility(bool isVisible)
        {
            minigameProgressSlider.gameObject.SetActive(isVisible);
        }

        /// <summary>
        /// Returns the current 3D object representing the fish being reeled
        /// </summary>
        /// <returns>The 3D object of the current fish</returns>
        public GameObject GetCurrent3DFishObject()
        {
            return _current3DObject;
        }

        /// <summary>
        /// Returns the current fishing rod
        /// </summary>
        /// <returns>Returns the current fishing rod</returns>
        public FishingRod GetCurrentFishingRod()
        {
            return fishingRodScript;
        }


        #endregion

        /// <summary>
        /// Randomly selects the next minigame that will be played and then initializes and begins it
        /// The same minigame can not be player two times in a row
        /// </summary>
        private void SetNextMiniGame()
        {
            int index = Random.Range(0, _currentPoolOfMiniGames.Count);

            GameObject nextMiniGame = _currentPoolOfMiniGames[index];

            nextMiniGame.GetComponent<IReelingMinigame>().InitializeMiniGame(_currentlyReelingObject);
            _currentPoolOfMiniGames.Remove(nextMiniGame);
            _currentMinigame = nextMiniGame;
            _currentMinigame.GetComponent<IReelingMinigame>().BeginMiniGame();
        }

        /// <summary>
        /// Returns true if the player has won the required amount of minigames
        /// </summary>
        /// <returns>Returns true if the currentMiniGamesWon variables is greater than the minigamewins required variable</returns>
        private bool CheckIfWonEnough()
        {
            if (_currentMiniGameWins >= _miniGameWinsRequired)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the amount of minigames required to complete the catch
        /// This is based of the fishes difficulty
        /// </summary>
        /// <param name="fishDifficulty">The difficulty of caught fish</param>
        /// <returns>Returns the amount of minigames that will need to be done based on the inputed fishdifficulty</returns>
        private int GetMiniGamesRequired(int fishDifficulty)
        {
            if (fishDifficulty == 1) { return 1; }
            else if (fishDifficulty >= 2 && fishDifficulty < 5) { return 2; }
            else { return 3; }
        }


        /// <summary>
        /// Ends the catch process, reenabling player controls, reseting perspective and reseting key variables
        /// Enables victory or loss text based on result
        /// </summary>
        /// <param name="didWin">Represents if the minigame was succsesful or not</param>
        private void EndCatch(bool didWin)
        {
            _cancelFishingAction.performed -= CancelFishing;
            IsFishing = false;
            _currentMinigame = null;
            _currentMiniGameWins = 0;
            initiationScript.ShouldEnableFishPerspective(false);
            fishingHook.PullBackHook(true);
            sliderAnimator.SetBool("isGameActive", false);
            SetMiniGameProgressVisibility(false);
            StartCoroutine(ControlsAreDisabledAfterTime(false, 1));
            if (GetCurrentFishingRod().GetCurrentBait().IsBaitUsedUp() == true) { GetCurrentFishingRod().GetCurrentBait().UsedUpBait(); }
            _current3DObject.GetComponent<Animator>().SetBool("Active", false);
            characterAnimator.SetBool(IsReeling, false);
            characterAnimator.SetBool(Fishing, false);
            Destroy(_current3DObject);

            GameManager.Instance.GameEvents.SetPlayerOccupied(false);
            if (didWin == false)
            {
                DisplayFishingResult(_currentlyReelingObject, false);
                StartCoroutine(HideUIAfterCatch(2));
                _currentlyReelingObject = null;
            }
            else
            {
                if (_currentFishPool != null) { _currentFishPool.ObjectCaught(_currentlyReelingObject); }

                IStorable itemGained = (IStorable)_currentlyReelingObject;
                if (itemGained != null) { inventoryScript.AddItem(itemGained); }

                GameManager.Instance.GameEvents.FishCaught(_currentlyReelingObject);
                DisplayFishingResult(_currentlyReelingObject, true);

                _currentlyReelingObject = null;
                StartCoroutine(HideUIAfterCatch(2));
            }
        }

        /// <summary>
        /// This function is run by both overload methods of begin catch, it runs code that sets up variables/data
        /// that dosen't change based on what type of fishable is caught
        /// </summary>
        /// <param name="visual3DObject">Visual 3D object of what is being reeled</param>
        /// <param name="fishPool">The pool this was caught from</param>
        private void ConsistentBeginCatchLogic(GameObject visual3DObject, FishingPool fishPool)
        {
            GameManager.Instance.GameEvents.SetPlayerOccupied(true);

            _cancelFishingAction.performed += CancelFishing;
            _currentFishPool = fishPool;
            _current3DObject = visual3DObject;
            _current3DObject.GetComponent<Animator>().SetBool("Active", true);

            DisableControls(true);
            initiationScript.InitiateFishingPerspective();
            SetDefaultVariables();

            _miniGameWinsRequired = GetMiniGamesRequired(_catchDifficulty);
            _currentPoolOfMiniGames = new List<GameObject>(miniGameTypes);
            sliderAnimator.SetBool("isGameActive", true);
            SetMiniGameProgressVisibility(true);
            fishingRodScript.SetChargerVisibility(false);

            SetNextMiniGame();
        }

        /// <summary>
        /// Sets the default variables that dont require parameters
        /// </summary>
        private void SetDefaultVariables()
        {
            _catchDifficulty = _currentlyReelingObject.GetCatchDifficulty();
            _currentMinigame = null;
            _currentMiniGameWins = 0;
            IsFishing = true;
        }

        /// <summary>
        /// Hides the ui shown after completiting a reel
        /// </summary>
        private void HideReelFinishedUI()
        {
            fishingFinishedText.gameObject.SetActive(false);
            caughtFishImage.gameObject.SetActive(false);
        }       

        /// <summary>
        /// Starts a timer that will then run the HideReelFinishedUI() function
        /// Also resets the position of the hook, this avoids camera freaking out as it is attatched to the hook
        /// </summary>
        private IEnumerator HideUIAfterCatch(int secondsToWait)
        {
            yield return new WaitForSeconds(secondsToWait);
            HideReelFinishedUI();
        }

        /// <summary>
        /// Starts a timer to disable or reenable controls
        /// </summary>
        /// <param name="disable">True to disable controls</param>
        /// <param name="secondssToWait">Amount of seconds to wait before disabling or enabling</param>
        /// <returns></returns>
        private IEnumerator ControlsAreDisabledAfterTime(bool disable, int secondssToWait)
        {
            yield return new WaitForSeconds(secondssToWait);
            DisableControls(disable);
        }

        /// <summary>
        /// Displays the fishing reslt after a catch is ended. Formats the text based on what is caught
        /// Sets the required UI elements to be active
        /// If the catch was a failure string will be formated to show that the fish got away
        /// </summary>
        /// <param name="fishData">The data of the fish being reeled</param>
        /// <param name="didCatch">Was the fish caught</param>
        private void DisplayFishingResult(Fishable fishData, bool didCatch)
        {
            if (didCatch)
            {
                winSound.Play();

                string textToDisplay = $"You have caught a {fishData.GetWeight()}kg {fishData.GetName()}!";

                GameManager.Instance.GameEvents.ShowNotificationText(textToDisplay, 3f, Color.green);
                caughtFishImage.sprite = fishData.GetTexture();
                caughtFishImage.gameObject.SetActive(true);
            }
            else
            {
                string textToDisplay = $"The {fishData.GetName()} got away!";

                GameManager.Instance.GameEvents.ShowNotificationText(textToDisplay, 3f, Color.red);
            }
        }

        /// <summary>
        /// A timer that does required UI functions after inputed time
        /// </summary>
        /// <param name="timeToWait">Time to wait</param>
        /// <returns>Sets the slider animator to not be active</returns>
        private IEnumerator AnimatorResetTimer(float timeToWait)
        {
            yield return new WaitForSeconds(timeToWait);
            sliderAnimator.SetBool("isGameActive", false);
        }
    }
}
