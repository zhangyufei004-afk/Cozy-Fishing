using FishingGame.SaveGame;
using System.Collections.Generic;
using UnityEngine;

namespace FishingGame.Reeling
{
    [CreateAssetMenu(fileName = "NewSliderWave", menuName = "Fishing Game/Minigames/SliderBehaviour")]
    public class SliderSO : SerializableObject
    {
        [Tooltip("A list that contains all the locations this fish will move to during the minigame")]
        public List<SliderBehaviour> SliderBehaviourList;

        [Tooltip("The max amount of points needed to pass")]
        public float PointsNeededToPass;

        [Tooltip("The default amount of points given per second that the player has the catchbox over the fish")]
        public float PointsPerSecond;




        internal SliderSO(int persistentID) : base(persistentID)
        {

        }
    }

}
