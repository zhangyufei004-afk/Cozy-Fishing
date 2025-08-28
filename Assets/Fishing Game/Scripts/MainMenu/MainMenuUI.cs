using UnityEngine;
using UnityEngine.SceneManagement;

namespace FishingGame.MainMenu
{
    /// <summary>
    /// Controls the main menu UI, including navigation to game, settings, quit functions.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject settingsPanel;

        public void OnStartGame()
        {
            SceneManager.LoadScene("PlayerSetting");
        }

        public void OnOpenSettings()
        {
            settingsPanel.SetActive(true);
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
