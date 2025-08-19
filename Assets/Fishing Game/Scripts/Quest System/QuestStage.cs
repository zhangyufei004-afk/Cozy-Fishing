using System;
using FishingGame.Inventory;
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
        [SerializeField] protected TextAsset questDialogue;
        [SerializeField] [CanBeNull] protected GameObject questReward;

        private bool _isComplete = false;

        public abstract void StartStage();

        protected virtual void FinishStage()
        {
            if (!_isComplete)
            {
                _isComplete = true;

                if (questReward is not null)
                {
                    QuestReward.GivePlayerItem(questReward);
                }
                // TODO: DISPLAY DIALOGUE
                Destroy(this.gameObject);
            }
        }
    }
}