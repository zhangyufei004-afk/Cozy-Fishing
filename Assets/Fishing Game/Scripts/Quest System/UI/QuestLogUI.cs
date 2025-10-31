using FishingGame.GameManagement;
using UnityEngine;

namespace FishingGame.QuestSystem.UI
{
    /// <summary>
    /// This class is responsible for the Entire Quest Log UI. It handles enabling and disabling the UI using input events,
    /// Initializing the Scroll List displaying all the quests, what each button entry in the scroll list does, and the
    /// initialization of the detail pane.
    /// </summary>
    public class QuestLogUI : MonoBehaviour
    {
        [Header("UI Elements")] 
        [SerializeField] private QuestDetailsUI questDetails;
        [SerializeField] private QuestScrollList questScrollList;

        private void OnEnable()
        {
            GameManager.Instance.GameEvents.OnQuestStateChange += QuestStateChange;
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
