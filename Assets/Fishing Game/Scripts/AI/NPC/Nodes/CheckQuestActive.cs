using FishingGame.GameTime;
using FishingGame.QuestSystem;
using JetBrains.Annotations;

namespace FishingGame.AI.NPC.Nodes
{
    /// <summary>
    /// Tree Node to check whether the quest this NPC gives out (if any) is currently active.
    /// If the NPC is a quest giver (such that they give the player a quest), and that quest is current in progress (active)
    /// then this node will return Success during <c>RunNode</c>.
    /// </summary>
    public class CheckQuestActive : TreeNode
    {
        private readonly IQuest _quest;
        private readonly float _workStartTime;
        private readonly float _workEndTime;
        private readonly InGameTime _gameTime;
        
        public CheckQuestActive([CanBeNull] IQuest quest)
        {
            if (quest != null)
            {
                _quest = quest;
            } 
            float workStartTimeRatio = (float)GetData("WorkStartTime");
            float workEndTimeRatio = (float)GetData("WorkEndTime");
            _gameTime = GetData("GameTime") as InGameTime;

            if (_gameTime != null)
            {
                _workStartTime = _gameTime.DayLength * workStartTimeRatio;
                _workEndTime = _gameTime.DayLength * workEndTimeRatio;
            }
        }

        public override ETreeNodeState RunNode()
        {
            State = ETreeNodeState.Failure;
            if (_quest.IsQuestInProgress())
            {
                if (_gameTime is not null)
                {
                    float currentTime = _gameTime.CurrentTimeOfDay;

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