using System;
using System.Collections.Generic;
using FishingGame.AI.NPC.Nodes;
using FishingGame.GameManagement;
using FishingGame.GameTime;
using FishingGame.NPC.UI;
using FishingGame.QuestSystem;
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
        private static readonly int Speed = Animator.StringToHash("Speed");

        [Header("Resting Parameters")]
        [Tooltip("The home which the NPC rests at. When gizmos are drawing, this will show as a house.")]
        [SerializeField] private Vector3 homePosition;
        
        [Tooltip("The time the NPC goes home in the evening. \nUnits: 0-1 Representing the Decimal of the time of Day.\n"+ 
                 "Example: 7pm = 0.792")]
        [Range(0f, 1f)]
        [SerializeField] private float homeTime;
        
        [Header("Work Parameters")]
        [Tooltip("The location the NPC works at. When gizmos are drawing, this will show as a briefcase.")]
        [SerializeField] private Vector3 workPosition;
        
        [Tooltip("The time the NPC goes working in the morning. \nUnits: 0-1 Representing the Decimal of the time of Day.\n"+
                 "Example: 7am = 0.292")]
        [Range(0f, 1f)]
        [SerializeField] private float workTime;
        
        [Header("Hobby Parameters")]
        [SerializeField] private HobbyData hobby;
        
        [Tooltip("The time the NPC goes to their hobby in the afternoon. \nUnits: 0-1 Representing the Decimal of the time of Day.\n"+
                 "Example: 7am = 0.292")]
        [Range(0f, 1f)]
        [SerializeField] private float hobbyTime;
        
        private Vector3 _hobbyPosition;
        
        [Header("Time Properties")]
        [SerializeField] private InGameTime gameTime;
        
        [Header("NPC Parameters")]
        [SerializeField] private string npcName;
        
        [Header("Quest Parameters")]
        [SerializeField] private string questName;
        [SerializeField] private QuestManager questManager;
        
        [Header("UI Parameters")]
        [SerializeField] private DialogueUI dialogueUI;
        
        [Header("General Parameters")]
        [SerializeField] private Animator animator;
        [SerializeField] private NavMeshAgent agent;

        private float _initialMovementSpeed;

        private void Awake()
        {
            _initialMovementSpeed = agent.speed;
        }

        protected override void Start()
        {
            GameManager.Instance.GameEvents.OnToggleNPCMovement += ToggleMovement;
            base.Start();
        }

        private void LateUpdate()
        {
            float speed = agent.velocity.magnitude;
            animator.SetFloat(Speed, speed);
        }
        
        /// <summary>
        /// Creates the Behaviour Tree by constructing all the nodes and returning the root node of the tree.
        /// </summary>
        /// <returns>The root node of the Tree as a TreeNode object.</returns>
        protected override TreeNode SetupTree()
        {
            _hobbyPosition = hobby.HobbyLocation;
            IQuest quest = questManager.GetQuestByName(questName);
            
            TreeNode rootNode = new Selector(new List<TreeNode>
            {
                new Sequence(new List<TreeNode>
                {
                    new CheckWithinDialogueRange(npcName),
                    new Selector(new List<TreeNode>
                    {
                        new Sequence(new List<TreeNode>
                        {
                            new CheckQuestGiver(quest, workTime, hobbyTime, gameTime),
                            new TaskQuestOperation(questName, quest.GetPreQuestDialogue(), dialogueUI, DialogueUI.EQuestOperation.Start, npcName)
                        }),
                        new Sequence(new List<TreeNode>
                        {
                            new CheckQuestCanBeEnded(quest),
                            new TaskQuestOperation(questName, quest.GetQuestEndDialogue(), dialogueUI, DialogueUI.EQuestOperation.End, npcName)
                        }),
                        new Sequence(new List<TreeNode>
                        {
                            new CheckQuestActive(quest),
                            new TaskDisplayQuestStageQuip(quest, dialogueUI, npcName)
                        }),
                        new Sequence(new List<TreeNode>
                        {
                            new CheckHobbyTime(hobbyTime, homeTime),
                            new TaskHobbyQuip(hobby.HobbyQuip, dialogueUI, npcName)
                        })
                    })
                }),
                new Sequence(new List<TreeNode>
                {
                    new CheckDestination(transform, 
                        workPosition, 
                        homePosition, 
                        _hobbyPosition,
                        gameTime,
                        workTime,
                        hobbyTime,
                        homeTime
                    ),
                    new TaskMoveToDestination(agent, animator)
                }),
                new Sequence(new List<TreeNode>
                {
                    new CheckLocation(_hobbyPosition, transform),
                    new TaskCompleteHobby(hobby, animator, transform)
                }),
                new TaskIdle()
            });
            
            return rootNode;
        }


        private void ToggleMovement(bool isMovementEnabled, string npcName)
        {
            if (string.Equals(this.npcName, npcName, StringComparison.CurrentCultureIgnoreCase))
            {
                agent.speed = isMovementEnabled ? _initialMovementSpeed : 0f;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawIcon(homePosition, "../Fishing Game/Gizmos/NPC/home.png", true);
            Gizmos.DrawIcon(workPosition, "../Fishing Game/Gizmos/NPC/work.png", true);

        }
    }
}