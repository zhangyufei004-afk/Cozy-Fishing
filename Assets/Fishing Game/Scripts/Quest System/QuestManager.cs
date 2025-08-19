using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace FishingGame.QuestSystem
{
    /// <summary>
    /// Class which manages all quests in the world. Saves quests when ending the game and restores quests from persistent storage
    /// when loading a save. 
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance => _instance;
        
        [SerializeField] private List<QuestData> questDataObjects;
        
        private static QuestManager _instance;
        private List<IQuest> _quests;
        
        private void Awake()
        {
            if (_instance is not null &&  _instance != this)
            {
                Destroy(this);
            }
            _instance = this;
        }

        private void OnEnable()
        {   // TODO: MIGHT NEED TO MOVE THIS INTO AWAKE FOR PROPER SERIALIZATION
            
        }


        /// <summary>
        /// Add the specified quest to the list of quests. Typically called by NPCs
        /// </summary>
        /// <param name="quest">The quest to add</param>
        public void AddQuest(IQuest quest)
        {
            _quests.Add(quest);
        }
        
        // Methods mapping to the IQUest Interface
        /// <summary>
        /// Ends the specified quest by calling EndQuest() on the quest scriptable object.
        /// </summary>
        /// <param name="questName">The quest to end.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the specified questName was not found.</exception>
        public void EndQuest(string questName)
        {
            // TODO: GIVE REWARD (HOW ON A STATIC CLASS AND SCRIPTABLE OBJECT)?
            int questIndex = _quests.FindIndex(quest => quest.Equals(questName));

            if (questIndex == QuestConstants.INDEX_NOT_FOUND)
            {
                throw new ArgumentOutOfRangeException($"Quest {questName} was unable to be ended, as it was not found. Did you make a spelling mistake?");
            }
            
            _quests[questIndex].EndQuest();
        }

        /// <summary>
        /// Start the specified quest by calling BeginQuest() on the quest scriptable object.
        /// </summary>
        /// <param name="questName">The quest to start.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the specified questName was not found.</exception>
        public void StartQuest(string questName)
        {
            int questIndex = FindQuestIndex(questName);
            
            _quests[questIndex].BeginQuest();
        }

        /// <summary>
        /// Progress the specified quest to the next stage.
        /// </summary>
        /// <param name="questName">The quest to move to the next stage.</param>
        public void ProgressQuest(string questName)
        {
            int questIndex = FindQuestIndex(questName);
            
            _quests[questIndex].ProgressStage();
        }

        public bool HasQuestBegun(string questName)
        {
            int questIndex = FindQuestIndex(questName);
            return _quests[questIndex].IsQuestInProgress();
        }
        
        private int FindQuestIndex(string questName)
        {
            int questIndex = _quests.FindIndex(quest => quest.Equals(questName));

            if (questIndex == QuestConstants.INDEX_NOT_FOUND)
            {
                throw new ArgumentOutOfRangeException($"Unable to start quest {questName}, as it was not found. Did you make a spelling mistake?");
            }
            return questIndex;
        }

        private void InitializeQuests()
        {
            _quests = new List<IQuest>();
            foreach (QuestData questData in questDataObjects)
            {
                Quest newQuest = new Quest(questData);
                _quests.Add(newQuest);
            } 
        }
    }
}
