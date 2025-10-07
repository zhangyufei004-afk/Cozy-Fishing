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

        /// <summary>
        /// Checks whether the NPC is at the desired location specified during construction.
        /// </summary>
        /// <returns>Success if the player is at the location, otherwise Failure</returns>
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