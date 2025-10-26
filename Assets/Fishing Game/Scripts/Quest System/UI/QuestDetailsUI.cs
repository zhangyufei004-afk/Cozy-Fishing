using FishingGame.GameManagement;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FishingGame.QuestSystem.UI
{
    /// <summary>
    /// This class is responsible for displaying the details of the quest on the right hand side of the Quest Log.
    /// It has methods to refresh the UI elements so they display the correct data.
    /// </summary>
    public class QuestDetailsUI : MonoBehaviour
    {
        [Header("UI Elements")] 
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Button setActiveQuestButton;
        [SerializeField] private GameObject questStageContentParent;
        [SerializeField] private GameObject questRewardContentParent;
        [SerializeField] private GameObject selectPanel;
        [SerializeField] private GameObject detailsPanel;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject questStageUIPrefab;
        [SerializeField] private GameObject questRewardImagePrefab;

        private string _questName;

        /// <summary>
        /// Initialize the Quest Details UI with the details given from `quest`
        /// </summary>
        /// <param name="quest">The quest we are going to display the details of.</param>
        public void InitializeQuestDetailsUI(IQuest quest)
        {
            ClearExitingEntries();
            if (selectPanel.activeSelf)
            {
                selectPanel.SetActive(false);
                detailsPanel.SetActive(true);
            }
            _questName = quest.GetName();
            titleText.text = _questName;
            setActiveQuestButton.onClick.AddListener(() =>
            {
                GameManager.Instance.GameEvents.ChangeActiveQuest(_questName);
            });
            
            foreach (string questStageName in quest.GetCompletedStageNames())
            {
                TextMeshProUGUI stageName = Instantiate(questStageUIPrefab, questStageContentParent.transform)
                    .GetComponentInChildren<TextMeshProUGUI>();
                stageName.text = $"<s>{questStageName}</s>";
            }
            
            TextMeshProUGUI activeStageUI = Instantiate(questStageUIPrefab, questStageContentParent.transform)
                .GetComponentInChildren<TextMeshProUGUI>();
            activeStageUI.text = quest.GetCurrentStageName();

            foreach (var rewardSprite in quest.GetRewardImages())
            {
                Image imageComponent = Instantiate(questRewardImagePrefab, questRewardContentParent.transform)
                    .GetComponent<Image>();
                imageComponent.sprite = rewardSprite;
            }
        }

        private void ClearExitingEntries()
        {
            for (int i = 0; i < questRewardContentParent.transform.childCount; i++)
            {
                Destroy(questRewardContentParent.transform.GetChild(i).gameObject);
            }

            for (int i = 0; i < questStageContentParent.transform.childCount; i++)
            {
                Destroy(questStageContentParent.transform.GetChild(i).gameObject);
            }
        }
    }
}