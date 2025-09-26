using UnityEngine;

namespace FishingGame
{
    public interface IBait
    {
        public void ApplyBait();

        public void UseBaitCharge();

        public void BaitCatchBehaviour();

        public void BaitMinigameBehaviour();
    }
}
 