#if SW_STAGE_STAGE1_OR_ABOVE

using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SupersonicWisdomSDK
{
    public static class Stage1ApiExtension
    {
        #region --- Members ---

        private static SwStage1Container _container;

        #endregion


        #region --- Properties ---

        private static SwStage1Container Container
        {
            get
            {
                if (_container == null || _container.IsDestroyed())
                {
                    _container = (SwStage1Container)SwApi.Container;
                }

                return _container;
            }
        }

        #endregion


        #region --- Public Methods ---

        [PublicAPI]
        public static void AddOnFacebookInitCompleteListener(this SwApi self, OnFacebookInitComplete listener)
        {
            self.RunWhenContainerIsReady(() => { Container.FacebookAdapter.OnFacebookInitCompleteEvent += listener; });
        }

        [PublicAPI]
        public static void AddOnGameAnalyticsInitListener(this SwApi self, OnGameAnalyticsInit listener)
        {
            self.RunWhenContainerIsReady(() => { Container.GameAnalyticsAdapter.OnGameAnalyticsInitEvent += listener; });
        }

        /// <summary>
        ///     Add listener for finishing resolving remote config
        ///     The name "onLoaded" is used here because it's eventually
        ///     exposed via `SupersonicWisdom.Api.AddOnLoadedListener`
        /// </summary>
        /// <param name="onLoadedCallback"></param>
        [PublicAPI]
        public static void AddOnLoadedListener(this SwApi self, OnLoaded onLoadedCallback)
        {
            self.RunWhenContainerIsReady(() => { Container.RemoteConfigRepository.AddOnLoadedListener(onLoadedCallback); });
        }

        [PublicAPI]
        public static void AddOnReadyListener(this SwApi self, OnReady listener)
        {
            self.RunWhenContainerIsReady(() => { Container.OnReadyEvent += listener; });
        }

        [PublicAPI]
        public static bool DidLoad(this SwApi self)
        {
            return self.GetWithContainerOrDefault(() => Container.RemoteConfigRepository.DidResolve(), false);
        }

        [PublicAPI]
        public static int GetConfigValue(this SwApi self, string key, int defaultVal)
        {
            return self.WasInitialized ? Container.ConfigAccessor.GetConfigValue(key, defaultVal) : defaultVal;
        }

        [PublicAPI]
        public static float GetConfigValue(this SwApi self, string key, float defaultVal)
        {
            return self.WasInitialized ? Container.ConfigAccessor.GetConfigValue(key, defaultVal) : defaultVal;
        }

        [PublicAPI]
        public static string GetConfigValue(this SwApi self, string key, string defaultVal)
        {
            return self.WasInitialized ? Container.ConfigAccessor.GetConfigValue(key, defaultVal) : defaultVal;
        }

        [PublicAPI]
        public static bool GetConfigValue(this SwApi self, string key, bool defaultVal)
        {
            return self.WasInitialized ? Container.ConfigAccessor.GetConfigValue(key, defaultVal) : defaultVal;
        }

        [PublicAPI]
        public static Dictionary<string, string> GetDeepLinkParameters(this SwApi self)
        {
            return self.GetWithContainerOrThrow(GetDeepLinkParametersWithContainer, "GetDeepLinkParameters");
        }

        [PublicAPI]
        public static string GetGroup(this SwApi self)
        {
            return self.GetWithContainerOrDefault(() => Container.RemoteConfigRepository.GetAb().group, "");
        }

        [PublicAPI]
        public static SwSettings GetSettings(this SwApi self)
        {
            return self.GetWithContainerOrThrow(GetSettingsWithContainer, "GetSettings");
        }

        [PublicAPI]
        public static string GetUserId(this SwApi self)
        {
            return self.GetWithContainerOrThrow(GetUserIdWithContainer, "GetUserId");
        }

        [PublicAPI]
        public static bool IsNewUser(this SwApi self)
        {
            return self.GetWithContainerOrThrow(IsNewUserWithContainer, "IsNewUser");
        }

        [PublicAPI]
        public static bool IsReady(this SwApi self)
        {
            return self.GetWithContainerOrDefault(IsReadyWithContainer, false);
        }

        /// <summary>
        ///     Notifies the SDK about level completed.
        /// </summary>
        /// <param name="level">The completed level's number.</param>
        /// <param name="action">What needs to be done after the SDK finishes.</param>
        /// <param name="levelName">The completed level's name</param>
        [PublicAPI]
        public static void NotifyLevelCompleted(this SwApi self, long level, Action action, string levelName = null)
        {
            if (!Container.Settings.isTimeBased)
            {
                self.RunWithContainerOrThrow(
                    () => Container.BlockingApiHandler.NotifyLevelCompleted(level, action, levelName),
                    "NotifyLevelCompleted");
            }
            else
            {
                self.HandleUnsupportedLevelBasedApiCall(nameof(NotifyLevelCompleted));
            }
        }

        /// <summary>
        ///     Notifies the SDK about level failed.
        /// </summary>
        /// <param name="level">The failed level's number.</param>
        /// <param name="action">What need to be done when the SDK finishes.</param>
        /// <param name="levelName">The failed level's name. Optional param.</param>
        [PublicAPI]
        public static void NotifyLevelFailed(this SwApi self, long level, Action action, string levelName = null)
        {
            if (!Container.Settings.isTimeBased)
            {
                self.RunWithContainerOrThrow(
                    () => Container.BlockingApiHandler.NotifyLevelFailed(level, action, levelName),
                    "NotifyLevelFailed");
            }
            else
            {
                self.HandleUnsupportedLevelBasedApiCall(nameof(NotifyLevelFailed));
            }
        }

        /// <summary>
        ///     Notifies the SDK about level skipped.
        /// </summary>
        /// <param name="level">The started level's number.</param>
        [PublicAPI]
        public static void NotifyLevelRevived(this SwApi self, long level, Action action)
        {
            if (!Container.Settings.isTimeBased)
            {
                self.RunWithContainerOrThrow(() => Container.BlockingApiHandler.NotifyLevelRevived(level, action),
                    "NotifyLevelRevived");
            }
            else
            {
                self.HandleUnsupportedLevelBasedApiCall(nameof(NotifyLevelRevived));
            }
        }

        /// <summary>
        ///     Notifies the SDK about level skipped.
        /// </summary>
        /// <param name="level">The started level's number.</param>
        [PublicAPI]
        public static void NotifyLevelSkipped(this SwApi self, long level, Action action)
        {
            if (!Container.Settings.isTimeBased)
            {
                self.RunWithContainerOrThrow(() => Container.BlockingApiHandler.NotifyLevelSkipped(level, action),
                    "NotifyLevelSkipped");
            }
            else
            {
                self.HandleUnsupportedLevelBasedApiCall(nameof(NotifyLevelSkipped));
            }
        }

        /// <summary>
        ///     Notifies the SDK about level started.
        /// </summary>
        /// <param name="level">The started level's number.</param>
        [PublicAPI]
        public static void NotifyLevelStarted(this SwApi self, long level, Action action)
        {
            if (!Container.Settings.isTimeBased)
            {
                self.RunWithContainerOrThrow(() => Container.BlockingApiHandler.NotifyLevelStarted(level, action),
                    "NotifyLevelStarted");
            }
            else
            {
                self.HandleUnsupportedLevelBasedApiCall(nameof(NotifyLevelStarted));
            }
        }

        [PublicAPI]
        public static void RemoveOnFacebookInitCompleteListener(this SwApi self, OnFacebookInitComplete listener)
        {
            self.RunWhenContainerIsReady(() => { Container.FacebookAdapter.OnFacebookInitCompleteEvent -= listener; });
        }

        [PublicAPI]
        public static void RemoveOnGameAnalyticsInitListener(this SwApi self, OnGameAnalyticsInit listener)
        {
            self.RunWhenContainerIsReady(() => { Container.GameAnalyticsAdapter.OnGameAnalyticsInitEvent -= listener; });
        }

        [PublicAPI]
        public static void RemoveOnLoadedListener(this SwApi self, OnLoaded listener)
        {
            self.RunWhenContainerIsReady(() => { Container.RemoteConfigRepository.RemoveOnLoadedListener(listener); });
        }

        [PublicAPI]
        public static void RemoveOnReadyListener(this SwApi self, OnReady listener)
        {
            self.RunWhenContainerIsReady(() => { Container.OnReadyEvent -= listener; });
        }

        [PublicAPI]
        public static string GetSdkVersion(this SwApi self)
        {
            return self.GetWithContainerOrDefault(GetSdkVersionWithContainer, string.Empty);
        }

        #endregion


        #region --- Private Methods ---

        
        internal static void HandleUnsupportedLevelBasedApiCall(this SwApi self, string fnName)
        {
            self.LogInvocationWarning(
                $"SupersonicWisdom.Api.{fnName} is not supported for time based games, if this game is level based please uncheck the 'Time Based Game' checkbox in the SupersonicWisdomSDK settings");
        }
        
        internal static void AddInternalListener(this SwApi self, OnInternal v)
        {
            SwInternalEvent.OnInternalEvent += v;
        }

        internal static void RemoveInternalListener(this SwApi self, OnInternal v)
        {
            SwInternalEvent.OnInternalEvent -= v;
        }

        internal static SwUserState UserState(this SwApi self)
        {
            return Container.CopyOfUserState();
        }

        private static Dictionary<string, string> GetDeepLinkParametersWithContainer ()
        {
            return Container.DeepLinkHandler.DeepLinkParamsClone;
        }

        private static SwSettings GetSettingsWithContainer ()
        {
            return Container.Settings;
        }

        private static string GetUserIdWithContainer ()
        {
            return Container.UserData.Uuid;
        }

        private static bool IsNewUserWithContainer ()
        {
            return Container.UserData.IsNew;
        }

        private static bool IsReadyWithContainer ()
        {
            return Container.IsReady;
        }
        
        private static string GetSdkVersionWithContainer()
        {
            return SwConstants.SdkVersion;
        }

        #endregion
    }
}

#endif