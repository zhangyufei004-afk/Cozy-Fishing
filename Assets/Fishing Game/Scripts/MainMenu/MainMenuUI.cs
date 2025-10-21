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
        [SerializeField] private GameObject mainMenuButtons; // Assign MainMenuCanvas in Inspector

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
    }
}

