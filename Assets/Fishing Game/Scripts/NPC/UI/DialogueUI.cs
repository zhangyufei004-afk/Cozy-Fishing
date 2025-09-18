using System;
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
        /// Displays the specified Dialogue on the screen for the player to read.
        /// <para>
        /// Dialogue is a list because it can be multiple sentences worth, and only a single sentence will appear at one time.
        /// </para>
        /// </summary>
        /// <param name="dialogue">The list of sentences to display</param>
        /// <param name="questNameToStart">The quest which should be started after displaying the dialogue. If not specified no Quest will be started.</param>
        public void DisplayDialogue(List<string> dialogue, [CanBeNull] string questNameToStart = null)
        {
            dialoguePanel.SetActive(true);
            _currentDialogueIndex = 0;
            StartCoroutine(DisplayDialogueRoutine(dialogue, questNameToStart));
        }

        private IEnumerator DisplayDialogueRoutine(List<string> dialogue, [CanBeNull] string questNameToStart)
        {
            GameManager.Instance.GameEvents.SetPlayerOccupied(true);

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
            if (questNameToStart is not null)
            {
                GameManager.Instance.GameEvents.QuestStarted(questNameToStart);
            }
            GameManager.Instance.GameEvents.TogglePlayerMovement(true);
            GameManager.Instance.GameEvents.ToggleDialogueCamera(false);
            GameManager.Instance.GameEvents.SetPlayerOccupied(false);

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