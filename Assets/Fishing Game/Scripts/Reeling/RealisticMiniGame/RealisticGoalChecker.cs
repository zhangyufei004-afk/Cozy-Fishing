using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FishingGame.Reeling
{
    /// <summary>
    /// This class is purley used to check if the mouse is ontop of the goal in the realisticminigame
    /// </summary>
    public class RealisticGoalChecker : MonoBehaviour
    {
        private int _uILayer;

        private void Start()
        {
            _uILayer = LayerMask.NameToLayer("UI");
        }


        /// <summary>
        /// Returns true if the mouse is touching the goal for the realistic minigame
        /// </summary>
        public bool IsPointerOverUIElement()
        {
            return IsPointerOverUIElement(GetEventSystemRaycastResults());
        }


        /// <summary>
        /// Returns true if raycast from mouse touches a ui element taged as "RealisticGoal"
        /// </summary>
        private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
        {
            for (int index = 0; index < eventSystemRaysastResults.Count; index++)
            {
                RaycastResult curRaysastResult = eventSystemRaysastResults[index];
                if (curRaysastResult.gameObject.layer == _uILayer && curRaysastResult.gameObject.CompareTag("RealisticGoal"))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets all event system raycast results of current mouse or touch position.
        /// </summary>
        private static List<RaycastResult> GetEventSystemRaycastResults()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = UnityEngine.Input.mousePosition;
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults;
        }
    }
}

