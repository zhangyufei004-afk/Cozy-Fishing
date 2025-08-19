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
    public class QuestData : SerializableObject
    {
        // Public Getters. These are properties to remove the set function for memory integrity.
        public string QuestName => questName;
        public string QuestDescription => questDescription;
        public GameObject QuestReward => questReward;
        public double QuestMoneyReward => questMoneyReward;
        public bool IsMonetaryRewardQuest => isMonetaryRewardQuest;
        public List<QuestStage> QuestStages => questStages;
        
        [SerializeField] private string questName;
        [SerializeField] private string questDescription;
        [SerializeField] private GameObject questReward;
        [SerializeField] private double questMoneyReward;
        [SerializeField] private bool isMonetaryRewardQuest;
        [SerializeField] private List<QuestStage> questStages;
        [SerializeField][HideInInspector] private int currentStageIndex;

        /// <summary>
        /// Constructs a new Quest Data object using the specified persistentID
        /// </summary>
        /// <param name="persistentID">The ID of the persisted SerializableObject</param>
        internal QuestData(int persistentID) : base(persistentID)
        {
        }
    }
}
