using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FishingGame.Reeling
{
    public class ArrowSpawner : MonoBehaviour
    {
        [Header("Script references")]

        [SerializeField]
        [Tooltip("The MinigameMasterScript")]
        private ArrowMiniGameMaster masterScript;

        private bool _isActive = false;
        private int _currentDifficultyLevel;
        private GameObject _arrowToSpawn;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            SpawnArrow();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        /// <summary>
        /// Spawns a new arrow, runs the required functions on the new arrow for proper initilization
        /// </summary>
        public void SpawnArrow()
        {
            GameObject currentArrow = Instantiate(_arrowToSpawn, gameObject.transform);
            masterScript.AddOrRemoveActiveArrow(currentArrow.GetComponent<MovingArrow>(), true);
            currentArrow.GetComponent<MovingArrow>().ActivateArrow(50);
            currentArrow.GetComponent<MovingArrow>().SetSpawner(this);
        }

        /// <summary>
        /// Sets this spawners required variables for it to function,
        /// this should be run everytime the minigame is initiated
        /// </summary>
        /// <param name="difficultyLevel">The difficulty level to set</param>
        /// <param name="arrowPrefab">The arrow prefab that will be spawned</param>
        public void InitiateSpawners(int difficultyLevel, GameObject arrowPrefab)
        {
            _currentDifficultyLevel = difficultyLevel;
            _arrowToSpawn = arrowPrefab;
        }

        /// <summary>
        /// Activates or deactivates the spawned based on inputed parameter
        /// True will activate the spawned, false will turn it off
        /// </summary>
        /// <param name="isActive">True will activate the spawned, false will turn it off</param>
        public void ActivateOrDeactivateSpawner(bool isActive)
        {
            if (isActive) { _isActive = true; }
            else { _isActive = false; }
        }
    }
}
