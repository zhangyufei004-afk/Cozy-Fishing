using FishingGame.Items;
using UnityEngine;

namespace FishingGame.Reeling
{
    public class FishingRod : MonoBehaviour
    {
        [Header("Scrip References")]

        [SerializeField]
        [Tooltip("Reference to the master reeling script found in the reelingcontainer")]
        private ReelingMaster reelingMasterScript;

        [SerializeField]
        [Tooltip("A reference to the initiation script attatched to player.")]
        private ReelingInitiation initiationScript;

        [SerializeField]
        [Tooltip("A reference to the fishing hook script which is attatched to a fishing rod.")]
        private FishingHook fishingHook;

        [Header("Aiming and charging cast")]



        [Header("Misc")]

        private IBait _currentlyEquipedBait;


        /// <summary>
        /// Equips the inputed bait 
        /// </summary>
        /// <param name="baitToEquip">The bait item to equip</param>
        public void EquipBait(IBait baitToEquip)
        {
            _currentlyEquipedBait = baitToEquip;
        }



    }
}
