using FishingGame.FishSystem;
using UnityEngine;

namespace FishingGame.Reeling
{
    /// <summary>
    /// This interface is required for all minigames.
    /// It contains 4 key functons that are referenced by the reeling master script
    /// InitializeMiniGame which takes the fish difficulty will set up required variables
    /// BeginMiniGame will begin the current minigames play
    /// WinMiniGame will end the minigame and tell the ReelingMaster it was succsesful
    /// LoseMiniGame will end the minigame and tell the ReelingMaster it was a fail
    /// </summary>
    public interface IReelingMinigame
    {
        /// <summary>
        /// Setsup required minigame variables
        /// Takes the difficulty of the fish
        /// Variables can be modified based on the difficulty of fish
        /// </summary>
        /// /// <param name="fishScriptable">The data of the fish being caught</param>
        public void InitializeMiniGame(Fish fishScriptable);

        /// <summary>
        /// Begins the currently selected minigame
        /// </summary>
        public void BeginMiniGame();

        /// <summary>
        /// Wins the minigame and tells the ReelingMaster it was a win
        /// </summary>
        public void WinMiniGame();

        /// <summary>
        /// Loses the minigame and tells the ReelingMaster it was a loss
        /// </summary>
        public void LoseMiniGame();

        public void UpdateTimer();
    }
}
