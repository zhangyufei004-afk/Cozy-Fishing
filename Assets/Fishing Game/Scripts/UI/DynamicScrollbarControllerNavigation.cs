using FishingGame.GameManagement;
using UnityEngine;
using UnityEngine.UI;

namespace FishingGame.UI
{
    /// <summary>
    /// Establishes lefthand UI navigation for the scrollbar. Listens to the item added to scrollbar event and then establishes the lefthand navigation.
    /// </summary>
    public class DynamicScrollbarControllerNavigation : MonoBehaviour
    {
        [Header("Navigation")] 
        [SerializeField] private Selectable selectOnRight;
        [SerializeField] private Selectable selectOnUp;
        [SerializeField] private Selectable selectOnDown;
        [SerializeField] private bool setupButtonNavigation;
        
        [Header("UI Elements")]
        [Tooltip("The scroll bar to apply the navigation to.")]
        [SerializeField] private Scrollbar scrollbar;
        [SerializeField] private GameObject scrollBoxGameObject;
        [SerializeField] private GameObject scrollBoxContent;

        private bool _hasNavigationBeenEstablished;

        private void OnEnable()
        {
            GameManager.Instance.GameEvents.OnElementAddedToScrollbox += SetupLeftNavigation;
            _hasNavigationBeenEstablished = false;
            
            var navigation = new Navigation();
            navigation.mode = Navigation.Mode.Explicit;
            
            navigation.selectOnRight = selectOnRight;
            navigation.selectOnDown = selectOnDown;
            navigation.selectOnUp = selectOnUp;
            
            scrollbar.navigation = navigation;        
        }

        private void SetupLeftNavigation(string scrollBoxName)
        {
            if (string.Equals(scrollBoxGameObject.name, scrollBoxName))
            {
                int childCount = scrollBoxContent.transform.childCount;
                GameObject latestEntry = scrollBoxContent.transform.GetChild(childCount - 1).gameObject;
                Button entryButton = latestEntry.GetComponent<Button>();
                if (setupButtonNavigation)
                {
                    var buttonNavigation = new Navigation
                    {
                        mode = Navigation.Mode.Automatic,
                        selectOnRight = scrollbar,
                        wrapAround = true,
                    };
                    entryButton.navigation = buttonNavigation;
                }

                if (!_hasNavigationBeenEstablished)
                {
                    Navigation navigation = scrollbar.navigation;
                    navigation.selectOnLeft = entryButton;
                    scrollbar.navigation = navigation;
                    _hasNavigationBeenEstablished = true;
                }            
            }
        }
    }
}
