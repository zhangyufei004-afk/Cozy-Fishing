using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace FishingGame.Reeling
{
    /// <summary>
    /// This class is used to check if arrows are close enough to this gameobject
    /// It also checks if arrows have passed the goal and are considered to be failed
    /// </summary>
    public class ArrowGoalPoints : MonoBehaviour
    {
        /// <summary>
        /// Checks if the inputed object is within the inputed range
        /// Returns true if so else false
        /// </summary>
        /// <param name="maxDifference">The positive value of this is used as upper range, negative value is used as lower range</param>
        /// <param name="objectComparedTo">The object to check</param>
        /// <returns>Returns true if object is within range otherwise false</returns>
        public bool CheckIfObjectIsInRange(float maxDifference, GameObject objectComparedTo)
        {
            float distanceFromPoint = gameObject.transform.localPosition.y - objectComparedTo.transform.localPosition.y;

            if (distanceFromPoint < maxDifference && distanceFromPoint > -maxDifference)
            {
                return true;
            }
            else { return false; }
        }

        /// <summary>
        /// Checks if the object imputed has gone far enough below this objects location that it is considered failed
        /// The range is determined by the inputed maxDifference multiplied by 3
        /// </summary>
        /// <param name="maxDifference">Multiplied by 3, is used to calculate how far away object can be</param>
        /// <param name="objectComparedTo">The object to check if it has gone too far</param>
        /// <returns>True if object has passed fail point, otherwise false</returns>
        public bool CheckIfFailSpot(float maxDifference, GameObject objectComparedTo)
        {
            float distanceFromPoint = gameObject.transform.localPosition.y - objectComparedTo.transform.localPosition.y;

            if (distanceFromPoint > gameObject.transform.localPosition.y + maxDifference * 3)
            { return true; }
            else { return false; }
        }
    }
}
