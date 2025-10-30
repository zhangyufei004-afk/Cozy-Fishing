using System.Collections.Generic;
using FishingGame.SaveGame;
using UnityEngine;

namespace FishingGame.QuestSystem
{
    /// <summary>
    /// Stores the Quest related data. New Quests are creatable by creating new instances of this Scriptable Object.
    /// </summary>
    [CreateAssetMenu(fileName = "NewQuest", menuName = "Fishing Game/Quests/New Quest")]
    public class QuestData : SerializableObject
    {
        // Public Getters. These are properties to remove the set function for memory integrity.
        public string QuestName => questName;
        public string QuestDescription => questDescription;
        public GameObject QuestReward => questReward;
        public double QuestMoneyReward => questMoneyReward;
        public bool IsMonetaryRewardQuest => isMonetaryRewardQuest;
        public List<GameObject> QuestStagePrefabs => questStagePrefabs;
        
        public List<Sprite> QuestRewardImages => questRewardImages;
        public List<string> PreQuestDialogueLines => preQuestDialogueLines;
        public List<string> PostQuestDialogueLines => postQuestDialogueLines;
        
        
        [Header("Quest Details")]
        [SerializeField] private string questName;
        [SerializeField] private string questDescription;
        
        [Header("Quest Rewards")]
        [SerializeField] private GameObject questReward;
        [SerializeField] private double questMoneyReward;
        [SerializeField] private bool isMonetaryRewardQuest;
        [SerializeField] private List<Sprite> questRewardImages;

        
        [Header("Quest Stages")]
        [SerializeField] private List<GameObject> questStagePrefabs;
        
        [Header("Quest NPC Dialogue")]
        [SerializeField] private List<string> preQuestDialogueLines;
        [SerializeField] private List<string> postQuestDialogueLines;
        
        /// <summary>
        /// Constructs a new Quest Data object using the specified persistentID
        /// </summary>
        /// <param name="persistentID">The ID of the persisted SerializableObject</param>
        internal QuestData(int persistentID) : base(persistentID)
        {
        }
    }
}
