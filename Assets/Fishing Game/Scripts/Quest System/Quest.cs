using UnityEngine;

namespace FishingGame.QuestSystem
{
    public enum EQuestState : sbyte
    {
        RequirementsNotMet = -1,
        CanStart,
        InProgress,
        CanFinish,
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
            throw new System.NotImplementedException("TODO: IMPLEMENT ENDING QUESTS");
        }

        /// <summary>
        /// Progress the quest to the next stage.
        /// </summary>
        public void ProgressStage()
        {
            if (_currentStageIndex < _questData.QuestStages.Count - 1)
            {
                _currentStageIndex++;    
            }
            else
            {
                throw new QuestException("Unable to Progress Stage as the quest has reached the end of its stages.");
            }
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

        public void SetState(EQuestState newState)
        {
            this._currentState = newState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentTransform"></param>
        public void InstantiateQuestStep(Transform parentTransform)
        {
            QuestStage currentQuestStage = GetCurrentQuestStage();
            if (currentQuestStage is not null)
            {
                Object.Instantiate(currentQuestStage, parentTransform);
                currentQuestStage.InitializeStage(this.GetId());
            }
            
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