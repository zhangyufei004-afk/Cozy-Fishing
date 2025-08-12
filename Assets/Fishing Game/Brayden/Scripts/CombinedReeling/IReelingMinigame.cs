using UnityEngine;

namespace PrototypeFishingMechanics
{
    public interface IReelingMinigame
    {
        public void InitializeMiniGame(int fishCatchDifficulty);

        public void BeginMiniGame();

        public void WinMiniGame();

        public void LoseMiniGame();
    }
}
