using System;
using UnityEngine;

namespace FishingGame.QuestSystem.Stages
{
    public class TravelToPointQuestStage : QuestStage
    {
        [Header("Point Details")] 
        [SerializeField] private Vector3 pointLocationWorld;
        
        public override void StartStage()
        {
            throw new System.NotImplementedException();
        }

        private void OnEnable()
        {
            this.transform.SetPositionAndRotation(pointLocationWorld, Quaternion.identity);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                FinishStage();
            }
        }
    }
}