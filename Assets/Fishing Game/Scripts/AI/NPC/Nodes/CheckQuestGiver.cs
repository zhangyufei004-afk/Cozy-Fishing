using FishingGame.GameTime;
using FishingGame.QuestSystem;
using JetBrains.Annotations;

namespace FishingGame.AI.NPC.Nodes
{
    /// <summary>
    /// Checks whether this NPC is a quest giver (i.e. They have a quest which can be given out).
    /// This class will also check whether the NPCs pre-quest conditions have been met and the quest is unstarted.
    /// The RunNode will return successful if the quest is unstarted, conditions have been met and the NPC gives out quests,
    /// otherwise it will return failure.
    /// </summary>
    public class CheckQuestGiver : TreeNode
    {
        private bool _isQuestGiver;
        private IQuest _quest;
        
        private readonly float _workStartTimeRatio;
        private readonly float _workEndTimeRatio;
        private readonly InGameTime _inGameTime;
        
        /// <summary>
        /// Constructs a CheckQuestGiver Object. 
        /// </summary>
        /// <param name="quest">The quest associated with this NPC.</param>
        /// <param name="workStartTimeRatio">The ratio of a day which represents the time work starts at. (e.g. 7am = 0.292)</param>
        /// <param name="workEndTimeRatio">The ratio of a day when work ends at (e.g. 7pm = 0.792, or 79.2% of the way through a day).</param>
        /// <param name="gameTime">The game time object which stores the games current time. </param>
        public CheckQuestGiver([CanBeNull] IQuest quest, float workStartTimeRatio, float workEndTimeRatio, InGameTime gameTime)
        {
            _workStartTimeRatio = workStartTimeRatio;
            _workEndTimeRatio = workEndTimeRatio;
            _inGameTime = gameTime;
                
            SetData("WorkStartTime", _workStartTimeRatio);
            SetData("WorkEndTime", _workEndTimeRatio);
            SetData("GameTime", _inGameTime);
            
            if (quest != null)
            { 
                _isQuestGiver = true;    
                _quest = quest;
                
            }
        }

        public override ETreeNodeState RunNode()
        {
            State = ETreeNodeState.Failure;
            
            if (_isQuestGiver && !_quest.IsQuestInProgress())
            {   // Quests can only be worked on during work hours
                float currentTime = _inGameTime.CurrentTimeOfDay;
                
                float workStartTime = _inGameTime.DayLength * _workStartTimeRatio;
                float workEndTime =  _inGameTime.DayLength * _workEndTimeRatio;

                if (currentTime >= workStartTime && currentTime <= workEndTime)
                {
                    State = ETreeNodeState.Success;
                }
            }
            return State;
        }
    }
}