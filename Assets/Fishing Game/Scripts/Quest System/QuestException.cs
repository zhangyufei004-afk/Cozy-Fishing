using System;

namespace FishingGame.QuestSystem
{
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