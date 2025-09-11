using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace FishingGame.UI
{
    /// <summary>
    /// Handles resolution and fullscreen/window settings via Slider instead of Dropdown.
    /// Supports a Return button to go back to the previous UI.
    /// Works for both main menu and in-game settings.
    /// Ensures controller navigation works by auto-selecting a starting UI element.
    /// </summary>
    public class SettingsManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Slider resolutionSlider;
        [SerializeField] private TMP_Text resolutionLabel;
        [SerializeField] private TMP_Dropdown displayModeDropdown;
        [SerializeField] private Button returnButton;

        [Header("Controller Navigation")]
        [SerializeField] private Selectable firstSelected;

        private Resolution[] _availableResolutions;

        private void OnEnable()
        {
            SetupResolutions();
            SetupDisplayMode();
            SetupReturnButton();
            SetInitialSelection();
        }

        private void SetupResolutions()
        {
            _availableResolutions = Screen.resolutions;

            int currentResolutionIndex = 0;
            for (int i = 0; i < _availableResolutions.Length; i++)
            {
                if (_availableResolutions[i].width == Screen.currentResolution.width &&
                    _availableResolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                    break;
                }
            }

            resolutionSlider.minValue = 0;
            resolutionSlider.maxValue = _availableResolutions.Length - 1;
            resolutionSlider.wholeNumbers = true;
            resolutionSlider.value = currentResolutionIndex;

            UpdateResolutionLabel(currentResolutionIndex);

            resolutionSlider.onValueChanged.RemoveAllListeners();
            resolutionSlider.onValueChanged.AddListener(index =>
            {
                SetResolution((int)index);
                UpdateResolutionLabel((int)index);
            });
        }

        private void SetupDisplayMode()
        {
            displayModeDropdown.ClearOptions();
            displayModeDropdown.AddOptions(new List<string> { "Fullscreen", "Windowed" });
            displayModeDropdown.value = Screen.fullScreen ? 0 : 1;
            displayModeDropdown.RefreshShownValue();

            displayModeDropdown.onValueChanged.RemoveAllListeners();
            displayModeDropdown.onValueChanged.AddListener(SetDisplayMode);
        }

        private void SetupReturnButton()
        {
            returnButton.onClick.RemoveAllListeners();
            returnButton.onClick.AddListener(ReturnToPreviousUI);
        }

        private void UpdateResolutionLabel(int index)
        {
            Resolution res = _availableResolutions[index];
            resolutionLabel.text = $"{res.width} x {res.height}";
        }

        private void SetResolution(int index)
        {
            Resolution res = _availableResolutions[index];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        }

        private void SetDisplayMode(int index)
        {
            Screen.fullScreen = (index == 0);
        }

        /// <summary>
        /// Hides the settings menu UI instead of changing scenes.
        /// </summary>
        private void ReturnToPreviousUI()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Ensure a UI element is selected so controller navigation works immediately.
        /// </summary>
        private void SetInitialSelection()
        {
            if (EventSystem.current != null)
            {
                GameObject target = null;

                if (firstSelected != null)
                    target = firstSelected.gameObject;
                else if (resolutionSlider != null)
                    target = resolutionSlider.gameObject;
                else if (displayModeDropdown != null)
                    target = displayModeDropdown.gameObject;
                else if (returnButton != null)
                    target = returnButton.gameObject;

                if (target != null)
                    EventSystem.current.SetSelectedGameObject(target);
            }
        }
    }
}
