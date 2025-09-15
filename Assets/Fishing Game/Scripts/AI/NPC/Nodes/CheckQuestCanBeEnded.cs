using FishingGame.GameTime;
using FishingGame.QuestSystem;
using JetBrains.Annotations;

namespace FishingGame.AI.NPC.Nodes
{
    /// <summary>
    /// Tree Node to check if the quest that this NPC gave to the player, and is currently active can be completed.
    /// If the quest can be completed, this node will return Success in <c>RunNode</c>, otherwise it will return Failure.
    /// </summary>
    public class CheckQuestCanBeEnded : TreeNode
    {
        private readonly IQuest _quest;
        private readonly float _workStartTime;
        private readonly float _workEndTime;
        private readonly InGameTime _gameTime;
        
        public CheckQuestCanBeEnded([CanBeNull] IQuest quest)
        {
            if (quest != null)
            {
                _quest = quest;
                float workStartTimeRatio = (float)GetData("WorkStartTime");
                float workEndTimeRatio = (float)GetData("WorkEndTime");
                _gameTime = GetData("GameTime") as InGameTime;

                if (_gameTime != null)
                {
                    _workStartTime = _gameTime.DayLength * workStartTimeRatio;
                    _workEndTime = _gameTime.DayLength * workEndTimeRatio;
                }
            }
        }

        public override ETreeNodeState RunNode()
        {
            State = ETreeNodeState.Failure;
            if (_quest.CanQuestBeMarkedComplete())
            {   // Quests can only be worked on during work hours
                if (_gameTime is not null)
                {
                    float currentTime =  _gameTime.CurrentTimeOfDay;

                    if (currentTime >= _workStartTime && currentTime <= _workEndTime)
                    {
                        State = ETreeNodeState.Success;
                    }
                }
            }
            return State;
        }
    }
}