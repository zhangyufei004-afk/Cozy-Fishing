using System;
using FishingGame.GameManagement;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingGame.Fishing_Game.Scripts.UI
{
    /// <summary>
    /// Class to display an interact prompt for the player when they enter a trigger. This lets the player know there is an interactable nearby.
    /// </summary>
    public class DisplayInteractPrompt : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI interactPromptText;

        [Tooltip("The text to display on the interact prompt after 'Press E to '")] [SerializeField]
        private string textToDisplay;

        private bool canShowPrompt = true;

        private void OnEnable()
        {
            GameManager.Instance.GameEvents.OnBecomeOccupied += PlayerOccupied;
        }

        private void OnDisable()
        {
            GameManager.Instance.GameEvents.OnBecomeOccupied -= PlayerOccupied;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                EnableInteractText();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                DisableInteractText();
            }
        }

        private void DisableInteractText()
        {
            interactPromptText.gameObject.SetActive(false);
        }

        private void EnableInteractText()
        {
            if (canShowPrompt)
            {
                interactPromptText.gameObject.SetActive(true);
                string interactButtonText = "E";
                if (GameManager.Instance.GetCurrentControlScheme() is Gamepad)
                {
                    interactButtonText = "the interact button";
                }

                interactPromptText.text = $"Press {interactButtonText} to " + textToDisplay;
            }
        }

        private void PlayerOccupied(bool isOccupied)
        {
            if (isOccupied)
            {
                canShowPrompt = false;
                DisableInteractText();
            }
            else
            {
                canShowPrompt = true;
            }
        }
    }
}