using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace FishingGame.AI.NPC.Nodes
{
    /// <summary>
    /// Tree Node which checks if the current destination is correct, and sets it if it isn't.
    /// </summary>
    public class CheckDestination : TreeNode
    {
        private readonly Transform _npcTransform;
        private Vector3 _currentDestinationWorld;

        public CheckDestination(
            Transform npcTransform, 
            Vector3 workplaceLocation,
            Vector3 homeLocation,
            Vector3 hobbyLocation) 
        {
            _npcTransform = npcTransform;
            
            SetData("Work", workplaceLocation);
            SetData("Home", homeLocation);
            SetData("Hobby", hobbyLocation);
        }

        public override ETreeNodeState RunNode()
        {
            
            // TODO: IMPLEMENT TIMES FOR MOVING TO DIFFERENT DESTINATIONS
            
            
            if (Vector3.Distance(_npcTransform.position, _currentDestinationWorld) < 0.1f)
            {
                SetData("Arriving", true);    
            }
            else
            {
                SetData("Arriving", false);
            }
            return base.RunNode();
        }
    }
}