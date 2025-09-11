using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace FishingGame.MainMenu
{
    /// <summary>
    /// Controls the main menu UI, including navigation to game, settings, and quit functions.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject settingsPanel; // Assign SettingsCanvas in Inspector
        [SerializeField] private GameObject mainMenuPanel; // Assign MainMenuCanvas in Inspector

        public void OnStartGame()
        {
            SceneManager.LoadScene("PlayerSetting");
        }

        public void OnOpenSettings()
        {
            if (settingsPanel != null && mainMenuPanel != null)
            {
                settingsPanel.SetActive(true);
                mainMenuPanel.SetActive(false);
            }
        }

        public void OnCloseSettings()
        {
            if (settingsPanel != null && mainMenuPanel != null)
            {
                settingsPanel.SetActive(false);
                mainMenuPanel.SetActive(true);
            }
            EventSystem.current.SetSelectedGameObject(mainMenuPanel.transform.GetChild(0).gameObject);
        }

        public void OnQuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}

