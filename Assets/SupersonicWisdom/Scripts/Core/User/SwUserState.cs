using System;
using JetBrains.Annotations;

namespace SupersonicWisdomSDK
{
    [Serializable]
    internal class SwUserState
    {
        #region --- Members ---

        public long age;
        public long completedLevels;
        public long consecutiveCompletedLevels;
        public long consecutiveFailedLevels;
        public long levelAttempts;
        public long levelRevives;
        public long playedLevels;
        public long todaySessionsCount;
        public long totalSessionsCount;
        public bool isDuringLevel;
        [CanBeNull] [NonSerialized] public string SessionId;

        #endregion


        #region --- Construction ---

        protected SwUserState(SwUserState other)
        {
            SessionId = other.SessionId;

            isDuringLevel = other.isDuringLevel;
            age = other.age;
            todaySessionsCount = other.todaySessionsCount;
            totalSessionsCount = other.totalSessionsCount;
            completedLevels = other.completedLevels;
            playedLevels = other.playedLevels;
            consecutiveFailedLevels = other.consecutiveFailedLevels;
            consecutiveCompletedLevels = other.consecutiveCompletedLevels;
            levelRevives = other.levelRevives;
            levelAttempts = other.levelAttempts;
        }

        internal SwUserState ()
        { }

        #endregion


        #region --- Public Methods ---

        public virtual SwUserState Copy ()
        {
            return new SwUserState(this);
        }

        #endregion
    }
}