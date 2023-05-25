using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal delegate void SwProgressionCallback(long level, string levelName, long attempts, long revives);

    internal class SwBlockingApiHandler
    {
        #region --- Events ---

        private event Action OnTimeBasedGameStartedEvent;
        private event SwProgressionCallback OnLevelCompletedEvent;
        private event SwProgressionCallback OnLevelFailedEvent;
        private event SwProgressionCallback OnLevelRevivedEvent;
        private event SwProgressionCallback OnLevelSkippedEvent;
        private event SwProgressionCallback OnLevelStartedEvent;

        #endregion


        #region --- Members ---

        /// <summary>
        ///     The current running blocking api invocation as string
        ///     e.g "NotifyLevelCompleted(10, \"level_10\")"
        /// </summary>
        [PublicAPI]
        protected string CurrentRunningBlockingApiInvocation;

        /// <summary>
        ///     middlewares wre running serially on each blocking api call
        ///     Each one perform an async operation that suspend the blocking api call
        /// </summary>
        private readonly ISwBlockingApiMiddleware[] _middlewares;

        private readonly Lazy<SwBlockingSimulator> _lazySwBlockingSimulator = new Lazy<SwBlockingSimulator>(() => UnityEngine.Object.Instantiate(Resources.Load("Core/Simulators/SwBlockingSimulator", typeof(SwBlockingSimulator))) as SwBlockingSimulator);
        private readonly SwCoreTracker _tracker;
        private readonly SwSettings _settings;
        private readonly SwUserData _userData;
        
        #endregion


        #region --- Properties ---

        /// <summary>
        ///     An interface for showing the blocking api invocation as text in a popup
        ///     Relevant only when _settings.testBlockingApiInvocation == true
        /// </summary>
        private SwBlockingSimulator BlockingSimulator
        {
            get { return _lazySwBlockingSimulator.Value; }
        }

        private bool CanNotifyRewardedVideoOpportunityMissed
        {
            get => !_settings.isTimeBased && !_userData.ImmutableUserState().isDuringLevel;
        }
        
        #endregion


        #region --- Construction ---

        internal SwBlockingApiHandler(SwSettings settings, SwCoreTracker tracker, SwUserData userData, ISwBlockingApiMiddleware[] blockingApiMiddlewares, ISwGameProgressionListener[] gameProgressionListeners)
        {
            _settings = settings;
            _userData = userData;
            _tracker = tracker;
            _middlewares = blockingApiMiddlewares ?? new ISwBlockingApiMiddleware[] { };

            if (gameProgressionListeners != null)
            {
                foreach (var gameProgressionListener in gameProgressionListeners)
                {
                    OnTimeBasedGameStartedEvent += gameProgressionListener.OnTimeBasedGameStarted;
                    OnLevelStartedEvent += gameProgressionListener.OnLevelStarted;
                    OnLevelCompletedEvent += gameProgressionListener.OnLevelCompleted;
                    OnLevelFailedEvent += gameProgressionListener.OnLevelFailed;
                    OnLevelSkippedEvent += gameProgressionListener.OnLevelSkipped;
                    OnLevelRevivedEvent += gameProgressionListener.OnLevelRevived;
                }
            }
        }

        #endregion


        #region --- Public Methods ---

        public IEnumerator NotifyGameStarted ()
        {
            _userData.ModifyUserStateSync(mutableUserState =>
            {
                mutableUserState.isDuringLevel = true;
            });
            
            try
            {
                if (_settings.isTimeBased)
                {
                    OnTimeBasedGameStartedEvent?.Invoke();   
                }
            }
            catch (Exception e)
            {
                SwInfra.Logger.LogError($"SwBlockingApiHandler.NotifyGameStarted error: {e.Message}");
            }
            
            foreach (var middleware in _middlewares)
            {
                yield return middleware.ProcessGameStarted();
            }
        }

        public IEnumerator NotifyLevelCompleted(long level, string levelName = null)
        {
            var waitForBlockerClose = Lock($"NotifyLevelCompleted({level}, \"{levelName}\")");

            // saving state before resetting it
            var userState = _userData.ImmutableUserState();

            _userData.ModifyUserStateSync(mutableUserState =>
            {
                mutableUserState.completedLevels = level;
                mutableUserState.playedLevels++;
                mutableUserState.consecutiveFailedLevels = 0;
                mutableUserState.consecutiveCompletedLevels++;
                mutableUserState.levelAttempts = 0;
                mutableUserState.isDuringLevel = false;
            });

            try
            {
                OnLevelCompletedEvent?.Invoke(level, levelName, userState.levelAttempts, userState.levelRevives);
            }
            catch (Exception e)
            {
                SwInfra.Logger.LogError($"SwBlockingApiHandler.NotifyLevelCompleted error: {e.Message}");
            }
            
            foreach (var middleware in _middlewares)
            {
                yield return middleware.ProcessLevelCompleted(level, levelName);
            }

            yield return Unlock(waitForBlockerClose);
        }
        
        public void NotifyLevelCompleted(long level, Action action, string levelName = null)
        {
            SwInfra.CoroutineService.StartCoroutineWithCallback(() => NotifyLevelCompleted(level, levelName), action);
        }

        public IEnumerator NotifyLevelFailed(long level, string levelName = null)
        {
            var waitForBlockerClose = Lock($"NotifyLevelFailed({level}, \"{levelName}\")");

            _userData.ModifyUserStateSync(mutableUserState =>
            {
                mutableUserState.playedLevels++;
                mutableUserState.consecutiveFailedLevels++;
                mutableUserState.consecutiveCompletedLevels = 0;
                mutableUserState.isDuringLevel = false;
            });

            var userState = _userData.ImmutableUserState();

            try
            {
                OnLevelFailedEvent?.Invoke(level, levelName, userState.levelAttempts, userState.levelRevives);
            }
            catch (Exception e)
            {
                SwInfra.Logger.LogError($"SwBlockingApiHandler.NotifyLevelFailed error: {e.Message}");
            }
            
            foreach (var middleware in _middlewares)
            {
                yield return middleware.ProcessLevelFailed(level, levelName);
            }

            yield return Unlock(waitForBlockerClose);
        }

        public void NotifyLevelFailed(long level, Action action, string levelName = null)
        {
            SwInfra.CoroutineService.StartCoroutineWithCallback(() => NotifyLevelFailed(level, levelName), action);
        }

        public IEnumerator NotifyLevelRevived(long level, string levelName = null)
        {
            var waitForBlockerClose = Lock($"NotifyLevelRevived({level}, \"{levelName}\")");

            _userData.ModifyUserStateSync(mutableUserState =>
            {
                mutableUserState.levelRevives++;
                mutableUserState.isDuringLevel = true;
            });

            var userState = _userData.ImmutableUserState();

            try
            {
                OnLevelRevivedEvent?.Invoke(level, levelName, userState.levelAttempts, userState.levelRevives);
            }
            catch (Exception e)
            {
                SwInfra.Logger.LogError($"SwBlockingApiHandler.NotifyLevelRevived error: {e.Message}");
            }
            
            foreach (var middleware in _middlewares)
            {
                yield return middleware.ProcessLevelRevived(level, levelName);
            }

            yield return Unlock(waitForBlockerClose);
        }

        public void NotifyLevelRevived(long level, Action action, string levelName = null)
        {
            SwInfra.CoroutineService.StartCoroutineWithCallback(() => NotifyLevelRevived(level, levelName), action);
        }
        
        public IEnumerator NotifyLevelSkipped(long level, string levelName = null)
        {
            var waitForBlockerClose = Lock($"NotifyLevelSkipped({level}, \"{levelName}\")");

            // saving state before resetting it
            var userState = _userData.ImmutableUserState();

            _userData.ModifyUserStateSync(mutableUserState =>
            {
                mutableUserState.completedLevels = level;
                mutableUserState.consecutiveCompletedLevels++;
                mutableUserState.levelAttempts = 0;
                mutableUserState.isDuringLevel = false;
            });

            try
            {
                OnLevelSkippedEvent?.Invoke(level, levelName, userState.levelAttempts, userState.levelRevives);
            }
            catch (Exception e)
            {
                SwInfra.Logger.LogError($"SwBlockingApiHandler.NotifyLevelSkipped error: {e.Message}");
            }
            
            foreach (var middleware in _middlewares)
            {
                yield return middleware.ProcessLevelSkipped(level, levelName);
            }

            yield return Unlock(waitForBlockerClose);
        }

        public void NotifyLevelSkipped(long level, Action action, string levelName = null)
        {
            SwInfra.CoroutineService.StartCoroutineWithCallback(() => NotifyLevelSkipped(level, levelName), action);
        }

        public IEnumerator NotifyLevelStarted(long level, string levelName = null)
        {
            var waitForBlockerClose = Lock($"NotifyLevelStarted({level}, \"{levelName}\")");

            _userData.ModifyUserStateSync(mutableUserState =>
            {
                mutableUserState.levelRevives = 0;
                mutableUserState.levelAttempts++;
                mutableUserState.isDuringLevel = true;
            });

            var userState = _userData.ImmutableUserState();

            try
            {
                OnLevelStartedEvent?.Invoke(level, levelName, userState.levelAttempts, userState.levelRevives);
            }
            catch (Exception e)
            {
                SwInfra.Logger.LogError($"SwBlockingApiHandler.NotifyLevelStarted error: {e.Message}");
            }
            
            foreach (var middleware in _middlewares)
            {
                yield return middleware.ProcessLevelStarted(level, levelName);
            }

            yield return Unlock(waitForBlockerClose);
        }

        public void NotifyLevelStarted(long level, Action action, string levelName = null)
        {
            SwInfra.CoroutineService.StartCoroutineWithCallback(() => NotifyLevelStarted(level, levelName), action);
        }

        public void NotifyRewardedVideoOpportunityMissedAfterLevelEnd(Action action)
        {
            SwInfra.CoroutineService.StartCoroutineWithCallback(NotifyRewardedVideoOpportunityMissedAfterLevelEnd, action);
        }


        public IEnumerator NotifyRewardedVideoOpportunityMissedAfterLevelEnd()
        {
            if (!CanNotifyRewardedVideoOpportunityMissed)
            {
                SwInfra.Logger.LogWarning("SwBlockingApiHandler | Can only notify opportunity missed if game is level based and level has ended");
                yield break;
            }
            
            var waitForBlockerClose = Lock("RewardedVideoOpportunityMissed()");

            foreach (var middleware in _middlewares)
            {
                yield return middleware.ProcessRewardedVideoOpportunityMissed();
            }

            yield return Unlock(waitForBlockerClose);
        }

        #endregion


        #region --- Private Methods ---

        /// <summary>
        ///     This methods is run upon every blocking api invocation
        ///     It sets the singular CurrentRunningBlockingApiInvocation with new value
        /// </summary>
        /// <param name="blockingApiInvocation"></param>
        /// <returns>a function which returns an IEnumerator that ends when the blocking popup is closed</returns>
        private SwAsyncMethod Lock(string blockingApiInvocation)
        {
            var error = "";

            if (!string.IsNullOrEmpty(CurrentRunningBlockingApiInvocation))
            {
                // In this case another blocking API is still running, produce a human-readable error
                error = $"Blocking API Error: {blockingApiInvocation} was called before {CurrentRunningBlockingApiInvocation} was resolved";
                SwInfra.Logger.LogError($"SwBlockingApiHandler | {error}");

                // For the purpose of this error to be noticed the app is crashed when in DebugBuild
                if (Debug.isDebugBuild)
                {
                    Application.Quit(1);
                }

                // In case _settings.testBlockingApiInvocation = true
                // The current popup is hidden and a new popup with the error is shown right after that.
                if (_settings.testBlockingApiInvocation)
                {
                    BlockingSimulator.Hide();
                }
            }

            CurrentRunningBlockingApiInvocation = blockingApiInvocation;

            if (!_settings.testBlockingApiInvocation) return null;

            BlockingSimulator.Closable = false;

            // returns an async function (IEnumerator), that resolves when the popup is closed.
            return BlockingSimulator.Show(CurrentRunningBlockingApiInvocation, error);
        }

        /// <summary>
        ///     Resets CurrentRunningBlockingApiInvocation member
        ///     Waits for blocking simulator popup to close
        /// </summary>
        /// <param name="waitForBlockerClose">an async function (IEnumerator), that resolves when the popup is closed</param>
        /// <returns></returns>
        private IEnumerator Unlock(SwAsyncMethod waitForBlockerClose)
        {
            CurrentRunningBlockingApiInvocation = "";

            if (!_settings.testBlockingApiInvocation) yield break;

            BlockingSimulator.Closable = true;

            yield return waitForBlockerClose();
        }
        

        #endregion
    }
}