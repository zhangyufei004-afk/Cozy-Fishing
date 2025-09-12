using UnityEngine;

namespace FishingGame
{
    [System.Serializable]
    public enum TabType
    {
        INVENTORY,
        FISHLOG,
        QUESTS,
        SETTINGS
    }

    public class TabManager : MonoBehaviour
    {
        // Private Readable Variables
        [Header("References")]
        [SerializeField] private GameObject inventoryTabObject;
        [SerializeField] private GameObject fishLogTabObject;
        [SerializeField] private GameObject questsTabObject;
        [SerializeField] private GameObject settingsTabObject;

        // Private Variables
        private TabType _currentTab = TabType.INVENTORY;

        void OnEnable()
        {
            SetTab(0);
            DisableEnableTabs();
        }

        public void SetTab(int tabType)
        {
            TabType inTabType = (TabType)tabType;

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
                case TabType.INVENTORY:
                    inventoryTabObject.SetActive(true);
                    break;
                case TabType.FISHLOG:
                    fishLogTabObject.SetActive(true);
                    break;
                case TabType.QUESTS:
                    questsTabObject.SetActive(true);
                    break;
                case TabType.SETTINGS:
                    settingsTabObject.SetActive(true);
                    break;
            }
        }
    }
}
