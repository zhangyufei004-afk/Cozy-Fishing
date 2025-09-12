using System.Collections.Generic;
using FishingGame.AI.NPC.Nodes;
using FishingGame.GameTime;
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
        [Header("Resting Parameters")]
        [Tooltip("The home which the NPC rests at.")]
        [SerializeField] private Vector3 homePosition;
        
        [Tooltip("The time the NPC goes home in the evening. \nUnits: 0-1 Representing the Decimal of the time of Day.\n"+ 
                 "Example: 7pm = 0.792")]
        [Range(0f, 1f)]
        [SerializeField] private float homeTime;
        
        [Header("Work Parameters")]
        [Tooltip("The location the NPC works at.")]
        [SerializeField] private Vector3 workPosition;
        
        [Tooltip("The time the NPC goes working in the morning. \nUnits: 0-1 Representing the Decimal of the time of Day.\n"+
                 "Example: 7am = 0.292")]
        [Range(0f, 1f)]
        [SerializeField] private float workTime;
        
        [Header("Hobby Parameters")]
        //TODO: [SerializeField] private HobbyData hobby;
        private Vector3 _hobbyPosition;
        
        [Header("Time Properties")]
        [SerializeField] private InGameTime gameTime;
        
        
        protected override TreeNode SetupTree()
        {
            TreeNode rootNode = new Selector(new List<TreeNode>
            {
                new Sequence(new List<TreeNode>
                {
                    new CheckDestination(transform, 
                        workPosition, 
                        homePosition, 
                        _hobbyPosition,
                        gameTime
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