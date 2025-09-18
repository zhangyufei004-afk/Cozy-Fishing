using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingGame.Reeling
{
    public class ArrowSpawner : MonoBehaviour
    {
        [Header("Script references")]

        [SerializeField]
        [Tooltip("The MinigameMasterScript")]
        private ArrowMiniGameMaster masterScript;

        [SerializeField]
        [Tooltip("The type of arrow this should spawn")]
        private EMovementDirection arrowType;



        private bool _isActive = false;
        private int _currentDifficultyLevel;
        private GameObject _arrowToSpawn;
        private ArrowGoalPoints _goalPoint;
        private List<MovingArrow> _activeArrows;


        private void OnEnable()
        {
            _activeArrows = new List<MovingArrow>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        /// <summary>
        /// Spawns a new arrow, runs the required functions on the new arrow for proper initilization
        /// </summary>
        /// <param name="speedToUse">The speed to use</param>
        public void SpawnArrow(float speedToUse)
        {
            if (CheckIsActive() != true) { return; }
            GameObject currentArrow = Instantiate(_arrowToSpawn, gameObject.transform);
            currentArrow.GetComponent<MovingArrow>().SetTypeOfArrow((int)arrowType);
            currentArrow.transform.SetParent(GetArrowGoal().gameObject.transform, true);
            masterScript.AddOrRemoveActiveArrow(currentArrow.GetComponent<MovingArrow>(), true);
            currentArrow.GetComponent<MovingArrow>().ActivateArrow(speedToUse);
            currentArrow.GetComponent<MovingArrow>().SetSpawner(this);
            _activeArrows.Add(currentArrow.GetComponent<MovingArrow>());
        }

        /// <summary>
        /// Sets this spawners required variables for it to function,
        /// this should be run everytime the minigame is initiated
        /// </summary>
        /// <param name="difficultyLevel">The difficulty level to set</param>
        /// <param name="arrowPrefab">The arrow prefab that will be spawned</param>
        /// <param name="goalPoint">The goal point for this spawner</param>
        public void InitiateSpawner(int difficultyLevel, GameObject arrowPrefab, ArrowGoalPoints goalPoint)
        {
            _currentDifficultyLevel = difficultyLevel;
            _arrowToSpawn = arrowPrefab;
            _goalPoint = goalPoint;
        }

        /// <summary>
        /// Activates or deactivates the spawned based on inputed parameter
        /// True will activate the spawned, false will turn it off
        /// </summary>
        /// <param name="isActive">True will activate the spawned, false will turn it off</param>
        public void ActivateOrDeactivateSpawner(bool isActive)
        {
            _isActive = isActive;
        }

        public void PunishPoorPress()
        {
            if (_activeArrows.Count != 0)
            {
                _activeArrows[0].ArrowFailed();
            }
        }

        public void RemoveActiveArrow(MovingArrow arrowToRemove)
        {
            _activeArrows.Remove(arrowToRemove);
        }

        /// <summary>
        /// Returns the goalpoint of this spawner
        /// </summary>
        /// <returns>The goalpoint of this spawner</returns>
        public ArrowGoalPoints GetArrowGoal()
        { 
            return _goalPoint;
        }

        public int GetSpawnerTypeAsInt()
        {
            return (int)arrowType;
        }

        /// <summary>
        /// Checks if this spawner is active. returns true if so, otherwise false
        /// </summary>
        /// <returns>True if spawner is active, otherwise false</returns>
        private bool CheckIsActive()
        {
            if (_isActive) { return true; }
            else { return false; }
        }

        

        /// <summary>
        /// Returns the masterscript for the arrow minigame
        /// </summary>
        /// <returns>The masterscript for the arrow minigame</returns>
        public ArrowMiniGameMaster GetMasterScript()
        {
            return masterScript;
        }
    }
}
