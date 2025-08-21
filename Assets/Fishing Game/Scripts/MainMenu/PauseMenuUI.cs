using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace FishingGame.MainMenu
{
    /// <summary>
    /// Handles the in-game pause menu UI, triggered by button or input action.
    /// </summary>
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField] private InputActionReference pauseAction;
        [SerializeField] private GameObject pauseMenuUI;
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

        /// <summary>
        /// Can be called from both button and input action.
        /// </summary>
        public void TogglePause()
        {
            if (_isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        public void ResumeGame()
        {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            _isPaused = false;
            ResetEventSystemSelection();
        }

        public void PauseGame()
        {
            pauseMenuUI.SetActive(true);
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

        private void SetResumeButtonSelected()
        {
            EventSystem.current.SetSelectedGameObject(pauseMenuUI.transform.GetChild(0).gameObject);
        }

        private void ResetEventSystemSelection()
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
