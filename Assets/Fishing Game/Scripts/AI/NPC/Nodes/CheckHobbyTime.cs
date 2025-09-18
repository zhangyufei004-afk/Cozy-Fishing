using FishingGame.GameTime;

namespace FishingGame.AI.NPC.Nodes
{
    /// <summary>
    /// TreeNode to check whether it is currently the time for engaging in a hobby. If it is hobby time
    /// this node will return Success during <c>RunNode</c>. 
    /// </summary>
    public class CheckHobbyTime : TreeNode
    {
        private float _hobbyStartTime;
        private float _hobbyEndTime;
        private InGameTime _gameTime;
        
        public CheckHobbyTime(float hobbyStartTimeRatio, float hobbyEndTimeRatio)
        {
            _hobbyStartTime = hobbyStartTimeRatio;
            _hobbyEndTime = hobbyEndTimeRatio;
        }

        public override void Initialize()
        {
            _gameTime = GetData("GameTime") as InGameTime;

            if (_gameTime != null)
            {
                _hobbyStartTime *= _gameTime.DayLength;
                _hobbyEndTime *= _gameTime.DayLength;
            }
            base.Initialize();
        }

        /// <summary>
        /// Execute the Node. Checks whether the current time is the Hobby Time.
        /// </summary>
        /// <returns>Success if it is Hobby Time, Failure otherwise.</returns>
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