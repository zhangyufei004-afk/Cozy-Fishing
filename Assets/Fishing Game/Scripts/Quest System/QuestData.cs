using System.Collections.Generic;
using UnityEngine;

namespace FishingGame.QuestSystem
{
    /// <summary>
    /// Stores the Quest related data. New Quests are creatable by creating new instances of this Scriptable Object.
    /// </summary>
    [CreateAssetMenu(fileName = "NewQuest", menuName = "Fishing Game/Quests/New Quest")]
    public class QuestData : ScriptableObject
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
        
        
    }
}
