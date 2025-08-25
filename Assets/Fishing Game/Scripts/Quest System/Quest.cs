using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

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
        private List<String> _completedStages;
        private List<QuestStage> _questStages;

        public Quest(QuestData questData, int previousStageIndex)
        {
            this._questData = questData;
            this._currentStageIndex = previousStageIndex;
            this._currentState = EQuestState.RequirementsNotMet;
            _completedStages = new List<string>();
            _questStages = new List<QuestStage>();
            InitializeStagesList();
        }

        /// <summary>
        /// Progress the quest to the next stage.
        /// </summary>
        public void ProgressStage()
        {
            if (_currentStageIndex < _questStages.Count - 1)
            {
                string currentQuestStageName = _questStages[_currentStageIndex].GetName();
                _completedStages.Add(currentQuestStageName);
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
            return _currentState >= EQuestState.InProgress;
        }

        public string GetName()
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
            GameObject currentQuestStage = GetCurrentQuestStagePrefab();
            if (currentQuestStage is not null)
            {
                GameObject instantiatedQuestStage = Object.Instantiate(currentQuestStage, parentTransform);
                QuestStage questStage = instantiatedQuestStage.GetComponent<QuestStage>();
                questStage.InitializeStage(this.GetName());
            }
            
        }

        public string GetDescription()
        {
            return _questData.QuestDescription;
        }

        public List<string> GetCompletedStageNames()
        {
            return _completedStages;
        }

        public string GetCurrentStageName()
        {
            return _questStages[_currentStageIndex].GetName();
        }

        public List<Sprite> GetRewardImages()
        {
            return _questData.QuestRewardImages;
        }

        public string GetCurrentStageQuip()
        {
            return _questStages[_currentStageIndex].GetDialogue();
        }

        public bool CanQuestBeMarkedComplete()
        {
            return _currentState == EQuestState.CanFinish;
        }

        /// <summary>
        /// Gets the current quest stage prefab, only if the quest stage exists.
        /// </summary>
        /// <returns>The current quest stage if it exists, null otherwise.</returns>
        private GameObject GetCurrentQuestStagePrefab()
        {
            return CurrentStageExists() ? _questData.QuestStagePrefabs[_currentStageIndex] : null;
        }
        
        private bool CurrentStageExists()
        {
            return (_currentStageIndex < _questStages.Count);
        }

        private void InitializeStagesList()
        {
            _questStages.Clear();
            foreach (GameObject stagePrefab in _questData.QuestStagePrefabs)
            {
                QuestStage stage = stagePrefab.GetComponent<QuestStage>();
                _questStages.Add(stage);
            }
        }
    }
}