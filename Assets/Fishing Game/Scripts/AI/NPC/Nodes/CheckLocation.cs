using UnityEngine;

namespace FishingGame.AI.NPC.Nodes
{
    public class CheckLocation : TreeNode
    {
        private readonly Vector3 _targetLocation;
        private readonly Transform _agentTransform;
        
        public CheckLocation(Vector3 desiredLocation, Transform agentTransform)
        {
            _targetLocation = desiredLocation;
            _agentTransform = agentTransform;
        }

        public override ETreeNodeState RunNode()
        {
            if (Vector3.Distance(_agentTransform.position, _targetLocation) < 0.1f)
            {
                return ETreeNodeState.Success;
            }
            return ETreeNodeState.Failure;
        }
    }
}