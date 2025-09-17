using System.Collections.Generic;
using FishingGame.NPC.UI;
using FishingGame.QuestSystem;

namespace FishingGame.AI.NPC.Nodes
{
    /// <summary>
    /// Tree Node which displays the current quest stage's quip dialogue. The quip which is displayed
    /// depends upon the stage of the quest, and thus will vary at runtime. 
    /// </summary>
    public class TaskDisplayQuestStageQuip : TreeNode
    {
        private readonly IQuest _quest;
        private readonly DialogueUI _questDialogueUI;
        private readonly string _npcName;
        
        public TaskDisplayQuestStageQuip(IQuest quest, DialogueUI questDialogueUI, string npcName)
        {
            _quest = quest;
            _questDialogueUI = questDialogueUI;
            _npcName = npcName;
        }

        public override ETreeNodeState RunNode()
        {
            _questDialogueUI.DisplayDialogueLine(_quest.GetCurrentStageQuip(), _npcName);
            State = ETreeNodeState.Running;
            return State;
        }
    }
}