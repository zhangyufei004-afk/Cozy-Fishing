using UnityEngine;

namespace FishingGame.QuestSystem.Stages
{
    /// <summary>
    /// This quest stage is responsible for checking if the player has reached a specified point.
    /// </summary>
    public class TravelToPointQuestStage : QuestStage
    {
        [Header("Point Details")] 
        [Tooltip("The point the player needs to travel to - in world space coordinates.")]
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