using System;
using System.Collections.Generic;
using FishingGame.FishSystem;
using FishingGame.Inventory;
using FishingGame.QuestSystem;

namespace FishingGame.GameManagement
{
    /// <summary>
    /// <para>The game events class is responsible for all game wide events that a class may need to subscribe to, or trigger.</para>
    /// <list type="bullet">
    ///     <listheader>
    ///         <term>Example Events: </term>
    ///     </listheader>
    ///     <item>
    ///         <description>Quest Events (OnQuestStarted, OnQuestCompleted, ...)</description>
    ///     </item>
    ///     <item>
    ///         <description>Inventory Events (OnFishCaught, OnItemReceived, OnInventoryUpdated, ...)</description>
    ///     </item>
    ///     <item>
    ///         <description>Player Events (OnMovementToggled, OnDialogCameraEnabled, ...)</description>
    ///     </item>
    /// </list>
    /// </summary>
    public class GameEvents
    {
        #region Inventory Events

        public event Action<Fish> OnFishCaught;
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

        public event Action<bool, string> OnWithinDialogueRange;

        public event Action<bool, string> OnNPCInteraction;

        #endregion

        /// <summary>
        /// Fish Caught event - invokes all OnFishCaught subscribers
        /// </summary>
        /// /// <param name="fishCaught">The data object of the fish being caught</param>
        public void FishCaught(Fish fishCaught)
        {
            OnFishCaught?.Invoke(fishCaught);
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
        /// The quest requirements for <c>quest</c> have been met - invokes all subscribers to the OnQuestRequirementsMet event.
        /// </summary>
        /// <param name="quest">The name of the quest for which the requirements have been met.</param>
        public void QuestRequirementsMet(string quest)
        {
            OnQuestRequirementsMet?.Invoke(quest);
        }

        /// <summary>
        /// The quest <c>quest</c> has been started. This method alerts all classes subscribed to the OnQuestStarted event.
        /// </summary>
        /// <param name="quest">The name of the quest which has begun.</param>
        public void QuestStarted(string quest)
        {
            OnQuestStarted?.Invoke(quest);
        }

        /// <summary>
        /// The state of <c>quest</c> has changed. Alerts all subscribers to OnQuestStateChange of the state change.
        /// </summary>
        /// <param name="quest">The quest object which has changed its state.</param>
        public void QuestStateChange(IQuest quest)
        {
            OnQuestStateChange?.Invoke(quest);
        }
        
        /// <summary>
        /// <c>quest</c> has been completed. Invokes the OnQuestCompleted event to alert subscribers the quest has been completed.
        /// </summary>
        /// <param name="quest">The name of the quest which was completed</param>
        public void QuestCompleted(string quest)
        {
            OnQuestCompleted?.Invoke(quest);
        }

        /// <summary>
        /// Progress the quest named <c>quest</c> by invoking the OnQuestProgress event.
        /// </summary>
        /// <param name="quest">The name of the quest to progress</param>
        public void ProgressQuest(string quest)
        {
            OnQuestProgress?.Invoke(quest);
        }

        /// <summary>
        /// Changes the active quest to be <c>quest</c> by invoking the OnActiveQuestChanged event.
        /// </summary>
        /// <param name="quest"></param>
        public void ChangeActiveQuest(string quest)
        {
            OnActiveQuestChanged?.Invoke(quest);
        }

        /// <summary>
        /// Toggle the players movement to <c>isMovementEnabled</c>. 
        /// </summary>
        /// <param name="isMovementEnabled">Whether the players movement is enabled or disabled.</param>
        public void TogglePlayerMovement(bool isMovementEnabled)
        {
            OnTogglePlayerMovement?.Invoke(isMovementEnabled);
        }

        /// <summary>
        /// The item <c>item</c> has been received. Notify those subscribed to the OnItemReceived event.
        /// </summary>
        /// <param name="item">The item which was received.</param>
        public void ItemReceived(IStorable item)
        {
            OnItemReceived?.Invoke();
        }

        /// <summary>
        /// Toggle the dialogue camera to be enabled or disabled by invoking the OnToggleDialogueCamera event.
        /// </summary>
        /// <param name="isCameraEnabled">Whether the dialogue camera is enabled or disabled.</param>
        public void ToggleDialogueCamera(bool isCameraEnabled)
        {
            OnToggleDialogueCamera?.Invoke(isCameraEnabled);
        }

        /// <summary>
        /// Invokes the OnWithinDialogueRange event to let the NPC <c>npcName</c> know the player is within dialogue range.
        /// </summary>
        /// <param name="isInRange">Is the player in range of the NPC Character for dialogue. True if they are, false otherwise.</param>
        /// <param name="npcName">The name of the NPC we are in the dialogue range of.</param>
        public void WithinDialogueRange(bool isInRange, string npcName)
        {
            OnWithinDialogueRange?.Invoke(isInRange, npcName);
        }

        /// <summary>
        /// Invokes the OnNPCInteraction event to let the NPC named <c>npcName</c> know the player is interacting with them and wants dialogue displayed.
        /// </summary>
        /// <param name="isCurrentlyInteracting">Is the player currently interacting with <c>npcName</c> NPC. True if they are, false if they are no longer interacting.</param>
        /// <param name="npcName">The name of the NPC the player is interacting with.</param>
        public void NPCInteraction(bool isCurrentlyInteracting, string npcName)
        {
            OnNPCInteraction?.Invoke(isCurrentlyInteracting, npcName);
        }
    }
}