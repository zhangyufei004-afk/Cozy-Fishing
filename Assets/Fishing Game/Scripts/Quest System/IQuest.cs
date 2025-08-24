
using UnityEngine;

namespace FishingGame.QuestSystem
{
    /// <summary>
    /// Quest Interface defining common functions among all types of quests.
    /// </summary>
    public interface IQuest
    {
        public void EndQuest();
        public void ProgressStage();
        public bool Equals(string otherQuestName);
        public bool IsQuestInProgress();
        public string GetId();
        public void SetState(EQuestState newState);
        public void InstantiateQuestStep(Transform parentTransform);
    }
}
