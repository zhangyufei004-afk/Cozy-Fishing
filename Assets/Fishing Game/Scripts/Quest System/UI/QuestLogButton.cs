using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FishingGame.QuestSystem.UI
{
    public class QuestLogButton : MonoBehaviour
    {
        private Button _button;
        private TextMeshProUGUI _buttonText;
        
        private void OnEnable()
        {
            _button = GetComponent<Button>();
            _buttonText = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void InitializeButton(string questName, UnityAction onClickAction)
        {
            _button.onClick.AddListener(onClickAction);
            _buttonText.text = questName;
        }
    }
}
