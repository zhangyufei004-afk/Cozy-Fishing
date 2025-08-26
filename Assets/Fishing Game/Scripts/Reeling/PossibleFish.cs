using FishingGame.FishSystem;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace FishingGame
{
    public class PossibleFish : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("A list of all potential fish in this level")]
        private List<FishScriptableObject> potentialFishTypes;

        public List<FishScriptableObject> GetPossibleFishList()
        {
            return potentialFishTypes;
        }
    }
}
