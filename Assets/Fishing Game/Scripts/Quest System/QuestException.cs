using System;

namespace FishingGame.QuestSystem
{
    /// <summary>
    /// Quest Exception Class, for proper logging in the unity editor debug logger that an error occured with a quest.
    /// </summary>
    internal class QuestException : InvalidOperationException
    {
        public QuestException(string message) : base(message)
        {
        }

        public QuestException() : base("An unknown error occured with a Quest.")
        {
        }
    }
}