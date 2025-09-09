using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

namespace FishingGame.UI
{
    /// <summary>
    /// Handles button-triggered scene transitions with fade in/out effects.
    /// Attach this to a UI Button and assign the FadeImage.
    /// </summary>
    public class UIButtonSceneFader : MonoBehaviour
    {
        [SerializeField] private string targetSceneName;
        [SerializeField] private Image fadeImage;
        [SerializeField] private float fadeDuration = 1f;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            if (_button == null || fadeImage == null)
            {
                return; 
            }

            // Ensure the fade image is transparent at start
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;

            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            if (string.IsNullOrEmpty(targetSceneName))
            {
                return; 
            }

            StartCoroutine(FadeAndLoadScene());
        }

        private IEnumerator FadeAndLoadScene()
        {
            // Fade out
            yield return StartCoroutine(Fade(0f, 1f));

            // Load new scene
            SceneManager.LoadScene(targetSceneName);

            // Wait one frame for scene to load
            yield return null;

            // Fade in
            yield return StartCoroutine(Fade(1f, 0f));
        }

        private IEnumerator Fade(float startAlpha, float endAlpha)
        {
            float elapsed = 0f;
            Color c = fadeImage.color;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
                c.a = alpha;
                fadeImage.color = c;
                yield return null;
            }

            c.a = endAlpha;
            fadeImage.color = c;
        }

        private void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(OnButtonClicked);
        }
    }
}
