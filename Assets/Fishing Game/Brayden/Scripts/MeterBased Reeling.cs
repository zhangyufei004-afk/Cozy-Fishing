using System;
using UnityEngine;
using UnityEngine.UI;

namespace Brayden
{
    /*
     * Meterbased reeling much like stardew valley fishing
     * UI contains a fish and a green box, the fish will move vertically a random amount
     * player can control the green box with up and down controls
     * goal is to keep the fish in the green box which will fill a progress bar at bottom of screen
     */

    public class MeterBasedReeling : MonoBehaviour
    {
        // The icon of the fish in the ui, this moves around
        public GameObject fishIcon;

        // The green UI box that is moveable by player
        public GameObject catchIndicator;

        // The progress bar for catching
        public Slider catchProgress;

        [SerializeField]
        private CatchBox UiCatchBoxScript;

        [SerializeField]
        private RectTransform CatchBox;

        [SerializeField]
        private RectTransform FishImage;

        // Max and minimum value to move fish along ui
        public int fishMoveMax;
        public int fishMoveMin;

        // Timer for how often fish moves
        public float fishMoveTimer = 0.5f;

        // Max and min heights for the fish icon
        public float fishHeightMax;
        public float fishHeightMin;

        // Max and min heights for the catch indicator ui
        public float catchIndicatorHeightMax;
        public float catchIndicatorHeightMin;

        // Tracks times
        private float timerValue = 0;

        // Tracks if catching is active
        public bool catchingActive = false;

        public void Start()
        {
            // Temp for testing
            catchingActive = true;
        }

        public void Update()
        {
            // If catching is active add to timer
            if (catchingActive)
            {
                timerValue += Time.deltaTime;
            }

            // Check if timer complete
            if (timerValue >= fishMoveTimer)
            {
                TimerComplete();
            }

            // Check if up arrow pressed
            if (Input.GetKey(KeyCode.UpArrow))
            {
                MoveCatchIndicator(5 * Time.deltaTime);
            }

            // Check if down arrow pressed
            if (Input.GetKey(KeyCode.DownArrow))
            {
                MoveCatchIndicator(-5 * Time.deltaTime);
            }

            // Check if fish is in box
            // If so positivly increase progress bar
            // If not decrease progress bar
            if (CheckFishInBox())
            {
                ChangeCatchProgress(2);
            }
            else
            {
                ChangeCatchProgress(-2);
            }

        }

        // Reset timer and move the fish
        public void TimerComplete()
        {
            timerValue = 0;
            MoveFish();
            
        }

        // Randomly move the fish icon up or down
        public void MoveFish()
        {
            int randomValue = UnityEngine.Random.Range(fishMoveMin, fishMoveMax);

            Vector3 currentPosition = fishIcon.transform.position;
            Vector3 newPosition = new Vector3(currentPosition.x, currentPosition.y += randomValue, currentPosition.z);
            
            fishIcon.transform.position = newPosition;
        }

        // Move the catch indicator based on the input
        public void MoveCatchIndicator(float Amount)
        {
            Vector3 currentPosition = catchIndicator.transform.position;
            Vector3 newPosition = new Vector3(currentPosition.x, currentPosition.y += Amount, currentPosition.z);

            catchIndicator.transform.position = newPosition;
        }

        // Checks if fish is currently within the catching box
        public bool CheckFishInBox()
        {
            if (UiCatchBoxScript.CheckUIOverlap(CatchBox, FishImage) == true)
            {
                return true;
            }
            else { return false; }
        }

        // Change the progress bar based on if fish is in box
        public void ChangeCatchProgress(int multiplier)
        {
            catchProgress.value += multiplier * Time.deltaTime;
        }
    }
}
