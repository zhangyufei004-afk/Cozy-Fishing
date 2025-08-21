using System;
using System.Collections.Generic;
using FishingGame.Inventory;

namespace FishingGame.GameManagement
{
    public class GameEvents
    {
        #region Inventory Events

        public event Action OnFishCaught;
        public event Action OnItemReceived;
        public event Action<List<IStorable>> OnInventoryUpdated;

        #endregion

        public void FishCaught()
        {
            OnFishCaught?.Invoke();
        }

        public void InventoryUpdated(List<IStorable> itemList)
        {
            OnInventoryUpdated?.Invoke(itemList);
        }
}
}