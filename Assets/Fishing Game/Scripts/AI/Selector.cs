using System.Collections.Generic;

namespace FishingGame.AI
{
    /// <summary>
    /// Selector Behaviour Tree node. This node operates like an OR gate. It will execute only one child, starting execution
    /// from the left and with every failed execution moving to the right in the sub tree.
    /// </summary>
    public class Selector : TreeNode
    {
        public Selector() : base() {}
        public Selector(List<TreeNode> treeNode) : base(treeNode) {}
        
        /// <summary>
        /// Runs this Selector Node in the Tree once per frame. The node will execute its first child, and if that fails move on to the next child,
        /// until it either runs out of children, or a child executes successfully. It then returns the childs execution state as its own.
        /// </summary>
        /// <returns>The current state of execution of this node.</returns>
        public override ETreeNodeState RunNode()
        {
            foreach (TreeNode treeNode in Children)
            {
                switch (treeNode.RunNode())
                {
                    case ETreeNodeState.Failure:
                        continue;
                    case ETreeNodeState.Success:
                        State = ETreeNodeState.Success;
                        return State;
                    case ETreeNodeState.Running:
                        State = ETreeNodeState.Running;
                        return State;
                    default:
                        continue;
                }
            }

            State = ETreeNodeState.Failure;
            return State;
        }

        /// <summary>
        /// Runs this Selector Node's Physics logic in the Tree once per Fixed Update Interval (1/60s). The node will execute
        /// its first child, and if that fails move on to the next child, until it either runs out of children, or a child
        /// executes successfully. It then returns the child's execution state as its own.
        /// </summary>
        /// <returns>The current state of execution of this node.</returns>
        public override ETreeNodeState RunPhysics()
        {
            foreach (TreeNode treeNode in Children)
            {
                switch (treeNode.RunPhysics())
                {
                    case ETreeNodeState.Failure:
                        continue;
                    case ETreeNodeState.Success:
                        State = ETreeNodeState.Success;
                        return State;
                    case ETreeNodeState.Running:
                        State = ETreeNodeState.Running;
                        return State;
                    default:
                        continue;
                }
            }

            State = ETreeNodeState.Failure;
            return State;
        }
    }
}