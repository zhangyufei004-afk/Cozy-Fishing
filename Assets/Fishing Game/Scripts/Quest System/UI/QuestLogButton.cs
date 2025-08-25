using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FishingGame.QuestSystem.UI
{
    public class QuestLogButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI buttonText;
        
        public void InitializeButton(string questName, UnityAction onClickAction)
        {
            button.onClick.AddListener(onClickAction);
            buttonText.text = questName;
        }
    }
}
