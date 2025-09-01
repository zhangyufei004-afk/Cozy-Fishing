using UnityEngine;

namespace FishingGame.AI.NPC.Nodes
{
    public class TaskIdle : TreeNode
    {
        private static readonly int Speed = Animator.StringToHash("Speed");
        private readonly Animator _animator;

        public TaskIdle(Animator animator)
        {
            _animator = animator;
        }

        public override ETreeNodeState RunNode()
        {
            _animator.SetFloat(Speed, 0);
            return ETreeNodeState.Success;
        }
    }
}