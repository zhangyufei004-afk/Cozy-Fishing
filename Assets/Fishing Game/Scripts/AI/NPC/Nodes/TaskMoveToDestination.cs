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

        /// <summary>
        /// Sets the navmesh destination to the intended destination for the time of day.
        /// </summary>
        /// <returns>Running if the destination was set successfully, Failure otherwise.</returns>
        public override ETreeNodeState RunNode()
        {
            if (GetData("Destination") is Vector3 destination)
            {
                _navMeshAgent.SetDestination(destination);
                return ETreeNodeState.Running;
            }
            return ETreeNodeState.Failure;
        }
    }
}