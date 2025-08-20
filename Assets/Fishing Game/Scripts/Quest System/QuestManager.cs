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
        private Dictionary<string, IQuest> _quests;
        
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
            InitializeQuests();
        }


        /// <summary>
        /// Add the specified quest to the list of quests. Typically called by NPCs
        /// </summary>
        /// <param name="quest">The quest to add</param>
        public void AddQuest(IQuest quest)
        {
            _quests.Add(quest.GetId(), quest);
        }
        
        // Methods mapping to the IQUest Interface
        /// <summary>
        /// Ends the specified quest by calling EndQuest() on the quest scriptable object.
        /// </summary>
        /// <param name="questName">The quest to end.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the specified questName was not found.</exception>
        public void EndQuest(string questName)
        {
            if (!_quests.TryGetValue(questName, out IQuest quest))
            {
                throw new ArgumentOutOfRangeException($"Quest {questName} was unable to be ended, as it was not found. Did you make a spelling mistake?");
            }
            
            quest.EndQuest();
        }

        /// <summary>
        /// Start the specified quest by calling BeginQuest() on the quest scriptable object.
        /// </summary>
        /// <param name="questName">The quest to start.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the specified questName was not found.</exception>
        public void StartQuest(string questName)
        {
            _quests[questName].BeginQuest();
        }

        /// <summary>
        /// Progress the specified quest to the next stage.
        /// </summary>
        /// <param name="questName">The quest to move to the next stage.</param>
        public void ProgressQuest(string questName)
        {
            _quests[questName].ProgressStage();
        }

        /// <summary>
        /// Returns whether the specified quest has begun
        /// </summary>
        /// <param name="questName">The quest to check</param>
        /// <returns>True is the quest is in progress, false otherwise.</returns>
        public bool HasQuestBegun(string questName)
        {
            return _quests[questName].IsQuestInProgress();
        }
        /// <summary>
        /// Gets the IQuest object by the specified name. 
        /// </summary>
        /// <param name="questName">The name of the quest to retrieve.</param>
        /// <returns>The IQuest object which matches that name, or null if it wasn't found.</returns>
        public IQuest GetQuestByName(string questName)
        {
            return _quests[questName];
        }

        private void InitializeQuests()
        {
            _quests = new ();
            foreach (QuestData questData in questDataObjects)
            {   // TODO: ADD SERIALIZATION SO THE PREVIOUS STAGE INDEX MATCHES THE SAVED VERSION
                string questName = questData.QuestName;
                Quest newQuest = new Quest(questData, 0);
                _quests.Add(questName, newQuest);
            } 
        }
    }
}
