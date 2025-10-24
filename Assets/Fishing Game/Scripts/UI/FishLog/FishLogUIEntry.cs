using FishingGame.FishSystem;
using FishingGame.GameManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FishingGame.FishLog
{
    /// <summary>
    /// UI Entry for displaying a fish icon in the Fish Log.
    /// </summary>
    public class FishLogUIEntry : MonoBehaviour
    {
        [SerializeField] private Image fishImage;
        [SerializeField] private TMP_Text fishName;
        [SerializeField] private FishScriptableObject fishData;

        [Tooltip("The parent of this object")]
        [SerializeField] private FishLogUI logUIController;

        

        /// <summary>
        /// Fish data corresponding to the entry
        /// </summary>
        public FishScriptableObject GetFishData() => fishData;

        /// <summary>
        /// Set icon color based on whether it has been captured or not
        /// </summary>
        /// <param name="caught">has been captured or not</param>
        public void MarkAsCaught(bool caught)
        {
            fishImage.color = caught ? Color.white : Color.black;
            fishName.text = caught ? fishData.Name : "???";
        }


        // For DEBUG purposes only
        [ContextMenu("Show")]
        public void Show()
        {
            MarkAsCaught(true);
        }
        [ContextMenu("Hide")]
        public void Hide()
        {
            MarkAsCaught(false);
        }

        /// <summary>
        /// Run when this UI element is pressed, this will cause the fish log UI to display the clicked fish
        /// </summary>
        public void OnClick()
        {
            logUIController.FishEntryClicked(fishData);
        }
    }
}
