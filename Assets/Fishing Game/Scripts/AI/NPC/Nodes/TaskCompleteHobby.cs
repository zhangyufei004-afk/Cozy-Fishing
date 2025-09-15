using UnityEngine;

namespace FishingGame.AI.NPC.Nodes
{
    /// <summary>
    /// TreeNode to execute an NPCs hobby. When this node runs it will animate the NPC to reflect the hobby animation,
    /// it will rotation the NPC to face in the correct direction for the hobby to occur (i.e. towards the lake if they are fishing,
    /// or towards a screen if they are watching a movie). 
    /// </summary>
    public class TaskCompleteHobby : TreeNode
    {
        private readonly string _animationBoolName;
        private readonly Quaternion _directionToFace;
        private readonly Animator _npcAnimator;
        private readonly Transform _transform;
        
        public TaskCompleteHobby(HobbyData hobbyData, 
            Animator npcAnimator,
            Transform transform)
        {
            _animationBoolName = hobbyData.HobbyAnimationBooleanName;
            _directionToFace = hobbyData.HobbyRotation;
            _npcAnimator = npcAnimator;
            _transform = transform;
        }

        public override ETreeNodeState RunNode()
        {
            _npcAnimator.SetBool(_animationBoolName, true);
            _transform.rotation = Quaternion.Lerp(_transform.rotation, _directionToFace, Time.deltaTime);
            return ETreeNodeState.Running;
        }
    }
}