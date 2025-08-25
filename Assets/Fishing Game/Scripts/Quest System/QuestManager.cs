using System;
using System.Collections.Generic;
using FishingGame.GameManagement;
using UnityEngine;

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
        private Dictionary<string, GameObject> _questStageParentGameObjects;
        
        private void Awake()
        {
            if (_instance is not null &&  _instance != this)
            {
                Destroy(this);
            }
            _instance = this;
            InitializeQuests();
        }

        private void OnEnable()
        {   // TODO: MIGHT NEED TO MOVE THIS INTO AWAKE FOR PROPER SERIALIZATION
            GameManager gameManager = GameManager.Instance;
            gameManager.GameEvents.OnQuestStarted += StartQuest;
            gameManager.GameEvents.OnQuestCompleted += EndQuest;
            gameManager.GameEvents.OnQuestProgress += ProgressQuest;
        }

        private void OnDisable()
        {
            GameManager gameManager = GameManager.Instance;
            gameManager.GameEvents.OnQuestStarted -= StartQuest;
            gameManager.GameEvents.OnQuestCompleted -= EndQuest;
            gameManager.GameEvents.OnQuestProgress -= ProgressQuest;
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
        public void StartQuest(string questName)
        {
            IQuest quest = _quests[questName];
            quest.InstantiateQuestStep(_questStageParentGameObjects[questName].transform);
            ChangeState(quest.GetId(), EQuestState.InProgress);
        }

        /// <summary>
        /// Progress the specified quest to the next stage.
        /// </summary>
        /// <param name="questName">The quest to move to the next stage.</param>
        public void ProgressQuest(string questName)
        {
            try
            {
                IQuest quest = _quests[questName];
                quest.ProgressStage();
                quest.InstantiateQuestStep(_questStageParentGameObjects[questName].transform);
            }
            catch (QuestException exception)
            {   // If we catch an exception, we have reached the end of all the quest stages
                ChangeState(questName, EQuestState.CanFinish);
#if UNITY_EDITOR
                Debug.LogWarning(exception);
#endif
            }
            
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

        /// <summary>
        /// Changes the quest <c>questName</c>'s state to <c>questState</c>
        /// <br></br><br></br>
        /// <list type="bullet">
        ///     <listheader>
        ///         <term>Example Usages: </term>
        ///     </listheader>
        ///     <item>
        ///         <description>Setting a quest to requirements met to be able to start</description>
        ///     </item>
        ///     <item>
        ///         <description>Setting a quest to be completed in the state. </description>
        ///     </item>
        ///     <item>
        ///         <description>Setting a quest to in progress when we start a quest.</description>
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="questName">The name of the quest we want to change.</param>
        /// <param name="questState">The new state of the quest. </param>
        public void ChangeState(string questName, EQuestState questState)
        {
            IQuest quest = GetQuestByName(questName);
            quest.SetState(questState);
            GameManager.Instance.GameEvents.QuestStateChange(quest);
        }
        
        private void InitializeQuests()
        {
            _quests = new Dictionary<string, IQuest>();
            _questStageParentGameObjects = new Dictionary<string, GameObject>();
            foreach (QuestData questData in questDataObjects)
            {   /* 
                *   TODO: ADD SERIALIZATION SO THE PREVIOUS STAGE INDEX MATCHES THE SAVED VERSION
                *    DATE: 25-08-2025 
                */ 
                string questName = questData.QuestName;
                Quest newQuest = new Quest(questData, 0);
                _quests.Add(questName, newQuest);
                ChangeState(questName, EQuestState.RequirementsNotMet);
                
                // Add the quest stage parents - Instantiate at start time for object pooling efficiency
                GameObject questStageParentGameObject = new GameObject(questName);
                questStageParentGameObject.transform.SetParent(transform);
                _questStageParentGameObjects.Add(questName, questStageParentGameObject);
                
            }
        }
    }
}
