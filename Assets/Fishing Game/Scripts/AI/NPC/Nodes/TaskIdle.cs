using UnityEngine;

namespace FishingGame.AI.NPC.Nodes
{
    public class TaskIdle : TreeNode
    {
        public TaskIdle()
        {
        }

        public override ETreeNodeState RunNode()
        {
            return ETreeNodeState.Success;
        }
    }
}