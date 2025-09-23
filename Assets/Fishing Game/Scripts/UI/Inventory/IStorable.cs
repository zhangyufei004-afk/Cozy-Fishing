using System;
using FishingGame.SaveGame;

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
        /// Gets the item type.
        /// </summary>
        /// <returns>The item type as defined in EItemType</returns>
        public EItemType GetItemType();
        
        /// <summary>
        /// Get the data object for the item. The Data Object is a ScriptableObject, which can be Serailzied. It stores static
        /// non-runtime data. 
        /// </summary>
        /// <param name="dataClassType">Output parameter to give the type of the DataObject, for casting correctness. </param>
        /// <returns>The SerializableObject that the Data is stored in.</returns>
        public SerializableObject GetDataObject(out Type dataClassType);
    }
}