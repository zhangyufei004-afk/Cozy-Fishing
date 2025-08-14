using System;
using System.Collections.Generic;
using FishingGame.SaveGame;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace FishingGame.QuestSystem
{
    /// <summary>
    /// Stores the Quest related data. New Quests are creatable by creating new instances of this Scriptable Object.
    /// </summary>
    [CreateAssetMenu(fileName = "NewQuest", menuName = "Fishing Game/Quests/New Quest")]
    public class QuestData : SerializableObject, IQuest
    {
        // Public Getters. These are properties to remove the set function for memory integrity.
        public string QuestName => questName;
        public string QuestDescription => questDescription;
        public GameObject QuestReward => questReward;
        public double QuestMoneyReward => questMoneyReward;
        public bool IsMonetaryRewardQuest => isMonetaryRewardQuest;
        public List<string> QuestStages => questStages;
        
        // TODO: ADD DIALOGUE SYSTEM (MAYBE INK?)
        
        // TODO: ADD DATA FOR CAMERA POSITION BASED ON DIALOGUE?
        
        [SerializeField] private string questName;
        [SerializeField] private string questDescription;
        [SerializeField] private GameObject questReward;
        [SerializeField] private double questMoneyReward;
        [SerializeField] private bool isMonetaryRewardQuest;
        [SerializeField] private List<string> questStages = new List<string> {"Beginning", "End"}; // TODO: MAYBE CHANGE THIS TO AN ENUM I CAN ADJUST SOMEHOW??
        [SerializeField][HideInInspector] private int currentStageIndex;

        /// <summary>
        /// Constructs a new Quest Data object using the specified persistentID
        /// </summary>
        /// <param name="persistentID">The ID of the persisted SerializableObject</param>
        internal QuestData(int persistentID) : base(persistentID)
        {
        }

        /// <summary>
        /// End the quest. This will also trigger the ending quest dialogue.
        /// </summary>
        public void EndQuest()
        {
            // TODO: DO I NEED THIS? - WE COULD HANDLE ALL THIS IN QUEST MANAGER
        }

        /// <summary>
        /// Begin the quest. This will also trigger the beginning quest dialogue.
        /// </summary>
        public void BeginQuest()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Progress the quest to the next stage.
        /// </summary>
        public void ProgressStage()
        {
            currentStageIndex++;
        }

        /// <summary>
        /// Progresses the quest to the specified stage (`newStage`)
        /// </summary>
        /// <param name="newStage">The stage we are progressing the quest to.</param>
        public void ProgressStage(string newStage)
        {
            // TODO: TRIGGER DIALOGUE
            if (questStages is not null)
            {
                
                int newStageIndex = questStages.IndexOf(newStage);
                
                if (newStageIndex is QuestConstants.INDEX_NOT_FOUND)
                {
                    throw new ArgumentException($"Invalid stage passed to quest {QuestName}. Did you make a spelling mistake?");
                }

                currentStageIndex = newStageIndex;
            }
        }

        /// <summary>
        /// Compares the quest names to check whether they are equal.
        /// </summary>
        /// <param name="otherQuestName"></param>
        /// <returns></returns>
        public bool Equals(string otherQuestName)
        {
            return this.questName == otherQuestName;
        }

        public bool IsQuestInProgress()
        {
            return this.currentStageIndex > -1;
        }

        // TODO: DO I NEED THIS?
        // public static bool operator ==(QuestData thisQuestData, QuestData otherQuestData)
        // {
        //     if (thisQuestData is null || otherQuestData is null)
        //     {
        //         return false;
        //     }
        //     
        //     if (object.ReferenceEquals(thisQuestData, otherQuestData))
        //     {
        //         return true;
        //     }
        //     return thisQuestData.QuestName == otherQuestData.QuestName;
        // }
        //
        // public static bool operator !=(QuestData thisQuestData, QuestData otherQuestData)
        // {
        //     return !(thisQuestData == otherQuestData);
        // }

        /// <summary>
        /// Triggers the dialogue to be shown at the current stage.
        /// The current stage is read from _currentStageIndex.
        /// </summary>
        private void TriggerDialogue()
        {
            // TODO: TRIGGER DIALOGUE AT THE CURRENT INDEX
            throw new System.NotImplementedException("TODO: Dialogue Triggering");
        }
    }
}
