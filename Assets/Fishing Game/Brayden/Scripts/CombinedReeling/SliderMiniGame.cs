using System.Collections;
using UnityEngine;

namespace PrototypeFishingMechanics
{
    public class SliderMiniGame : MonoBehaviour, IReelingMinigame
    {
        [SerializeField]
        [Tooltip("Reference to the ReelingMaster script.")]
        private ReelingMaster ReelingMaster;



        public void InitializeMiniGame() 
        {
            SliderCanvas.SetActive(true);
        }

        public void BeginMiniGame()
        {
            StartCoroutine(TempTimeForWin());
        }

        public GameObject SliderCanvas;

        // TEMP TESTING COROTINE WHILE THE MINIGAME DOSENT WORK ITSELF

        IEnumerator TempTimeForWin()
        {
            yield return new WaitForSeconds(4);
            WinMiniGame();
        }

        public void WinMiniGame()
        {
            SliderCanvas.SetActive(false);
            ReelingMaster.EndCurrentMiniGame(true);
        }

        public void LoseMiniGame()
        {
            throw new System.NotImplementedException();
        }
    }
}
