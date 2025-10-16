using System;
using FishingGame.Items;
using FishingGame.SaveGame;
using UnityEngine;

namespace FishingGame.Inventory
{
    /// <summary>
    /// Enum to indicate the type of Storable object.
    /// </summary>
    public enum EItemType : byte
    {
        Fish,
        Rod,
        RodAttachment,
        Money,
        Trash
    }
    
    /// <summary>
    /// Interface to define common methods for all Storable Objects
    /// </summary>
    public interface IStorable
    {
        /// <summary>
        /// Gets the weight of the item.
        /// </summary>
        /// <returns>The weight in grams of the item.</returns>
        public float GetWeight();

        /// <summary>
        /// Returns the texture used for this item
        /// </summary>
        /// <returns>The texture used for this item</returns>
        public Sprite GetTexture();

        /// <summary>
        /// Returns the item name
        /// </summary>
        /// <returns>The item name</returns>
        public String GetName();

        /// <summary>
        /// Gets the item type.
        /// </summary>
        /// <returns>The item type as defined in EItemType</returns>
        public EItemType GetItemType();

        /// <summary>
        /// Uses the item
        /// Not every Istoreable object needs to have functionality for this
        /// If a Istoreable item is of a type that can't be used from the inventory
        /// the UI button that runs this function should be hidden
        /// </summary>
        public void UseItem();
        
        /// <summary>
        /// Get the data object for the item. The Data Object is a ScriptableObject, which can be Serailzied. It stores static
        /// non-runtime data. 
        /// </summary>
        /// <param name="dataClassType">Output parameter to give the type of the DataObject, for casting correctness. </param>
        /// <returns>The SerializableObject that the Data is stored in.</returns>
        public abstract SerializableObject GetDataObject(out Type dataClassType);
    }
}