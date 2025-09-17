using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace FishingGame.Reeling
{
    public class ArrowGoalPoints : MonoBehaviour
    {
        private Image _goalPointImage;

        private void OnEnable()
        {
            _goalPointImage = GetComponent<Image>();
        }

        public bool CheckUIOverlap(float maxDifference, GameObject objectComparedTo)
        {
            float distanceFromPoint = gameObject.transform.localPosition.y - objectComparedTo.transform.localPosition.y;
            Debug.Log(distanceFromPoint);

            if (distanceFromPoint < maxDifference && distanceFromPoint > -maxDifference)
            {
                return true;
            }
            else { return false; }
        }

        public Image GetGoalPointImage()
        {
            return _goalPointImage;
        }
    }
}
