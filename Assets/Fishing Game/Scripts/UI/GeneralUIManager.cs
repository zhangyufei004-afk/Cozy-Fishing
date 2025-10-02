using FishingGame.GameManagement;
using TMPro;
using UnityEngine;

namespace FishingGame
{
    /// <summary>
    /// This class is responsible for some of the general UI elements that are 
    /// commonly present but not tied to specific things
    /// For example reeling UI elements involved with the minigame or fishing
    /// will be not be controlled by this script, but something like
    /// the interact text would be
    /// </summary>
    public class GeneralUIManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The text that shows what button to press to interact with something")]
        private TextMeshProUGUI interactionText;

        private void OnEnable()
        {
            GameManager.Instance.GameEvents.OnWithinItemPickupRange += SetInteractionText;
        }

        /// <summary>
        /// Sets the interaction text's activeness and text based on inputed parameters
        /// This text is used to display to the player that they are able to interact with a nearby
        /// object
        /// </summary>
        /// <param name="isActive">Is the text active</param>
        /// <param name="textToSet">What should the text say</param>
        public void SetInteractionText(bool isActive, string textToSet)
        {
            interactionText.text = textToSet;
            interactionText.gameObject.SetActive(isActive); 
        }



    }
}
