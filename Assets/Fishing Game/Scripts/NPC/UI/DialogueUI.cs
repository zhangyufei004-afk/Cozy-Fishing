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

        public void DisplayDialogue(List<string> dialogue, [CanBeNull] string questNameToStart = null)
        {
            dialoguePanel.SetActive(true);
            _currentDialogueIndex = 0;
            StartCoroutine(DisplayDialogueRoutine(dialogue, questNameToStart));
        }

        IEnumerator DisplayDialogueRoutine(List<string> dialogue, [CanBeNull] string questNameToStart)
        {
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