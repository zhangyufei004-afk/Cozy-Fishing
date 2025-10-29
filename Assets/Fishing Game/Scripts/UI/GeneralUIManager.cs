using FishingGame.GameManagement;
using System.Collections;
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

        [SerializeField]
        [Tooltip("The text that shows statuses")]
        private TextMeshProUGUI reelingThrowLineText;

        [SerializeField]
        [Tooltip("The text that shows default notifications")]
        private TextMeshProUGUI defaultNotificationText;

        private void OnEnable()
        {
            GameManager.Instance.GameEvents.OnWithinItemPickupRange += SetInteractionText;
            GameManager.Instance.GameEvents.OnShowStatusText += SetFadeAwayStatusText;
            GameManager.Instance.GameEvents.OnShowDefaultNotificationText += SetDefaultNotificationText;
        }

        /// <summary>
        /// Sets the interaction text's activeness and text based on inputed parameters
        /// This text is used to display to the player that they are able to interact with a nearby
        /// object
        /// </summary>
        /// <param name="isActive">Is the text active</param>
        /// <param name="textToSet">What should the text say</param>
        private void SetInteractionText(bool isActive, string textToSet)
        {
            interactionText.text = textToSet;
            interactionText.gameObject.SetActive(isActive); 
        }

        /// <summary>
        /// Reveals and displays the text used to show status events
        /// Has an inputed string variable which represents what text to set
        /// Float variable represents how long the text should stay active for
        /// </summary>
        /// <param name="textToSet">What the text should display</param>
        /// <param name="timeToShowFor">How long should text be displayed for</param>
        /// <param name="colorToUse">Color to use for the text</param>
        private void SetFadeAwayStatusText(string textToSet, float timeToShowFor, Color colorToUse)
        {
            reelingThrowLineText.text = textToSet;
            reelingThrowLineText.gameObject.SetActive(true);
            reelingThrowLineText.color = colorToUse;
            reelingThrowLineText.GetComponent<Animator>().SetTrigger("TextIsActive");
            StartCoroutine(HideStatusText(timeToShowFor, reelingThrowLineText));
        }

        /// <summary>
        /// Reveals and display the text used to show default notifcations
        /// Has inputed string variable which represents what text to set
        /// Float variable represents how long hte text should stay active for
        /// </summary>
        /// <param name="textToSet">Text to be set</param>
        /// <param name="timeToShowFor">Time to show it for in seconds</param>
        /// <param name="colorToUse">Color to use</param>
        private void SetDefaultNotificationText(string textToSet, float timeToShowFor, Color colorToUse)
        {
            defaultNotificationText.text = textToSet;
            defaultNotificationText.gameObject.SetActive(true);
            defaultNotificationText.color = colorToUse;
            StartCoroutine(HideStatusText(timeToShowFor, defaultNotificationText));
        }

        /// <summary>
        /// Timer that hides the status text UI after inputed seconds
        /// </summary>
        /// <param name="timeUntilHide">How long until this text should be hidden</param>
        /// <returns>Hides the status text</returns>
        private IEnumerator HideStatusText(float timeUntilHide, TextMeshProUGUI textoToHide)
        {
            yield return new WaitForSecondsRealtime(timeUntilHide);
            textoToHide.gameObject.SetActive(false);
        }





    }
}
