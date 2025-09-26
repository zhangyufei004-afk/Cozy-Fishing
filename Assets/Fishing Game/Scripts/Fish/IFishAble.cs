using FishingGame.SaveGame;
using System;
using UnityEngine;

namespace FishingGame.FishSystem
{
    /// <summary>
    /// An enum that represents what type of fishable item this is
    /// </summary>
    public enum ECatchableType
    {
        None,
        Fish,
        Trash
    }

    public interface IFishAble
    {
        /// <summary>
        /// Returns the difficulty of this catchable object
        /// </summary>
        /// <returns>The difficulty value as an int</returns>
        public int GetCatchDifficulty();

        /// <summary>
        /// Returns the name of this object
        /// </summary>
        /// <returns>The name as a string</returns>
        public String GetName();

        /// <summary>
        /// Returns the texture used for this fishable item
        /// </summary>
        /// <returns>The texture used for this item</returns>
        public Sprite GetTexture();

        /// <summary>
        /// Returns the weight of object
        /// </summary>
        /// <returns>The weight of this object</returns>
        public float GetWeight();

        /// <summary>
        /// Returns what type of fishable object this is
        /// </summary>
        /// <returns>Enum value represneting the catch type</returns>
        public ECatchableType GetCatchType();

        /// <summary>
        /// Get the data object for the item. The Data Object is a ScriptableObject, which can be Serailzied. It stores static
        /// non-runtime data. 
        /// </summary>
        /// <param name="dataClassType">Output parameter to give the type of the DataObject, for casting correctness. </param>
        /// <returns>The SerializableObject that the Data is stored in.</returns>
        public SerializableObject GetDataObject(out Type dataClassType);
    }
}
