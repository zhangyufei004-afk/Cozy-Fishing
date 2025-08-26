using System;
using FishingGame.GameManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingGame.QuestSystem.UI
{
    /// <summary>
    /// This class is responsible for the Entire Quest Log UI. It handles enabling and disabling the UI using input events,
    /// Initializing the Scroll List displaying all the quests, what each button entry in the scroll list does, and the
    /// initialization of the detail pane.
    /// </summary>
    public class QuestLogUI : MonoBehaviour
    {
        [Header("UI Elements")] 
        [SerializeField] private GameObject questLogPanel;
        [SerializeField] private QuestDetailsUI questDetails;
        [SerializeField] private QuestScrollList questScrollList;
        
        private bool _isUIActive;

        private void OnEnable()
        {
            GameManager.Instance.GameEvents.OnQuestStateChange += QuestStateChange;
            _isUIActive = false;
            InputActionAsset inputActions = InputSystem.actions;
            inputActions.Enable();
            inputActions.FindActionMap("Player").FindAction("ToggleQuestLog").performed += ToggleUI;
        }

        private void OnDisable()
        {
            GameManager.Instance.GameEvents.OnQuestStateChange -= QuestStateChange;
            InputActionAsset inputActions = InputSystem.actions;
            inputActions.Enable();
            inputActions.FindActionMap("Player").FindAction("ToggleQuestLog").performed -= ToggleUI;
        }

        private void QuestStateChange(IQuest quest)
        {
            if (!quest.IsQuestInProgress())
            {   // If we havent started we dont want to display
                return;
            }
            questScrollList.AddQuestToList(quest, () =>
            {
                questDetails.InitializeQuestDetailsUI(quest);
            });
        }

        private void ToggleUI(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _isUIActive = !_isUIActive;
                questLogPanel.SetActive(_isUIActive);
            }
        }
    }
}
