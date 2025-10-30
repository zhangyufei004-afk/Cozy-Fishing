using FishingGame.GameManagement;
using JetBrains.Annotations;
using UnityEngine;


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
        
        /// <summary>
        /// Initializes the Stage to store the name of the quest it belongs to
        /// </summary>
        /// <param name="questName">The name of the quest this stage is apart of</param>
        public void InitializeStage(string questName)
        {
            this._questName = questName;
        }
        
        public abstract void StartStage();

        /// <summary>
        /// Finishes the stage by invoking the ProgressQuest event, dispensing the reward if there is one, and then
        /// destroying itself.
        /// </summary>
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

        /// <summary>
        /// Get the quest stages name. This is different to the quest name, and is the specific stage.
        /// <para>For Example:</para>
        /// <code>
        /// Class:                      CatchFishQuestStage.cs
        /// Number of Fish to catch:    5
        /// stageName :                 Catch 5 fish
        /// </code>
        /// </summary>
        /// <returns>The name of the stage</returns>
        public string GetName()
        {
            return stageName;
        }

        /// <summary>
        /// Gets the dialogue for this stage. This is a single string, and is more of a quip.
        /// </summary>
        /// <returns>The quip the NPC gives this stage.</returns>
        public string GetDialogue()
        {
            return stageDialogue;
        }
    }
}