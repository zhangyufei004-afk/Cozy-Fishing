using FishingGame.GameManagement;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;


namespace FishingGame.QuestSystem
{
    /// <summary>
    /// Abstract class representing the stages of a quest. Quest stages are the items which need to be completed, before the
    /// quest itself can be considered completed. For Example: Collect 5 Fish.
    /// </summary>
    public abstract class QuestStage : MonoBehaviour
    {
        // TODO: Set this up for Ink
        [SerializeField] protected string stageDialogue;
        [SerializeField] [CanBeNull] protected GameObject questReward;
        [SerializeField] protected string stageName;

        private bool _isComplete = false;
        private string _questName;
        
        
        public void InitializeStage(string questName)
        {
            this._questName = questName;
        }
        
        public abstract void StartStage();

        protected virtual void FinishStage()
        {
            if (!_isComplete)
            {
                _isComplete = true;
                GameManager.Instance.GameEvents.ProgressQuest(_questName);
                if (questReward != null)
                {
                    QuestReward.GivePlayerItem(questReward);
                }
                // TODO: DISPLAY DIALOGUE
                Destroy(this.gameObject);
            }
        }

        public string GetName()
        {
            return stageName;
        }

        public string GetDialogue()
        {
            return stageDialogue;
        }
    }
}