using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace FishingGame.UI
{
    /// <summary>
    /// Handles resolution and fullscreen/window settings via Dropdowns.
    /// Supports a Return button to go back to the previous UI.
    /// Works for both main menu and in-game settings.
    /// </summary>
    public class SettingsManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private TMP_Dropdown displayModeDropdown;
        [SerializeField] private Button returnButton;

        private Resolution[] _availableResolutions;
        private List<string> _resolutionOptions = new List<string>();

        private static string LastSceneName;
        private static bool IsInGame; // true if settings opened in game

        private void Start()
        {
            SetupResolutions();
            SetupDisplayMode();
            SetupReturnButton();
        }

        private void SetupResolutions()
        {
            _availableResolutions = Screen.resolutions;
            _resolutionOptions.Clear();
            int currentResolutionIndex = 0;

            for (int i = 0; i < _availableResolutions.Length; i++)
            {
                Resolution res = _availableResolutions[i];
                string option = $"{res.width} x {res.height}";
                _resolutionOptions.Add(option);

                if (res.width == Screen.currentResolution.width &&
                    res.height == Screen.currentResolution.height)
                    currentResolutionIndex = i;
            }

            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(_resolutionOptions);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();

            resolutionDropdown.onValueChanged.AddListener(SetResolution);
        }

        private void SetupDisplayMode()
        {
            displayModeDropdown.ClearOptions();
            displayModeDropdown.AddOptions(new List<string> { "Fullscreen", "Windowed" });
            displayModeDropdown.value = Screen.fullScreen ? 0 : 1;
            displayModeDropdown.RefreshShownValue();

            displayModeDropdown.onValueChanged.AddListener(SetDisplayMode);
        }

        private void SetupReturnButton()
        {
            if (returnButton != null)
            {
                returnButton.onClick.RemoveAllListeners();
                returnButton.onClick.AddListener(ReturnToPreviousUI);
            }
        }

        private void SetResolution(int index)
        {
            if (index < 0 || index >= _availableResolutions.Length) return;
            Resolution res = _availableResolutions[index];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        }

        private void SetDisplayMode(int index)
        {
            switch (index)
            {
                case 0: Screen.fullScreen = true; break;
                case 1: Screen.fullScreen = false; break;
            }
        }

        private void ReturnToPreviousUI()
        {
            if (string.IsNullOrEmpty(LastSceneName)) return;

            if (IsInGame)
            {
                // Unload settings scene, return to game
                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
                Time.timeScale = 1f; // resume game
            }
            else
            {
                // Reload main menu
                SceneManager.LoadScene(LastSceneName);
            }
        }

        public static void SetLastScene(string sceneName, bool isInGame)
        {
            LastSceneName = sceneName;
            IsInGame = isInGame;
        }
    }
}

