using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwTimerManager : ISwGameProgressionListener
    {
        #region --- Constants ---

        // Playtime count will invoke a tick each 6 seconds (60/10 =6) 
        // in order to support timer configuration for time based games with values like 1.1, 1.2, 1.3 etc.
        private const int PlaytimeTickInterval = 6;

        #endregion


        #region --- Members ---

        protected readonly ISwTimer _playtimeStopWatch;

        #endregion


        #region --- Properties ---

        public ISwTimerListener PlaytimeStopWatch
        {
            get { return _playtimeStopWatch; }
        }

        protected bool IsDuringPlaytime { get; private set; }

        #endregion


        #region --- Construction ---

        public SwTimerManager(MonoBehaviour mono)
        {
            _playtimeStopWatch = SwStopWatch.Create(mono.gameObject, $"{ETimers.PlaytimeMinutes}", true, PlaytimeTickInterval);
        }

        #endregion


        #region --- Public Methods ---
        
        public void OnTimeBasedGameStarted ()
        {
            IsDuringPlaytime = true;
            _playtimeStopWatch.StartTimer();
        }

        public void OnLevelCompleted(long level, string levelName, long attempts, long revives)
        {
            IsDuringPlaytime = false;
            _playtimeStopWatch.PauseTimer();
        }

        public void OnLevelFailed(long level, string levelName, long attempts, long revives)
        {
            IsDuringPlaytime = false;
            _playtimeStopWatch.PauseTimer();
        }

        public void OnLevelRevived(long level, string levelName, long attempts, long revives)
        {
            IsDuringPlaytime = true;
            _playtimeStopWatch.ResumeTimer();
        }

        public void OnLevelSkipped(long level, string levelName, long attempts, long revives)
        {
            IsDuringPlaytime = false;
            _playtimeStopWatch.PauseTimer();
        }

        public void OnLevelStarted(long level, string levelName, long attempts, long revives)
        {
            IsDuringPlaytime = true;
            _playtimeStopWatch.StartTimer();
        }

        #endregion
    }

    internal enum ETimers
    {
        PlaytimeMinutes
    }
}