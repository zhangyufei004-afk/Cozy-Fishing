using System;
using System.Collections.Generic;
using System.Diagnostics;
using FishingGame.FishSystem;
using FishingGame.Inventory;
using FishingGame.Items.Bait;
using FishingGame.QuestSystem;
using UnityEditor;
using UnityEngine;

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
        public event Action<IStorable> OnItemReceived;
        public event Action<List<IStorable>> OnInventoryUpdated;
        public event Action<IStorable> OnItemUsedUp;
        public event Action<IBait> OnBaitEquipped;

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

        public event Action<string, Vector3> OnNPCFocus;

        public event Action<bool, string> OnNPCInteraction;

        public event Action<bool> OnBecomeOccupied;

        public event Action<bool> OnToggleGrappleCamera;

        public event Action<bool, string> OnWithinItemPickupRange;

        public event Action OnAttemptItemPickup;

        public event Action OnPlayerDeath;

        public event Action<bool> OnPlayerDeathScreenActive;

        #endregion

        #region AI Events

        public event Action<bool, string> OnToggleNPCMovement;

        #endregion

        #region UI Events

        public event Action<string, float, Color> OnShowStatusText;

        public event Action<string, float, Color> OnShowDefaultNotificationText;

        public event Action<string> OnElementAddedToScrollbox;

        #endregion

        /// <summary>
        /// Fish Caught event - invokes all OnFishCaught subscribers
        /// </summary>
        /// /// <param name="fishCaught">The data object of the fish being caught</param>
        public void FishCaught(IFishAble fishCaught)
        {
            if (fishCaught.GetCatchType() == ECatchableType.Fish)
            {
                OnFishCaught?.Invoke((Fish)fishCaught);
            }
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
            OnItemReceived?.Invoke(item);
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
        /// Invokes the OnNPCFocus event to tell the player which NPC should be focused on
        /// </summary>
        /// <param name="npcName">The name of the NPC</param>
        /// <param name="npcForwardVector">The forward vector of the NPC</param>
        public void NpcFocus(string npcName, Vector3 npcForwardVector)
        {
            OnNPCFocus?.Invoke(npcName, npcForwardVector);
        }

        /// <summary>
        /// Resets the players NPC focus by invoking the OnNPCFocus event with no npcName and a Zero forward vector.
        /// </summary>
        public void ResetNpcFocus()
        {
            OnNPCFocus?.Invoke("", Vector3.zero);
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

        /// <summary>
        /// Invokes the OnToggleNPCMovement event to tell the NPC named <c>npcName</c> to disable or enable its movement.
        /// </summary>
        /// <param name="isMovementEnabled">Bool to represent whether the movement is enabled. True if movement is enabled, false otherwise.</param>
        /// <param name="npcName">The name of the NPC to disable movement on.</param>
        public void ToggleNPCMovement(bool isMovementEnabled, string npcName)
        {
            OnToggleNPCMovement?.Invoke(isMovementEnabled, npcName);
        }

        /// <summary>
        /// Invokes the OnBecomeOccupied event with <c>isPlayerOccupied</c>. 
        /// </summary>
        /// <param name="isPlayerOccupied">Is the player currently occupied doing something else.</param>
        public void SetPlayerOccupied(bool isPlayerOccupied)
        {
            OnBecomeOccupied?.Invoke(isPlayerOccupied);
        }

        /// <summary>
        /// Invokes the OnToggleGrappleCamera event to tell the grapple camera to become <c>isCameraEnabled</c>
        /// </summary>
        /// <param name="isCameraEnabled">Bool for if the Camera is enabled or disabled.</param>
        public void ToggleGrappleCamera(bool isCameraEnabled)
        {
            OnToggleGrappleCamera?.Invoke(isCameraEnabled);
        }

        /// <summary>
        /// Run when the player is within range of an item for pickup
        /// Used primarily to display text to the player that they can pickup an item
        /// </summary>
        /// <param name="isInRange">True if in range, otherwise false</param>
        /// <param name="textToDisplay">The text to display to the player</param>
        public void PickupItemRange(bool isInRange, string textToDisplay)
        {
            OnWithinItemPickupRange?.Invoke(isInRange, textToDisplay);
        }

        /// <summary>
        /// Run when the player attempts to pickup an item and is range
        /// </summary>
        public void AttemptItemPickup()
        {
            OnAttemptItemPickup?.Invoke();
        }

        /// <summary>
        /// Run when an item has used its final charge
        /// </summary>
        /// <param name="itemUsedUp">The item that has used its final charge</param>
        public void ItemUsedUp(IStorable itemUsedUp)
        {
            OnItemUsedUp?.Invoke(itemUsedUp);
        }

        /// <summary>
        /// This can be run when a script wants to display some status text due to an event
        /// </summary>
        /// <param name="textToShow">The text to be shown</param>
        /// <param name="durationToShow">How long in seconds should this text stay up for</param>
        /// <param name="colorToUse">The color to use for the text</param>
        public void ShowStatusText(string textToShow, float durationToShow, Color colorToUse)
        {
            OnShowStatusText?.Invoke(textToShow, durationToShow, colorToUse);
        }

        /// <summary>
        /// This can be run when a script wants to display a default notification through the main canvas
        /// </summary>
        /// <param name="textToShow">The text to be shown</param>
        /// <param name="durationToShow">How long in seconds should this text stay up for</param>
        /// <param name="colorToUse">The color to use for the text</param>
        public void ShowNotificationText(string textToShow, float durationToShow, Color colorToUse)
        {
            OnShowDefaultNotificationText?.Invoke(textToShow, durationToShow, colorToUse);
        }

        /// <summary>
        /// This is run when a bait is attempted to be equiped
        /// </summary>
        /// <param name="baitToEquip">The item to equip</param>
        public void EquipBait(IBait baitToEquip)
        {
            OnBaitEquipped?.Invoke(baitToEquip);
        }

        /// <summary>
        /// Invokes the OnPlayerDeath event to do things when the player dies.
        /// </summary>
        public void PlayerDied()
        {
            OnPlayerDeath?.Invoke();
        }

        /// <summary>
        /// Invokes the OnPlayerDeathScreenActive event to indicate the death screen <c>isActive</c>
        /// </summary>
        /// <param name="isActive">Boolean indicating whether the screen is active or not</param>
        public void PlayerDeathScreenActive(bool isActive)
        {
            OnPlayerDeathScreenActive?.Invoke(isActive);
        }

        /// <summary>
        /// Invokes the OnElementAddedToScrollbox event to tell subscribing classes that an element has been added to the scroll box <c>scrollBoxName</c>
        /// </summary>
        /// <param name="scrollBoxName">The name of the scrollbox gameobject</param>
        public void ElementAddedToScrollbox(string scrollBoxName)
        {
            OnElementAddedToScrollbox?.Invoke(scrollBoxName);
        }
    }
}