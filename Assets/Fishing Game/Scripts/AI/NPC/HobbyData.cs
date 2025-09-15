using FishingGame.SaveGame;
using UnityEditor;
using UnityEngine;

namespace FishingGame.AI.NPC
{
    /// <summary>
    /// Scriptable Object Class to represent a Hobby. 
    /// <list type="bullet">
    ///     <listheader>
    ///         <term>This Data class stores common attributes among all hobbies like: </term>
    ///     </listheader>
    ///     <item>
    ///         <description>The hobby location as a Vector 3 in World Space</description>
    ///     </item>
    ///     <item>
    ///         <description>The animation to play while at the hobby location</description>
    ///     </item>
    ///     <item>
    ///         <description>The direction to face while engaging in the hobby.</description>
    ///     </item>
    ///     <item>
    ///         <description>The quip (if any) to display to the player if they try to interact while you are at the hobby.</description>
    ///     </item>
    /// </list>
    /// </summary>
    [CreateAssetMenu(fileName = "NewHobby", menuName = "Fishing Game/Hobby")]
    public class HobbyData : SerializableObject
    {
        internal HobbyData(int persistentID) : base(persistentID)
        {
        }

        public Vector3 HobbyLocation => hobbyLocation;
        public string HobbyAnimationBooleanName => hobbyAnimationBooleanName;
        public Quaternion HobbyRotation => hobbyRotation;
        public string HobbyQuip => hobbyQuip;
        
        
        [Header("Hobby Properties")]
        [Tooltip("The location in world space that the hobby is located at.")]
        [SerializeField] private Vector3 hobbyLocation;
        
        [Tooltip("The animator variable which triggers the animation for the hobby.")]
        [SerializeField] private string hobbyAnimationBooleanName;
        
        [Tooltip("The direction to face when engaging in the hobby.")]
        [SerializeField] private Quaternion hobbyRotation;
        
        [Tooltip("The string quip to display to the player if they interact while the NPC is in the hobby.")]
        [SerializeField] private string hobbyQuip;
    }
}
