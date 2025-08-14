using UnityEngine;

namespace PrototypeFishingMechanics
{
    /// <summary>
    /// Fishing pools contain a type of fish scriptable object that is fished from them
    /// Player can cast lines into these to begin fishing
    /// These can randomly spawn and have a max amount of fish that can be caught from them
    /// These are designed to be easier to catch from compared to catching and individual swimming fish
    /// </summary>
    public class FishingPool : MonoBehaviour
    {
        #region Public Parameters



        #endregion



        #region Private Properties
        [SerializeField]
        private FishScritableObject typeOfFishCaught;

        #endregion
    }
}
