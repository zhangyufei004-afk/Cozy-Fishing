using UnityEngine;

namespace FishingGame.AI.NPC.Nodes
{
    /// <summary>
    /// Check Location Class. Extends Tree Node. Checks the distance to the current destination world position
    /// and if we are at the destination position (i.e. we have arrived, or are arriving) returns successful.
    /// </summary>
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