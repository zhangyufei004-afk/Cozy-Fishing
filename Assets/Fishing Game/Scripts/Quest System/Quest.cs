using System;
using System.Collections.Generic;
using UnityEngine;

namespace FishingGame.QuestSystem
{
    /// <summary>
    /// Class to represent a single quest. Each quest is associated with quest data, and can be started and completed by NPCs.
    /// </summary>
    public class Quest
    {
        public QuestData Data { get; private set; }


        private List<string> _questStages;
        private int _stageIndex;

        public Quest(QuestData data)
        {
            this.Data = data;
            _stageIndex = 0;
        }

        public void BeginQuest()
        {
            _questStages = Data.QuestStages;
            // TODO: TRIGGER DIALOGUE
        }
        
        private void ProgressStage()
        {
            _stageIndex++;
            // TODO: TRIGGER DIALOGUE
        }

        private void ProgressStage(string newStage)
        {
            // TODO: TRIGGER DIALOGUE
            if (_questStages is not null)
            {
                
                int newStageIndex = _questStages.IndexOf(newStage);
                
                if (newStageIndex is QuestConstants.INDEX_NOT_FOUND)
                {
                    throw new ArgumentException($"Invalid stage passed to quest {Data.QuestName}. Did you make a spelling mistake?");
                }

                _stageIndex = newStageIndex;
            }
        }
    }
}