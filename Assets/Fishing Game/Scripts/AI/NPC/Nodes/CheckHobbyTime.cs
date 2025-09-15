using FishingGame.GameTime;

namespace FishingGame.AI.NPC.Nodes
{
    /// <summary>
    /// TreeNode to check whether it is currently the time for engaging in a hobby. If it is hobby time
    /// this node will return Success during <c>RunNode</c>. 
    /// </summary>
    public class CheckHobbyTime : TreeNode
    {
        private readonly float _hobbyStartTime;
        private readonly float _hobbyEndTime;
        private readonly InGameTime _gameTime;
        
        public CheckHobbyTime(float hobbyStartTimeRatio, float hobbyEndTimeRatio)
        {
            _gameTime = GetData("GameTime") as InGameTime;

            if (_gameTime != null)
            {
                _hobbyStartTime = hobbyStartTimeRatio * _gameTime.DayLength;
                _hobbyEndTime = hobbyEndTimeRatio * _gameTime.DayLength;
            }
        }
        
        public override ETreeNodeState RunNode()
        {
            State = ETreeNodeState.Failure;
            if (_gameTime is not null)
            {
                if (_gameTime.CurrentTimeOfDay >= _hobbyStartTime && _gameTime.CurrentTimeOfDay <= _hobbyEndTime)
                {
                    State = ETreeNodeState.Success;
                }
            }

            return State;
        }
    }
}