using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingGame.UI
{
    [System.Serializable]
    internal enum ETabType
    {
        INVENTORY,
        FISHLOG,
        QUESTS,
        SETTINGS
    }

    /// <summary>
    /// Controls the UI panel visibility of Tabs.
    /// Toggles the various menus when the corresponding tab button is pressed.
    /// </summary>
    public class TabManager : MonoBehaviour
    {
        // Private Readable Variables
        [Header("References")]
        [SerializeField] private GameObject inventoryTabObject;
        [SerializeField] private GameObject fishLogTabObject;
        [SerializeField] private GameObject questsTabObject;
        [SerializeField] private GameObject settingsTabObject;

        [SerializeField]
        private AudioSource paperSound;

        // Private Variables
        private ETabType _currentTab = ETabType.INVENTORY;
        private InputAction _navigateTabsAction;

        void OnEnable()
        {
            SetTab(0);
            DisableEnableTabs();
            _navigateTabsAction = InputSystem.actions.FindActionMap("UI").FindAction("InventoryNavigation");
            _navigateTabsAction.performed += SwitchTabController;
        }

        private void OnDisable()
        {
            _navigateTabsAction.performed -= SwitchTabController;
        }

        /// <summary>
        /// Sets the current ui tab to <c>tabType</c>
        /// </summary>
        /// <param name="tabType">The integer representing the ETabType to switch to.</param>
        public void SetTab(int tabType)
        {
            paperSound.Play();
            ETabType inTabType = (ETabType)tabType;

            if (_currentTab != inTabType)
            {
                _currentTab = inTabType;
                DisableEnableTabs();
            }
        }

        private void SwitchTabController(InputAction.CallbackContext inputContext)
        {
            int increaseAmount = (int)inputContext.ReadValue<Single>();
            int nextTab = ((int)_currentTab + increaseAmount) % 4;
            SetTab(nextTab);
        }

        private void DisableEnableTabs()
        {
            inventoryTabObject.SetActive(false);
            fishLogTabObject.SetActive(false);
            questsTabObject.SetActive(false);
            settingsTabObject.SetActive(false);

            switch (_currentTab)
            {
                case ETabType.INVENTORY:
                    inventoryTabObject.SetActive(true);
                    break;
                case ETabType.FISHLOG:
                    fishLogTabObject.SetActive(true);
                    break;
                case ETabType.QUESTS:
                    questsTabObject.SetActive(true);
                    break;
                case ETabType.SETTINGS:
                    settingsTabObject.SetActive(true);
                    break;
            }
        }
    }
}
