using UnityEngine;

namespace FishingGame.QuestSystem
{
    internal enum EQuestState : sbyte
    {
        RequirementsNotMet = -1,
        CanStart,
        InProgress,
        Finished
    }
    
    public class Quest : IQuest
    {
        public QuestData Data => _questData; 
        
        private readonly QuestData _questData;
        private int _currentStageIndex;
        private EQuestState _currentState;

        public Quest(QuestData questData, int previousStageIndex)
        {
            this._questData = questData;
            this._currentStageIndex = previousStageIndex;
            this._currentState = EQuestState.RequirementsNotMet;
        }
        public void EndQuest()
        {
            throw new System.NotImplementedException();
        }

        public void BeginQuest()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Progress the quest to the next stage.
        /// </summary>
        public void ProgressStage()
        {
            _currentStageIndex++;
        }

        public bool Equals(string otherQuestName)
        {
            throw new System.NotImplementedException();
        }

        public bool IsQuestInProgress()
        {
            return _currentState == EQuestState.InProgress;
        }

        public string GetId()
        {
            return _questData.QuestName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentTransform"></param>
        public void InstantiateQuestStep(Transform parentTransform)
        {
            QuestStage currentQuestStage = GetCurrentQuestStage();
            Object.Instantiate(currentQuestStage, parentTransform);
        }

        /// <summary>
        /// Gets the current quest stage, only if the quest stage exists.
        /// </summary>
        /// <returns>The current quest stage if it exists, null otherwise.</returns>
        private QuestStage GetCurrentQuestStage()
        {
            if (CurrentStageExists())
            {
                return _questData.QuestStages[_currentStageIndex];
            }

            return null;
        }
        
        private bool CurrentStageExists()
        {
            return (_currentStageIndex < _questData.QuestStages.Count);
        }
    }
}