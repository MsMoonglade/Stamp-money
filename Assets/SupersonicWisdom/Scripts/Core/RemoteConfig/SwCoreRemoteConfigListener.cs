using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal class SwCoreRemoteConfigListener : ISwRemoteConfigListener
    {
        #region --- Members ---

        protected readonly ISwConfigAccessor ConfigAccessor;
        private readonly ISwSettings _settings;
        private readonly SwCoreTracker _tracker;
        private readonly SwNativeAdapter _wisdomNativeAdapter;
        private readonly SwUserData _userData;

        #endregion


        #region --- Construction ---

        public SwCoreRemoteConfigListener(ISwSettings settings, ISwConfigAccessor configAccessor, SwNativeAdapter wisdomNativeAdapter, SwCoreTracker tracker, SwUserData userData)
        {
            _settings = settings;
            ConfigAccessor = configAccessor;
            _wisdomNativeAdapter = wisdomNativeAdapter;
            _tracker = tracker;
            _userData = userData;
        }

        #endregion


        #region --- Public Methods ---

        public void OnResolve(SwRemoteConfigError error, SwRemoteConfig remoteConfig, SwAbConfig abConfig, Dictionary<string, object> accessibleConfig)
        {
            Handle(error, remoteConfig, abConfig, accessibleConfig);
            Track(error, remoteConfig, abConfig, accessibleConfig);
        }

        #endregion


        #region --- Private Methods ---

        protected virtual void Track(SwRemoteConfigError error, SwRemoteConfig remoteConfig, SwAbConfig config, Dictionary<string, object> accessibleConfig)
        {
            if (error != SwRemoteConfigError.NoError)
            {
                _tracker.TrackInfraEvent("RemoteConfigError", error.ToString());
            }

            _tracker.TrackInfraEvent("InitComplete");
        }

        private void HandleAgentOnResolve(SwRemoteConfig remoteConfig)
        {
            if (remoteConfig?.agent != null)
            {
                _userData.Country = remoteConfig.agent.country;
            }
        }

        private void HandleNativeEventsOnResolve(SwRemoteConfig remoteConfig, SwAbConfig abConfig)
        {
            var eventsConfiguration = remoteConfig?.events;
            _wisdomNativeAdapter.StoreNativeConfig(eventsConfiguration);
            _wisdomNativeAdapter.UpdateConfig();

            if (abConfig != null)
            {
                _wisdomNativeAdapter.UpdateAbData(abConfig.id, abConfig.key, abConfig.group);
            }

            _wisdomNativeAdapter.UpdateMetadata();
        }

        #endregion


        #region --- Event Handler ---

        /// <summary>
        ///     This method can be overriden in any stage to react differently upon remote config resolve
        /// </summary>
        /// <param name="error"></param>
        /// <param name="remoteConfig"></param>
        /// <param name="abConfig"></param>
        /// <param name="accessibleConfig"></param>
        protected virtual void Handle(SwRemoteConfigError error, SwRemoteConfig remoteConfig, SwAbConfig abConfig, Dictionary<string, object> accessibleConfig)
        {
            ConfigAccessor.Init(abConfig, accessibleConfig);
            HandleAgentOnResolve(remoteConfig);
            HandleNativeEventsOnResolve(remoteConfig, abConfig);
        }

        #endregion
    }
}