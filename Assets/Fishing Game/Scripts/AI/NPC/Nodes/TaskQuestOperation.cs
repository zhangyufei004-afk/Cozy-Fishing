using System.Collections.Generic;
using FishingGame.NPC.UI;
using FishingGame.QuestSystem;

namespace FishingGame.AI.NPC.Nodes
{
    /// <summary>
    /// Tree Node which either gives a quest to the player from this NPC, or ends the quest given out by this NPC.
    /// Displays the pre-/post-quest dialogue then once that dialogue has been displayed calls either the BeginQuest Event
    /// to alert the QuestManager a quest has begun, or the CompleteQuest event to alert the QuestManager a quest has been completed.
    /// </summary>
    public class TaskQuestOperation : TreeNode
    {
        private readonly string _questName;
        private readonly DialogueUI _questDialogueUI;
        private readonly List<string> _questDialogue;
        private readonly DialogueUI.EQuestOperation _questOperation;
        
        public TaskQuestOperation(string questName, List<string> questDialogue, DialogueUI questDialogueUI, DialogueUI.EQuestOperation operation)
        {
            _questName = questName;
            _questDialogueUI = questDialogueUI;
            _questDialogue = questDialogue;
            _questOperation = operation;
        }

        public override ETreeNodeState RunNode()
        {
            _questDialogueUI.DisplayDialogue(_questDialogue, _questName, _questOperation);

            State = ETreeNodeState.Running;
            return State;
        }
    }
}