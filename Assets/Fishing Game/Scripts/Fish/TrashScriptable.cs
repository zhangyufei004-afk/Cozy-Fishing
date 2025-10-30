using UnityEngine;

namespace FishingGame.FishSystem
{
    /// <summary>
    /// The trash scriptable object that is used to create trash items
    /// </summary>
    [CreateAssetMenu(fileName = "NewTrash", menuName = "Fishing Game/Trash Data")]
    public class TrashScriptable : FishableScriptable
    {
        
        internal TrashScriptable(int persistentID) : base(persistentID)
        {
        }
    }
}
