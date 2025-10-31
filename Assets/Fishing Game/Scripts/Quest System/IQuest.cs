
using System.Collections.Generic;
using UnityEngine;

namespace FishingGame.QuestSystem
{
    /// <summary>
    /// Quest Interface defining common functions among all types of quests.
    /// </summary>
    public interface IQuest
    {
        public void ProgressStage();
        public bool Equals(string otherQuestName);
        public bool IsQuestInProgress();
        public string GetName();
        public void SetState(EQuestState newState);
        public void InstantiateQuestStep(Transform parentTransform);
        public string GetDescription();
        public List<string> GetCompletedStageNames();
        public string GetCurrentStageName();
        public List<Sprite> GetRewardImages();
        public string GetCurrentStageQuip();
        public bool CanQuestBeMarkedComplete();
        public List<string> GetPreQuestDialogue();
        public List<string> GetQuestEndDialogue();
    }
}
