using System;
using System.Collections.Generic;
using UnityEngine;

namespace FishingGame.QuestSystem
{
    /// <summary>
    /// Class which manages all quests in the world. Saves quests when ending the game and restores quests from persistent storage
    /// when loading a save. 
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        // TODO: SHOULD THIS BE NON STATIC AND EXTEND MONO BEHAVIOUR AS A SINGLETON SO I CAN REFERENCE PLAYER?
        private static List<IQuest> _quests;

        public static List<IQuest> Quests => _quests;

        static QuestManager() 
        {
            _quests = new List<IQuest>();
        }

        
        /// <summary>
        /// Add the specified quest to the list of quests. Typically called by NPCs
        /// </summary>
        /// <param name="quest">The quest to add</param>
        public static void AddQuest(IQuest quest)
        {
            _quests.Add(quest);
        }
        
        // Methods mapping to the IQUest Interface
        /// <summary>
        /// Ends the specified quest by calling EndQuest() on the quest scriptable object.
        /// </summary>
        /// <param name="questName">The quest to end.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the specified questName was not found.</exception>
        public static void EndQuest(string questName)
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
        public static void StartQuest(string questName)
        {
            int questIndex = FindQuestIndex(questName);
            
            _quests[questIndex].BeginQuest();
        }

        /// <summary>
        /// Progress the specified quest to the next stage.
        /// </summary>
        /// <param name="questName">The quest to move to the next stage.</param>
        public static void ProgressQuest(string questName)
        {
            int questIndex = FindQuestIndex(questName);
            
            _quests[questIndex].ProgressStage();
        }

        /// <summary>
        /// Progresses the specified quest to the specified quest stage.
        /// </summary>
        /// <param name="questName">The quest to progress.</param>
        /// <param name="questStage">The stage to progress the quest to.</param>
        public static void ProgressQuest(string questName, string questStage)
        {
            int questIndex = FindQuestIndex(questName);
            
            _quests[questIndex].ProgressStage(questStage);
        }

        public static bool HasQuestBegun(string questName)
        {
            int questIndex = FindQuestIndex(questName);
            return _quests[questIndex].IsQuestInProgress();
        }
        
        private static int FindQuestIndex(string questName)
        {
            int questIndex = _quests.FindIndex(quest => quest.Equals(questName));

            if (questIndex == QuestConstants.INDEX_NOT_FOUND)
            {
                throw new ArgumentOutOfRangeException($"Unable to start quest {questName}, as it was not found. Did you make a spelling mistake?");
            }
            return questIndex;
        }
    }
}
