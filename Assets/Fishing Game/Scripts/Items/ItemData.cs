using FishingGame.Inventory;
using FishingGame.SaveGame;
using System;
using UnityEngine;

namespace FishingGame
{
    public class ItemData: IStorable
    {
        private ItemScriptable _itemBase;
        private EItemType _itemType;
        private Sprite _itemSprite;
        private float _itemWeight;
        private string _itemName;

        public ItemData(ItemScriptable itemScriptable)
        {
            _itemBase = itemScriptable;
            _itemType = _itemBase.ItemType;
            _itemSprite = _itemBase.Item2DTexture;
            _itemWeight = _itemBase.ItemWeight;
            _itemName = _itemBase.ItemName;
        }

        public SerializableObject GetDataObject(out Type dataClassType)
        {
            throw new NotImplementedException();
        }

        public ItemScriptable GetItemBase()
        {
            return _itemBase;
        }

        public string GetItemName()
        {
            return _itemName;
        }

        public EItemType GetItemType()
        {
            return _itemType;
        }

        public Sprite GetTexture()
        {
            return _itemSprite;
        }

        public float GetWeight()
        {
            return _itemWeight;
        }
    }
}
