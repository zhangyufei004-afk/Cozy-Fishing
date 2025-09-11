using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace FishingGame.MainMenu
{
    /// <summary>
    /// Handles the in-game pause menu UI, triggered by button or input action.
    /// </summary>
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField] private InputActionReference pauseAction;
        [SerializeField] private GameObject pauseMenuUI;
        [SerializeField] private GameObject settingsMenuUI;

        private bool _isPaused;

        private void OnEnable()
        {
            pauseAction.action.performed += OnPausePressed;
            pauseAction.action.Enable();
        }

        private void OnDisable()
        {
            pauseAction.action.performed -= OnPausePressed;
            pauseAction.action.Disable();
        }

        private void OnPausePressed(InputAction.CallbackContext context)
        {
            TogglePause();
        }

        public void TogglePause()
        {
            if (_isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        public void ResumeGame()
        {
            pauseMenuUI.SetActive(false);
            settingsMenuUI.SetActive(false);
            Time.timeScale = 1f;
            _isPaused = false;
            ResetEventSystemSelection();
        }

        public void PauseGame()
        {
            pauseMenuUI.SetActive(true);
            settingsMenuUI.SetActive(false);
            Time.timeScale = 0f;
            _isPaused = true;
            SetResumeButtonSelected();
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        
        /// <summary>
        /// Switch to another scene. 
        /// </summary>
        /// <param name="sceneName">The scene name to switch to.</param>
        public void JumpToScene(string sceneName)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// Open the settings menu from the pause menu.
        /// </summary>
        public void OpenSettings()
        {
            pauseMenuUI.SetActive(false);
            settingsMenuUI.SetActive(true);
        }

        /// <summary>
        /// Close the settings menu from the pause menu. 
        /// </summary>
        public void CloseSettings()
        {
            settingsMenuUI.SetActive(false);
            pauseMenuUI.SetActive(true);
            SetResumeButtonSelected();
        }
        
        private void SetResumeButtonSelected()
        {
            if (pauseMenuUI.transform.childCount > 0)
            {
                EventSystem.current.SetSelectedGameObject(pauseMenuUI.transform.GetChild(0).gameObject);
            }
        }

        private void ResetEventSystemSelection()
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
