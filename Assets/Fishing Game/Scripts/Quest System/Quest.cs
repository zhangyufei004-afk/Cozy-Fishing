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

        /// <summary>
        /// Is the quest in Progress - Does its state match the in progress state.
        /// </summary>
        /// <returns>True if the Quest is in Progress, can be finished or has finished, false otherwise.</returns>
        public bool IsQuestInProgress()
        {
            return _currentState == EQuestState.InProgress;
        }
        
        /// <summary>
        /// Gets the quests name
        /// </summary>
        /// <returns>The quest name</returns>
        public string GetName()
        {
            return _questData.QuestName;
        }

        /// <summary>
        /// Sets the quests state to <c>newState</c>
        /// </summary>
        /// <param name="newState">The new state of the quest</param>
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

        /// <summary>
        /// Gets the quests description as decribed in the Scriptable Object Instance
        /// </summary>
        /// <returns>The Description string</returns>
        public string GetDescription()
        {
            return _questData.QuestDescription;
        }

        /// <summary>
        /// Gets a list of the names of all the completed quest stages
        /// </summary>
        /// <returns>A list of names of completed stages</returns>
        public List<string> GetCompletedStageNames()
        {
            return _completedStages;
        }

        /// <summary>
        /// Gets the current stage name
        /// </summary>
        /// <returns>The stage name</returns>
        public string GetCurrentStageName()
        {
            return _questStages[_currentStageIndex].GetName();
        }

        /// <summary>
        /// Gets the reward sprite images set in the Scriptable Object instance.
        /// </summary>
        /// <returns>A list of reward image sprites</returns>
        public List<Sprite> GetRewardImages()
        {
            return _questData.QuestRewardImages;
        }

        /// <summary>
        /// Gets the current stages quip dialogue
        /// </summary>
        /// <returns>The quip string</returns>
        public string GetCurrentStageQuip()
        {
            return _questStages[_currentStageIndex].GetDialogue();
        }

        /// <summary>
        /// Checks whether the quest can be marked completed or not
        /// </summary>
        /// <returns>True if the current state is Can Finish, false otherwise.</returns>
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