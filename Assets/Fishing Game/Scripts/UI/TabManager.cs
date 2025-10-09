using UnityEngine;

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

        // Private Variables
        private ETabType _currentTab = ETabType.INVENTORY;

        void OnEnable()
        {
            SetTab(0);
            DisableEnableTabs();
        }

        public void SetTab(int tabType)
        {
            ETabType inTabType = (ETabType)tabType;

            if (_currentTab != inTabType)
            {
                _currentTab = inTabType;
                DisableEnableTabs();
            }
        }

        void DisableEnableTabs()
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
