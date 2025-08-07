using UnityEngine;

namespace PrototypeFishingMechanics
{
    public class SliderMiniGame : MonoBehaviour, IReelingMinigame
    {
        public void InitializeMiniGame() 
        {
            SliderCanvas.SetActive(true);
        }

        public void BeginMiniGame()
        {

        }

        public void EndMiniGame() 
        {

        }

        public GameObject SliderCanvas;

    }
}
