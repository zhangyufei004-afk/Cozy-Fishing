using System;
using System.Collections.Generic;
using FishingGame.GameManagement;
using FishingGame.NPC.UI;
using FishingGame.QuestSystem;
using UnityEngine;

namespace FishingGame.NPC
{
    /// <summary>
    /// Class which gives out a quest to the player.
    /// </summary>
    [Obsolete("This class will be refactored in future versions to support Inkle scripts. " +
              "Do not rely heavily on its existing implementation.")]
    public class QuestGiver : MonoBehaviour
    {
        [Header("Quest Elements")]
        [SerializeField] private QuestManager questManager;
        [SerializeField] private string questName;
        [SerializeField] private List<string> preQuestDialogue;
        [SerializeField] private List<string> questEndDialogue;
        
        [Header("UI Elements")]
        [SerializeField] private DialogueUI questDialogueUI;

        private IQuest questToGive;

        private void Start()
        {
            questToGive = questManager.GetQuestByName(questName);
        }

        /// <summary>
        /// Interact with an NPC and display the appropriate dialogue.
        /// </summary>
        public void InteractWithNPC()
        {
            if (questToGive is not null && questToGive.IsQuestInProgress())
            {
                string stageDialogue = questToGive.GetCurrentStageQuip();
                List<string> dialogueList = new List<string>();
                dialogueList.Add(stageDialogue);
                questDialogueUI.DisplayDialogue(dialogueList);
                return;
            }
            
            if (questToGive is not null && questToGive.CanQuestBeMarkedComplete())
            {
                questDialogueUI.DisplayDialogue(questEndDialogue);
                return;
            }
            questDialogueUI.DisplayDialogue(preQuestDialogue, questName);
        }
        
    }
}