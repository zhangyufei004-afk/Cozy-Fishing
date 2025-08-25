using System;
using FishingGame.GameManagement;
using UnityEngine;

namespace FishingGame.QuestSystem.UI
{
    public class QuestLogUI : MonoBehaviour
    {
        [Header("UI Elements")] 
        [SerializeField] private QuestDetailsUI questDetails;
        [SerializeField] private QuestScrollList questScrollList;

        private void OnEnable()
        {
            GameManager.Instance.GameEvents.OnQuestStateChange += QuestStateChange;
        }

        private void OnDisable()
        {
            GameManager.Instance.GameEvents.OnQuestStateChange -= QuestStateChange;
        }

        private void QuestStateChange(IQuest quest)
        {
            if (!quest.IsQuestInProgress())
            {   // If we havent started we dont want to display
                return;
            }
            questScrollList.AddQuestToList(quest, () =>
            {
                questDetails.InitializeQuestDetailsUI(quest);
            });
        }
    }
}
