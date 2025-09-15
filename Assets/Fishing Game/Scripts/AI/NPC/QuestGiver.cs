using System;
using System.Collections.Generic;
using FishingGame.GameManagement;
using FishingGame.NPC.UI;
using FishingGame.QuestSystem;
using UnityEngine;

namespace FishingGame.AI.NPC
{
    /// <summary>
    /// Class which gives out a quest to the player. The class should be placed on an NPC Gameobject, and then you can interact
    /// with them through methods in this class.
    /// </summary>
    [Obsolete("This has been deprecated. Please use the new Behaviour Tree system instead. See: EmployedNPC. This will be removed in future versions.")]
    public class QuestGiver : MonoBehaviour
    {
        [Header("Quest Elements")]
        [SerializeField] private QuestManager questManager;
        [SerializeField] private string questName;
        [SerializeField] private List<string> preQuestDialogue;
        [SerializeField] private List<string> questEndDialogue;
        
        [Header("UI Elements")]
        [SerializeField] private DialogueUI questDialogueUI;

        private IQuest _questToGive;

        private void Start()
        {
            _questToGive = questManager.GetQuestByName(questName);
        }

        /// <summary>
        /// Interact with an NPC and display the appropriate dialogue.
        /// </summary>
        public void InteractWithNPC()
        {
            if (_questToGive is not null && _questToGive.IsQuestInProgress())
            {
                string stageDialogue = _questToGive.GetCurrentStageQuip();
                List<string> dialogueList = new List<string>();
                dialogueList.Add(stageDialogue);
                questDialogueUI.DisplayDialogue(dialogueList);
                return;
            }
            
            if (_questToGive is not null && _questToGive.CanQuestBeMarkedComplete())
            {
                questDialogueUI.DisplayDialogue(questEndDialogue);
                return;
            }
            questDialogueUI.DisplayDialogue(preQuestDialogue, questName);
        }
        
    }
}