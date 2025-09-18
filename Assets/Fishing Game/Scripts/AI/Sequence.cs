using System.Collections.Generic;

namespace FishingGame.AI
{
    /// <summary>
    /// Sequence Behaviour Tree Node. A sequence node will execute its children in sequence, from left to right.
    /// If one child fails to execute, this sequence node fails its execution. 
    /// </summary>
    public class Sequence : TreeNode
    {
        public Sequence() : base() {}
        public Sequence(List<TreeNode> treeNode) : base(treeNode) {}
        
        /// <summary>
        /// Runs this Sequence Node in the Tree once per frame. The node will execute its first child, and if that succeeds
        /// move on to the next child, until it either runs out of children, or a child fails to execute successfully.
        /// It then returns the child's execution state as its own.
        /// </summary>
        /// <returns>The current state of execution of this node.</returns>
        public override ETreeNodeState RunNode()
        {
            bool childExecuting = false;
            foreach (TreeNode treeNode in Children)
            {
                switch (treeNode.RunNode())
                {
                    case ETreeNodeState.Failure:
                        State = ETreeNodeState.Failure;
                        return State;
                    case ETreeNodeState.Success:
                        continue;
                    case ETreeNodeState.Running:
                        childExecuting = true;
                        continue;
                    default:
                        State = ETreeNodeState.Success;
                        return State;
                }
            }
            
            State = childExecuting ? ETreeNodeState.Running : ETreeNodeState.Success;
            return State;
        }

        /// <summary>
        /// Runs this Sequence Node's Physics in the Tree once per fixed update interval (1/60s). The node will execute its first child,
        /// and if that succeeds move on to the next child, until it either runs out of children, or a child fails to execute successfully.
        /// It then returns the child's execution state as its own.
        /// </summary>
        /// <returns>The current state of execution of this node.</returns>
        public override ETreeNodeState RunPhysics()
        {
            bool childExecuting = false;
            foreach (TreeNode treeNode in Children)
            {
                switch (treeNode.RunPhysics())
                {
                    case ETreeNodeState.Failure:
                        State = ETreeNodeState.Failure;
                        return State;
                    case ETreeNodeState.Success:
                        continue;
                    case ETreeNodeState.Running:
                        childExecuting = true;
                        continue;
                    default:
                        State = ETreeNodeState.Success;
                        return State;
                }
            }
            
            State = childExecuting ? ETreeNodeState.Running : ETreeNodeState.Success;
            return State;
        }
    }
}