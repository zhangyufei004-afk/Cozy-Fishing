using FishingGame.FishSystem;
using UnityEngine;
using UnityEngine.UI;

namespace FishingGame.Reeling
{
    public class RealisticMiniGameMaster : MonoBehaviour, IReelingMinigame
    {
        [Header("Reeling Script")]

        [SerializeField]
        [Tooltip("Reference to the ReelingMaster script.")]
        private ReelingMaster reelingMaster;

        [Header("UI elements")]

        [SerializeField]
        [Tooltip("The gameobject that holds the UI in it. This is a child of reelingUI.")]
        private GameObject realisticCanvas;

        [SerializeField]
        [Tooltip("The UI slider that shows the progress of the minigame.")]
        private Slider progressSlider;







        public void BeginMiniGame()
        {
            throw new System.NotImplementedException();
        }

        public void InitializeMiniGame(Fish fishScriptable)
        {
            throw new System.NotImplementedException();
        }

        public void LoseMiniGame()
        {
            throw new System.NotImplementedException();
        }

        public void WinMiniGame()
        {
            throw new System.NotImplementedException();
        }
    }
}
