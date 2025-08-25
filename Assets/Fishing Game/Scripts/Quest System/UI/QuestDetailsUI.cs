using FishingGame.GameManagement;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FishingGame.QuestSystem.UI
{
    public class QuestDetailsUI : MonoBehaviour
    {
        [Header("UI Elements")] 
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Button setActiveQuestButton;
        [SerializeField] private GameObject questStageContentParent;
        [SerializeField] private GameObject questRewardContentParent;
        [SerializeField] private GameObject selectPanel;
        [SerializeField] private GameObject detailsPanel;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject questStageUIPrefab;
        [SerializeField] private GameObject questRewardImagePrefab;

        private string _questName;

        public void InitializeQuestDetailsUI(IQuest quest)
        {
            if (selectPanel.activeSelf)
            {
                selectPanel.SetActive(false);
                detailsPanel.SetActive(true);
            }
            _questName = quest.GetName();
            titleText.text = _questName;
            descriptionText.text = quest.GetDescription();
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
    }
}