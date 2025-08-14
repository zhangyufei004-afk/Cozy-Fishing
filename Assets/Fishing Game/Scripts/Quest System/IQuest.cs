
namespace FishingGame.QuestSystem
{
    /// <summary>
    /// Quest Interface defining common functions among all types of quests.
    /// </summary>
    public interface IQuest
    {
        public void EndQuest();
        public void BeginQuest();
        public void ProgressStage();
        public void ProgressStage(string newStage);
        public bool Equals(string otherQuestName);
        public bool IsQuestInProgress();
    }
}
