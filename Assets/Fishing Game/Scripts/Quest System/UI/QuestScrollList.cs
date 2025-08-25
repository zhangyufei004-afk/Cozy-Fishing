using UnityEngine;
using UnityEngine.Events;

namespace FishingGame.QuestSystem.UI
{
    /// <summary>
    /// This class is responsible for the Quest List - A scroll list UI element which contains buttons representing each active (has started)
    /// quest. 
    /// </summary>
    public class QuestScrollList : MonoBehaviour
    {
        [Header("UI Elements")] 
        [SerializeField] private GameObject contentContainer;
        [SerializeField] private GameObject logButtonPrefab;
        
        /// <summary>
        /// Add a quest to the scroll view list. The quest will be a button which has states the name of the quest, and
        /// completes the <c>onQuestClick</c> action.
        /// </summary>
        /// <param name="quest">The quest with which to represent.</param>
        /// <param name="onQuestClick">The onClick action to call when this quest button is clicked</param>
        public void AddQuestToList(IQuest quest, UnityAction onQuestClick)
        {
            QuestLogButton questLogButton = Instantiate(logButtonPrefab, contentContainer.transform)
                .GetComponent<QuestLogButton>();
            questLogButton.InitializeButton(quest.GetName(), onQuestClick);
        }
    }
}
