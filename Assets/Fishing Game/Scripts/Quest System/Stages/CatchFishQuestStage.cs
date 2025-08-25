using System;
using FishingGame.GameManagement;
using UnityEngine;
using UnityEngine.Serialization;

namespace FishingGame.QuestSystem.Stages
{
    /// <summary>
    /// This quest stage counts how many fish have been caught and when it matches the specified number the Quest can progress.
    /// </summary>
    public class CatchFishQuestStage : QuestStage
    {
        [SerializeField] private int numFishToCatch;

        private int _numFishCaught;
        
        public override void StartStage()
        {
            throw new System.NotImplementedException();
        }

        private void OnEnable()
        {
            GameManager.Instance.GameEvents.OnFishCaught += FishCaught;
        }

        private void OnDisable()
        {
            GameManager.Instance.GameEvents.OnFishCaught -= FishCaught;
        }

        private void FishCaught()
        {
            if (_numFishCaught < numFishToCatch)
            {
                _numFishCaught++;
            }

            // We evaluate it again because we may now have the correct number
            if (_numFishCaught >= numFishToCatch)
            {
                FinishStage();
            }
        }
    }
}