using System;
using System.Collections.Generic;
using FishingGame.Inventory;
using FishingGame.QuestSystem;

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
        public event Action<IQuest> OnQuestStateChange;

        public event Action<string> OnQuestProgress;
        public event Action<string> OnQuestCompleted;

        #region UI

        public event Action<string> OnActiveQuestChanged;

        #endregion

        #endregion

        #region Player Events

        public event Action<bool> OnTogglePlayerMovement;

        public event Action<bool> OnToggleDialogueCamera;

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

        public void QuestStateChange(IQuest quest)
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

        public void ChangeActiveQuest(string quest)
        {
            OnActiveQuestChanged?.Invoke(quest);
        }

        public void TogglePlayerMovement(bool isMovementEnabled)
        {
            OnTogglePlayerMovement?.Invoke(isMovementEnabled);
        }

        public void ItemReceived(IStorable item)
        {
            OnItemReceived?.Invoke();
        }

        public void ToggleDialogueCamera(bool isCameraEnabled)
        {
            OnToggleDialogueCamera?.Invoke(isCameraEnabled);
        }
    }
}