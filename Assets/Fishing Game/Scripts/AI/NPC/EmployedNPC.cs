using System.Collections.Generic;
using FishingGame.AI.NPC.Nodes;
using UnityEngine;
using UnityEngine.AI;

namespace FishingGame.AI.NPC
{
    /// <summary>
    /// Behaviour Tree for an NPC who is employed. 
    /// <list type="bullet">
    ///     <listheader>
    ///         <term>An employed NPC has three main actions:</term>
    ///     </listheader>
    ///     <item>
    ///         <description>Attending Work</description>
    ///     </item>
    ///     <item>
    ///         <description>Engaging in a Hobby</description>
    ///     </item>
    ///     <item>
    ///         <description>Resting at home</description>
    ///     </item>
    /// </list>
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class EmployedNpc : BehaviourTree
    {
        [SerializeField] private Vector3 homePosition;
        [SerializeField] private Vector3 workPosition;
        //TODO: [SerializeField] private HobbyData hobby;
        private Vector3 _hobbyPosition;
        
        protected override TreeNode SetupTree()
        {
            TreeNode rootNode = new Selector(new List<TreeNode>
            {
                new Sequence(new List<TreeNode>
                {
                    new CheckDestination(transform, 
                        workPosition, 
                        homePosition, 
                        _hobbyPosition
                    ),
                    new TaskMoveToDestination(GetComponent<NavMeshAgent>(), GetComponent<Animator>())
                }),
                new Sequence(new List<TreeNode>
                {
                    new CheckLocation(_hobbyPosition, transform),
                    new TaskCompleteHobby()
                }),
                new TaskIdle(GetComponent<Animator>())
            });
            
            return rootNode;
        }
    }
}