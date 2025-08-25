using System;
using UnityEngine;

namespace FishingGame.QuestSystem
{
    /// <summary>
    /// Script solely responsible for ending a quest when entering a trigger.
    /// </summary>
    public class QuestEnder : MonoBehaviour
    {
        [SerializeField] private string questNameToEnd;
        
        private void OnTriggerEnter(Collider other)
        {
            QuestManager.Instance.EndQuest(questNameToEnd);
        }
    }
}
