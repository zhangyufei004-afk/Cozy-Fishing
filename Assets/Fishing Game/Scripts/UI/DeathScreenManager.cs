using System.Collections;
using FishingGame.GameManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FishingGame.UI
{
    /// <summary>
    /// Death Screen Manager Class. Subcribes to the player died event and shows the Players death screen when they die.
    /// Also invokes the DeathScreenActive event in the GameEvents class. 
    /// </summary>
    public class DeathScreenManager : MonoBehaviour
    {
        private const float DeathScreenFadeDuration = 0.5f;
        private const float DeathMessageReadTime = 2.5f;
        
        [SerializeField] private Image deathScreenBackground;
        [SerializeField] private TextMeshProUGUI deathScreenText;
        [SerializeField] private TextMeshProUGUI deathScreenTipText;

        private bool _isRoutineRunning = false;
        private GameManager _gameManager;
        
        private void OnEnable()
        {
            _gameManager = GameManager.Instance;
            _gameManager.GameEvents.OnPlayerDeath += EnableDeathScreen;
            deathScreenBackground.CrossFadeAlpha(0f, 0f, true);
            deathScreenText.CrossFadeAlpha(0f, 0f, true);
            deathScreenTipText.CrossFadeAlpha(0f, 0f, true);
        }

        private void OnDisable()
        {
            _gameManager.GameEvents.OnPlayerDeath -= EnableDeathScreen;
        }

        private void EnableDeathScreen()
        {
            string deathMessage = $"Tip: {_gameManager.GetRandomDeathTip()}";
            if (!_isRoutineRunning)
            {
                StartCoroutine(FadeDeathScreen(deathMessage));
            }
        }

        private IEnumerator FadeDeathScreen(string deathMessage)
        {
            _isRoutineRunning = true;
            deathScreenBackground.gameObject.SetActive(true);
            deathScreenTipText.text = deathMessage;
            deathScreenBackground.CrossFadeAlpha(1f, DeathScreenFadeDuration, true);
            deathScreenText.CrossFadeAlpha(1f, DeathScreenFadeDuration, true);
            deathScreenTipText.CrossFadeAlpha(1f, DeathScreenFadeDuration, true);
            yield return new WaitForSeconds(DeathScreenFadeDuration);
            
            _gameManager.GameEvents.PlayerDeathScreenActive(true);
            yield return new WaitForSeconds(DeathMessageReadTime);
            
            deathScreenText.CrossFadeAlpha(0f, DeathScreenFadeDuration, true);
            deathScreenBackground.CrossFadeAlpha(0f, DeathScreenFadeDuration, true);
            deathScreenTipText.CrossFadeAlpha(0f, DeathScreenFadeDuration, true);
            yield return new WaitForSeconds(DeathScreenFadeDuration);
            
            _gameManager.GameEvents.PlayerDeathScreenActive(false);
            deathScreenBackground.gameObject.SetActive(false);
            _isRoutineRunning = false;
        }
    }
}
