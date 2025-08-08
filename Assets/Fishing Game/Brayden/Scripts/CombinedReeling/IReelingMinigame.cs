using UnityEngine;

namespace PrototypeFishingMechanics
{
    public interface IReelingMinigame
    {
        public void InitializeMiniGame();

        public void BeginMiniGame();

        public void WinMiniGame();

        public void LoseMiniGame();
    }
}
