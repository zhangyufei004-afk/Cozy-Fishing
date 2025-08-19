namespace FishingGame.QuestSystem
{
    public class Quest : IQuest
    {
        public QuestData Data => _questData; 
        
        private readonly QuestData _questData;
        private int _currentStageIndex;

        public Quest(QuestData questData, int previousStageIndex)
        {
            this._questData = questData;
            this._currentStageIndex = previousStageIndex;
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
            throw new System.NotImplementedException();
        }

        public bool Equals(string otherQuestName)
        {
            throw new System.NotImplementedException();
        }

        public bool IsQuestInProgress()
        {
            throw new System.NotImplementedException();
        }
    }
}