using System;

namespace FishingGame.GameManagement
{
    public class GameEvents
    {
        #region Inventory Events

        public event Action OnFishCaught;
        public event Action OnItemReceived;

        #endregion

        public void FishCaught()
        {
            OnFishCaught?.Invoke();
        }
    }
}