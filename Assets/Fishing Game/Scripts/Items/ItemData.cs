using UnityEngine;

namespace FishingGame
{
    public class ItemData
    {
        private ItemScriptable _itemBase;


        public ItemData(ItemScriptable itemScriptable)
        {
            _itemBase = itemScriptable;
        }

        public string GetItemName()
        {
            return _itemBase.ItemName;
        }


    }
}
