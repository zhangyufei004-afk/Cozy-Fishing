using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FishingGame.Reeling
{
    public class ArrowSpawner : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The arrow prefab")]
        private GameObject spawnableArrow;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            SpawnArrow();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void SpawnArrow()
        {
            GameObject currentArrow = Instantiate(spawnableArrow, gameObject.transform);
            currentArrow.GetComponent<MovingArrow>().ActivateArrow(50);
        }
    }
}
