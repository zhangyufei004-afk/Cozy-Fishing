using System.Collections;
using System.Collections.Generic;
using FishingGame.GameManagement;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingGame.NPC.UI
{
    /// <summary>
    /// UI element which displays the current dialogue and progresses on click.
    /// </summary>
    public class DialogueUI : MonoBehaviour
    {
        [Header("Dialogue UI Elements")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI dialogueText;
        
        private int _currentDialogueIndex = 0;

        public enum EQuestOperation
        {
            None = -1,
            Start,
            End
        }
        
        private void OnEnable()
        {
            InputActionAsset inputActions = InputSystem.actions;
            inputActions.Enable();
            inputActions.FindActionMap("UI").FindAction("ContinueDialogue").performed += IncrementDialogueIndex;
        }

        private void OnDisable()
        {
            InputActionAsset inputActions = InputSystem.actions;
            inputActions.Enable();
            inputActions.FindActionMap("UI").FindAction("ContinueDialogue").performed -= IncrementDialogueIndex;
        }

        /// <summary>
        /// Displays the specified single line of dialogue on the screen for the player to read.
        /// </summary>
        /// <param name="dialogueLine">The line of dialogue to display to the player.</param>
        /// <param name="initiatingNPCName">The name of the NPC who initiated this dialogue exchange.</param>
        /// <param name="questName">The name of the quest to have an operation performed on after the dialogue has been displayed. Can be null.</param>
        /// <param name="questOperation">The operation to perform on the specified quest. This operation will only be performed if questName is also specified.</param>
        public void DisplayDialogueLine(string dialogueLine, string initiatingNPCName, [CanBeNull] string questName = null,
            EQuestOperation questOperation = EQuestOperation.None)
        {
            List<string> dialogueLines = new List<string> { dialogueLine };
            DisplayDialogue(dialogueLines, initiatingNPCName, questName, questOperation);
        }

        /// <summary>
        /// Displays the specified Dialogue on the screen for the player to read.
        /// <para>
        /// Dialogue is a list because it can be multiple sentences worth, and only a single sentence will appear at one time.
        /// </para>
        /// </summary>
        /// <param name="dialogue">The list of sentences to display</param>
        /// <param name="initiatingNPCName">The name of the NPC who initiated this dialogue exchange.</param>
        /// <param name="questName">The quest which should be started/ended after displaying the dialogue. If not specified no Quest will be started/ended.</param>
        /// <param name="questOperation"></param>
        public void DisplayDialogue(List<string> dialogue, string initiatingNPCName, [CanBeNull] string questName = null, EQuestOperation questOperation = EQuestOperation.None)
        {
            if (!dialoguePanel.activeInHierarchy)
            {   // Only display dialogue if we aren't already
                dialoguePanel.SetActive(true);
                _currentDialogueIndex = 0;
                StartCoroutine(DisplayDialogueRoutine(dialogue, initiatingNPCName, questName, questOperation));
            }
        }

        private IEnumerator DisplayDialogueRoutine(List<string> dialogue, string initiatingNPCName, [CanBeNull] string questName, EQuestOperation questOperation)
        {
            GameManager.Instance.GameEvents.TogglePlayerMovement(false);
            GameManager.Instance.GameEvents.ToggleDialogueCamera(true);
            GameManager.Instance.GameEvents.ToggleNPCMovement(false, initiatingNPCName);

            int previousDialogueIndex = -1;
            while (_currentDialogueIndex < dialogue.Count)
            {
                if (previousDialogueIndex != _currentDialogueIndex)
                {
                    dialogueText.text = dialogue[_currentDialogueIndex];
                }
                yield return null;
            }
            dialoguePanel.SetActive(false);
            if (questName is not null)
            {
                switch (questOperation)
                {
                    case EQuestOperation.Start:
                        GameManager.Instance.GameEvents.QuestStarted(questName);
                        break;
                    case EQuestOperation.End:
                        GameManager.Instance.GameEvents.QuestCompleted(questName);
                        break;
                }
            }
            GameManager.Instance.GameEvents.TogglePlayerMovement(true);
            GameManager.Instance.GameEvents.ToggleDialogueCamera(false);
            GameManager.Instance.GameEvents.ToggleNPCMovement(true, initiatingNPCName);
            GameManager.Instance.GameEvents.NPCInteraction(false, initiatingNPCName);
        }

        private void IncrementDialogueIndex(InputAction.CallbackContext context)
        {
            if (context.performed && dialoguePanel.activeInHierarchy)
            {
                _currentDialogueIndex++;
            }
        }
    }
}