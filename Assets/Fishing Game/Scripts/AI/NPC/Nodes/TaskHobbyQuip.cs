using FishingGame.NPC.UI;

namespace FishingGame.AI.NPC.Nodes
{
    /// <summary>
    /// Tree Node which displays the quip for the current hobby. This node would be used when the NPC
    /// is engaging in their hobby and the player interacts with them. 
    /// </summary>
    public class TaskHobbyQuip : TreeNode
    {
        private readonly string _hobbyQuip;
        private readonly DialogueUI _dialogueUI;
        
        public TaskHobbyQuip(string hobbyQuip, DialogueUI dialogueUI)
        {
            _dialogueUI = dialogueUI;
            _hobbyQuip = hobbyQuip;
        }

        public override ETreeNodeState RunNode()
        {
            _dialogueUI.DisplayDialogueLine(_hobbyQuip);
            State = ETreeNodeState.Running;
            return State;
        }
    }
}