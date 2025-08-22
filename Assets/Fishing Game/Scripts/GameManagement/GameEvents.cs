using System;
using System.Collections.Generic;
using FishingGame.Inventory;

namespace FishingGame.GameManagement
{
    public class GameEvents
    {
        #region Inventory Events

        public event Action OnFishCaught;
        public event Action OnItemReceived;
        public event Action<List<IStorable>> OnInventoryUpdated;

        #endregion

        #region Quest Events

        public event Action<string> OnQuestRequirementsMet;
        public event Action<string> OnQuestStarted;
        public event Action<string> OnQuestStateChange;

        public event Action<string> OnQuestProgress;
        public event Action<string> OnQuestCompleted;

        #endregion
        
        /// <summary>
        /// Fish Caught event - invokes all OnFishCaught subscribers
        /// </summary>
        public void FishCaught()
        {
            OnFishCaught?.Invoke();
        }

        /// <summary>
        /// A new item has been added to the inventory - invokes all subscribers to the OnInventoryUpdated event
        /// </summary>
        /// <param name="itemList">The list of items in the inventory</param>
        public void InventoryUpdated(List<IStorable> itemList)
        {
            OnInventoryUpdated?.Invoke(itemList);
        }
        
        /// <summary>
        /// The quest requirements for 
        /// </summary>
        /// <param name="quest"></param>
        public void QuestRequirementsMet(string quest)
        {
            OnQuestRequirementsMet?.Invoke(quest);
        }

        public void QuestStarted(string quest)
        {
            OnQuestStarted?.Invoke(quest);
        }

        public void QuestStateChange(string quest)
        {
            OnQuestStateChange?.Invoke(quest);
        }

        public void QuestCompleted(string quest)
        {
            OnQuestCompleted?.Invoke(quest);
        }

        public void ProgressQuest(string quest)
        {
            OnQuestProgress?.Invoke(quest);
        }
    }
}