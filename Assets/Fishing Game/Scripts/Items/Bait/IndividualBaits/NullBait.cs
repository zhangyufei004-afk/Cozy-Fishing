using FishingGame.FishSystem;
using FishingGame.Reeling;

namespace FishingGame.Items.Bait
{
    /// <summary>
    /// When no bait is equiped by the fishing rod this bait is assigned to it
    /// This simply returns null for all of the bait functionality checks by the fishing rod
    /// </summary>
    public class NullBait : IBait
    {
        private FishingRod _activeFishingRod;

        /// <summary>
        /// Applys this bait to the inputed fishing rod
        /// It then sets the current active fishing rod to the inputed one
        /// </summary>
        /// <param name="rodToApplyTo">The fishing rod</param>
        public void SetActiveFishingRod(FishingRod rodToApplyTo)
        {
            _activeFishingRod = rodToApplyTo;
            _activeFishingRod.EquipBait(this);
            return;
        }
        
        /// <summary>
        /// Returns null
        /// </summary>
        /// <returns>Null</returns>
        public FishScriptableObject GetForcedFishType()
        {
            return null;
        }

        /// <summary>
        /// Returns null
        /// </summary>
        /// <returns>Null</returns>
        public void BaitMinigameBehaviour()
        {
            return;
        }

        /// <summary>
        /// Returns null
        /// </summary>
        /// <returns>Null</returns>
        public void UseBaitCharge()
        {
            return;
        }

        /// <summary>
        /// Returns null
        /// </summary>
        /// <returns>Null</returns>
        public void UsedUpBait()
        {
            return;
        }

        /// <summary>
        /// Returns null
        /// </summary>
        /// <returns>Null</returns>
        public bool IsBaitUsedUp()
        {
            return false;
        }
    }
}
