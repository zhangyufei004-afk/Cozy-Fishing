using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FishingGame.QuestSystem.UI
{
    /// <summary>
    /// This class is responsible for the button logic in the ScrollView on the QuestLog. The buttons when clicked will
    /// change what is displayed in the details pane.
    /// </summary>
    public class QuestLogButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI buttonText;
        
        /// <summary>
        /// Initializes the button with the text <c>questName</c> and the OnClick handler of <c>onClickAction</c>
        /// </summary>
        /// <param name="questName">The name of the quest this button represents.</param>
        /// <param name="onClickAction">The action that should occur when we click on this button.</param>
        public void InitializeButton(string questName, UnityAction onClickAction)
        {
            button.onClick.AddListener(onClickAction);
            buttonText.text = questName;
        }
    }
}
