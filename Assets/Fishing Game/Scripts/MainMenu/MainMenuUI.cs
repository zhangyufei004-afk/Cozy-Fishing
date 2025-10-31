using UnityEngine;
using UnityEngine.EventSystems;

namespace FishingGame.MainMenu
{
    /// <summary>
    /// Controls the main menu UI, including navigation to game, settings, and quit functions.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject settingsPanel; // Assign SettingsCanvas in Inspector
        [SerializeField] private GameObject mainMenuButtons; // Assign MainMenuCanvas in Inspector
        [SerializeField] private GameObject creditsPanel;

        public void OnOpenSettings()
        {
            if (settingsPanel != null && mainMenuButtons != null)
            {
                settingsPanel.SetActive(true);
            }
        }

        public void OnCloseSettings()
        {
            if (settingsPanel != null && mainMenuButtons != null)
            {
                settingsPanel.SetActive(false);
            }
            EventSystem.current.SetSelectedGameObject(mainMenuButtons.transform.GetChild(0).gameObject);
        }

        public void OnQuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void ShowCredits()
        {
            if (creditsPanel is not null)
            {
                creditsPanel.SetActive(true);
            }
        }

        public void CloseCredits()
        {
            if (creditsPanel is not null)
            {
                creditsPanel.SetActive(false);
                EventSystem.current.SetSelectedGameObject(mainMenuButtons.transform.GetChild(0).gameObject);
            }
        }
    }
}

