using System;
using FishingGame.GameManagement;

namespace FishingGame.AI.NPC.Nodes
{
    /// <summary>
    /// Behaviour Tree Node to check whether the NPC is within dialogue range of the player.
    /// Returns successful if the NPC is within the dialogue range of the player and the player is interacting with them, otherwise returns false.
    /// </summary>
    public class CheckWithinDialogueRange : TreeNode
    {
        private bool _isInDialogueRange = false;
        private bool _isPlayerInteracting = false;
        private readonly string _npcName;
        
        public CheckWithinDialogueRange(string npcName)
        {
            _npcName = npcName;
            GameManager gameManager = GameManager.Instance;
            gameManager.GameEvents.OnWithinDialogueRange += SetInDialogueRange;
            gameManager.GameEvents.OnNPCInteraction += SetPlayerInteracting;
        }

        public override ETreeNodeState RunNode()
        {
            if (_isInDialogueRange && _isPlayerInteracting)
            {
                State = ETreeNodeState.Success;
            }
            else
            {
                State = ETreeNodeState.Failure;
            }
            return State;
        }

        private void SetInDialogueRange(bool isInDialogueRange, string npcName)
        {
            if (npcName.Equals(this._npcName, StringComparison.CurrentCultureIgnoreCase))
            {
                _isInDialogueRange = isInDialogueRange;    
            }
        }

        private void SetPlayerInteracting(bool isPlayerInteracting, string npcName)
        {
            if (npcName.Equals(this._npcName, StringComparison.CurrentCultureIgnoreCase))
            {
                _isPlayerInteracting = isPlayerInteracting;
            }
        }
    }
}