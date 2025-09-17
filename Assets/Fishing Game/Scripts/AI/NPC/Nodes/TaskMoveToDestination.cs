using UnityEngine;
using UnityEngine.AI;

namespace FishingGame.AI.NPC.Nodes
{
    public class TaskMoveToDestination : TreeNode
    {
        private static readonly int Speed = Animator.StringToHash("Speed");
        private readonly NavMeshAgent _navMeshAgent;
        private readonly Animator _animator;
        
        public TaskMoveToDestination(NavMeshAgent navMeshAgent, Animator animator)
        {
            this._navMeshAgent = navMeshAgent;
            this._animator = animator;
        }

        public override ETreeNodeState RunNode()
        {
            // _animator.SetFloat(Speed, _navMeshAgent.velocity.magnitude);

            if (GetData("Destination") is Vector3 destination)
            {
                _navMeshAgent.SetDestination(destination);
                return ETreeNodeState.Running;
            }
            return ETreeNodeState.Failure;
        }
    }
}