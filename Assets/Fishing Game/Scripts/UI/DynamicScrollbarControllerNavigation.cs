using System;
using FishingGame.GameManagement;
using UnityEditor.Experimental.GraphView;
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

        [SerializeField] private GameObject scrollBoxGameObject;
        [SerializeField] private GameObject scrollBoxContent;

        private Scrollbar _scrollBar;
        private bool hasNavigationBeenEstablished;

        private void OnEnable()
        {
            GameManager.Instance.GameEvents.OnElementAddedToScrollbox += SetupLeftNavigation;
            hasNavigationBeenEstablished = false;
            _scrollBar = GetComponent<Scrollbar>();
            
            var navigation = new Navigation();
            navigation.mode = Navigation.Mode.Explicit;
            
            navigation.selectOnRight = selectOnRight;
            navigation.selectOnDown = selectOnDown;
            navigation.selectOnUp = selectOnUp;
            
            _scrollBar.navigation = navigation;        
        }

        private void SetupLeftNavigation(string scrollBoxName)
        {
            if (!hasNavigationBeenEstablished && string.Equals(scrollBoxGameObject.name, scrollBoxName))
            {
                Navigation navigation = _scrollBar.navigation;
                navigation.selectOnLeft = scrollBoxContent.transform.GetChild(0).GetComponent<Button>();
                _scrollBar.navigation = navigation;
                hasNavigationBeenEstablished = true;
            }
        }
    }
}
