namespace FishingGame.AI.NPC.Nodes
{
    public class TaskIdle : TreeNode
    {
        public TaskIdle()
        {
        }

        /// <summary>
        /// Idles the character
        /// </summary>
        /// <returns>Running</returns>
        public override ETreeNodeState RunNode()
        {
            return ETreeNodeState.Running;
        }
    }
}