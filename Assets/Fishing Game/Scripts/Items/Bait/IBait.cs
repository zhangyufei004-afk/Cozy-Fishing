using FishingGame.FishSystem;
using FishingGame.Reeling;
using UnityEngine;

namespace FishingGame.Items
{
    public interface IBait
    {
        public void ApplyBait();

        public void UseBaitCharge();

        public FishScriptableObject BaitCatchBehaviour(ReelingInitiation initationScript);

        public void BaitMinigameBehaviour();
    }
}
 