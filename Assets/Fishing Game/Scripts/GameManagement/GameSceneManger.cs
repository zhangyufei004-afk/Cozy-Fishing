using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace FishingGame.Fishing_Game.Scripts.GameManagement
{
    /// <summary>
    /// Scene Manager Class. Stores the references to the Loading Screen and has methods to load scenes with the loading screen.
    /// The class is a singleton, and therefore can be accessed anywhere in code.
    /// It must first be placed into the unity scene to operate.
    /// </summary>
    public class GameSceneManger : MonoBehaviour
    {
        public GameSceneManger Instance => _instance;
        
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private TextMeshProUGUI loadingScreenText;
        private CanvasGroup _loadingScreenCanvasGroup;
        private GameSceneManger _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            _instance = this;
        }

        private void Start()
        {
            _loadingScreenCanvasGroup = loadingScreen.GetComponent<CanvasGroup>();
            if (loadingScreen.activeSelf)
            {
                FadeInToScene();
            }
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
        
        /// <summary>
        /// Loads the scene <c>sceneName</c> in the background of displaying the loading screen. 
        /// </summary>
        /// <param name="sceneName">The scene to load</param>
        public void LoadSceneWithLoadingScreen(string sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }
        
        private void FadeInToScene()
        {
            StartCoroutine(FadeToScene());
        }

        IEnumerator FadeToScene()
        {
            yield return new WaitForSeconds(0.25f);
            while (_loadingScreenCanvasGroup.alpha > 0.1f)
            {
                _loadingScreenCanvasGroup.alpha = Mathf.Lerp(_loadingScreenCanvasGroup.alpha, 0f, Time.deltaTime * 5f);
                yield return null;
            }
            loadingScreen.SetActive(false);
        }

        IEnumerator FadeToLoadingScreen()
        {
            loadingScreen.SetActive(true);
            loadingScreenText.text = "Loading 0%";
            while (_loadingScreenCanvasGroup.alpha < 0.95f)
            {
                _loadingScreenCanvasGroup.alpha = Mathf.Lerp(_loadingScreenCanvasGroup.alpha, 1f, Time.deltaTime * 5f);
                yield return null;
            }
            _loadingScreenCanvasGroup.alpha = 1;
            yield return new WaitForSeconds(0.25f);

        }

        IEnumerator LoadSceneAsync(string sceneName)
        {
            yield return FadeToLoadingScreen();
            AsyncOperation sceneLoadOperation = SceneManager.LoadSceneAsync(sceneName);
            if (sceneLoadOperation == null) yield break;

            while (sceneLoadOperation is { isDone: false })
            {
                yield return null;
                loadingScreenText.text = $"Loading {sceneLoadOperation.progress}%";
            }
        }
    }
}