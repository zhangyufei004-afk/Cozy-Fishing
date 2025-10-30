using FishingGame.Reeling;

namespace FishingGame.FishSystem
{
    /// <summary>
    /// An enum that represents what type of fishable item this is
    /// </summary>
    public enum ECatchableType
    {
        None,
        Fish,
        Trash
    }

    public interface IFishAble
    {
        /// <summary>
        /// Returns the difficulty of this catchable object
        /// </summary>
        /// <returns>The difficulty value as an int</returns>
        public int GetCatchDifficulty();

        /// <summary>
        /// Returns what type of fishable object this is
        /// </summary>
        /// <returns>Enum value represneting the catch type</returns>
        public ECatchableType GetCatchType();

        /// <summary>
        /// Returns the custom arrow minigame behaviour for this fishable object
        /// </summary>
        /// <returns>The custom arrow minigame behaviour</returns>
        public ArrowWaveSO GetArrowMinigameBehaviour();

        /// <summary>
        /// Returns the custom slider minigame behaviour for this fishable object
        /// </summary>
        /// <returns>The custom slider minigame behaviour</returns>
        public SliderSO GetSliderMinigameBehaviour();
    }
}
