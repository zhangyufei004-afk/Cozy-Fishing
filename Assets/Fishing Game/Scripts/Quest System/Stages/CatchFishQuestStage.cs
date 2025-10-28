using System;
using System.Collections.Generic;
using FishingGame.FishSystem;
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
        [SerializeField] private List<FishScriptableObject> typeOfFish;

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
        
        /// <summary>
        /// Run when a fish is caught
        /// If the fish caught is equal to a type of fish wanted by quest or no specific fish are needed for the quest
        /// This runs the evaluate fish count function
        /// </summary>
        /// <param name="fishCaught">Data of the fish that was caught</param>
        private void FishCaught(Fish fishCaught)
        {
            if (typeOfFish.Count == 0 || typeOfFish.Contains(fishCaught.GetFishBase()))
            {
                EvaluateFishCount();
            }
        }

        /// <summary>
        /// Updates the amount of fish caught and then checks if it has reached the required amount
        /// </summary>
        private void EvaluateFishCount()
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