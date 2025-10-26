using FishingGame.Inventory;
using FishingGame.SaveGame;
using System;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

namespace FishingGame.Items
{
    /// <summary>
    /// This is the default item type class
    /// It uses the Istorable class
    /// Required data values are set based on ItemScriptable on construction
    /// </summary>
    public class ItemData : IStorable
    {
        private ItemScriptable _itemBase;
        private EItemType _itemType;
        private Sprite _itemSprite;
        private float _itemWeight;
        private string _itemName;
        private int _itemCharge;
        protected bool _currentlyEquiped = false;
        private string _itemToolTip;

        /// <summary>
        /// This is the default item type class
        /// It uses the Istorable class
        /// Required data values are set based on ItemScriptable on construction
        /// </summary>
        /// <param name="itemScriptable">The scriptable object that data is based on</param>
        public ItemData(ItemScriptable itemScriptable)
        {
            _itemBase = itemScriptable;
            _itemType = _itemBase.ItemType;
            _itemSprite = _itemBase.Item2DTexture;
            _itemWeight = _itemBase.ItemWeight;
            _itemName = _itemBase.ItemName;
            _itemToolTip = _itemBase.ItemToolTip;
        }

        /// <summary>
        /// Get the data object for the item. The Data Object is a ScriptableObject, which can be Serailzied. It stores static
        /// non-runtime data. 
        /// </summary>
        /// <param name="dataClassType">Output parameter to give the type of the DataObject, for casting correctness. </param>
        /// <returns>The SerializableObject that the Data is stored in.</returns>
        public SerializableObject GetDataObject(out Type dataClassType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the item base
        /// </summary>
        /// <returns>The scriptable object that created this</returns>
        public ItemScriptable GetItemBase()
        {
            return _itemBase;
        }

        /// <summary>
        /// Returns the item name
        /// </summary>
        /// <returns>The item name</returns>
        public string GetName()
        {
            return _itemName;
        }

        /// <summary>
        /// Returns the item type enum value
        /// </summary>
        /// <returns>The item type enum value</returns>
        public EItemType GetItemType()
        {
            return _itemType;
        }

        /// <summary>
        /// Returns the 2D sprite of this item
        /// </summary>
        /// <returns>The 2D sprite of this item</returns>
        public Sprite GetTexture()
        {
            return _itemSprite;
        }

        /// <summary>
        /// Returns the weight of this item
        /// </summary>
        /// <returns>The weight of this item</returns>
        public float GetWeight()
        {
            return _itemWeight;
        }

        /// <summary>
        /// Returns the current max charge of this item
        /// </summary>
        /// <returns>The current charge of this item</returns>
        public virtual int GetCurrentUseCharge()
        {
            return _itemCharge;
        }

        /// <summary>
        /// Sets currently equiped to equal true if not currently equiped, otehrwise sets to false
        /// This can be overiden if the specific item should have additional functionality
        /// usually when overriding this function you should still call .base()
        /// </summary>
        public virtual void UseItem()
        {
            if (_currentlyEquiped) { _currentlyEquiped = false; }
            else {  _currentlyEquiped = true; }
        }

        /// <summary>
        /// Sets currently equiped to equal false
        /// </summary>
        public void UnEquipItem()
        {
            _currentlyEquiped = false;
        }

        /// <summary>
        /// Returns true if this item is currently equiped otherwise false
        /// </summary>
        /// <returns>True if item is equiped otherwise false</returns>
        public bool IsCurrentlyEquiped()
        {
            if (_currentlyEquiped) {  return true; }
            else { return false; }
        }

        /// <summary>
        /// Returns the items tooltip
        /// </summary>
        /// <returns>Returns the items tooltip</returns>
        public string GetTooltip()
        {
            return _itemToolTip;
        }
    }
}
