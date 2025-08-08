using UnityEngine;
using UnityEngine.Serialization;

namespace FishingGame.SaveGame
{
    /// <summary>
    /// A serialized version of a scriptable object for easy writing to JSON.
    /// </summary>
    [System.Serializable]
    public class SerializableObject : ScriptableObject
    {
        // Internal so we can set it in SaveGame namespace
        [SerializeField] protected int persistentID;
        
        // Getter only Property to ensure data integrity
        public int PersistentID => persistentID;

        internal SerializableObject(int persistentID)
        {
            this.persistentID = persistentID;
        }
    }
}
