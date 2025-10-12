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

        private void OnEnable()
        {
            GameManager.Instance.GameEvents.OnWithinItemPickupRange += SetInteractionText;
            GameManager.Instance.GameEvents.OnShowStatusText += SetStatusText;
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
        private void SetStatusText(string textToSet, float timeToShowFor)
        {
            reelingThrowLineText.text = textToSet;
            reelingThrowLineText.gameObject.SetActive(true);
            reelingThrowLineText.GetComponent<Animator>().SetTrigger("TextIsActive");
            StartCoroutine(HideStatusText(timeToShowFor));
        }

        /// <summary>
        /// Timer that hides the status text UI after inputed seconds
        /// </summary>
        /// <param name="timeUntilHide">How long until this text should be hidden</param>
        /// <returns>Hides the status text</returns>
        private IEnumerator HideStatusText(float timeUntilHide)
        {
            yield return new WaitForSeconds(timeUntilHide);
            reelingThrowLineText.gameObject.SetActive(false);
        }





    }
}
