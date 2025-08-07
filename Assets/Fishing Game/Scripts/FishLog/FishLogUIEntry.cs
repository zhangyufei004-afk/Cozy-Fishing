using UnityEngine;
using UnityEngine.UI;

namespace FishingGame.FishLog
{
    /// <summary>
    /// UI Entry for displaying a fish icon in the Fish Log.
    /// </summary>
    public class FishLogUIEntry : MonoBehaviour
    {
        [SerializeField] private Image _fishImage;
        [SerializeField] private FishScriptableObject _fishData;

        /// <summary>
        /// Fish data corresponding to the entry
        /// </summary>
        public FishScriptableObject GetFishData() => _fishData;

        /// <summary>
        /// Set icon color based on whether it has been captured or not
        /// </summary>
        /// <param name="caught">has been captured or not</param>
        public void MarkAsCaught(bool caught)
        {
            _fishImage.color = caught ? Color.white : Color.black;
        }
    }
}
