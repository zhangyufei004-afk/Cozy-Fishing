using UnityEngine;
using UnityEngine.EventSystems;

namespace FishingGame.Fishing_Game.Scripts.UI
{
    /// <summary>
    /// Sets the current selected gameobject to the first child of the scrollbox when the scrollbox is enabled.
    /// This is useful for allowing controller users to navigate through the scrollbox when there is items, but the items are dynamic
    /// </summary>
    public class ScrollboxControllerNavigation : MonoBehaviour
    {
        [SerializeField] private GameObject scrollboxContent;
        [SerializeField] private EventSystem eventSystem;
        
        private void OnEnable()
        {
            if (scrollboxContent.transform.childCount > 0)
            {
                eventSystem.SetSelectedGameObject(scrollboxContent.transform.GetChild(0).gameObject);
            }
        }
    }
}