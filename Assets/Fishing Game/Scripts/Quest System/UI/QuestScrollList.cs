using UnityEngine;
using UnityEngine.Events;

namespace FishingGame.QuestSystem.UI
{
    public class QuestScrollList : MonoBehaviour
    {
        [Header("UI Elements")] 
        [SerializeField] private GameObject contentContainer;
        [SerializeField] private GameObject logButtonPrefab;

        public void AddQuestToList(IQuest quest, UnityAction onQuestClick)
        {
            QuestLogButton questLogButton = Instantiate(logButtonPrefab, contentContainer.transform)
                .GetComponent<QuestLogButton>();
            questLogButton.InitializeButton(quest.GetId(), onQuestClick);
        }
    }
}
