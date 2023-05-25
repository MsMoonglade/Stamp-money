using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwCoreRemoteConfigRepository : ISwRemoteConfigRepository
    {
        #region --- Constants ---

        private const int ConfigRequestTimeout = 2;

        private const string ApiVersion = "2";
        private const string ConfigEndPoint = "https://config.mobilegamestats.com/";

        #endregion


        #region --- Events ---

        private event OnLoaded OnLoadedEvent;

        #endregion


        #region --- Members ---

        private readonly ISwRemoteConfigListener _listener;
        private readonly ISwSettings _settings;
        private readonly ISwWebRequestClient _webRequestClient;
        private readonly SwCoreTracker _tracker;
        private readonly SwStoreKeys _storeKeys;
        private readonly SwUserData _userData;
        private bool _didInit;

        private bool _didResolve;
        private SwAbConfig _ab;

        #endregion


        #region --- Properties ---

        /// <summary>
        ///     Availabilty
        /// </summary>
        public bool Unavailable { get; private set; }

        /// <summary>
        ///     Config dictionary for developer and wisdom internal usage.
        ///     Does not contain AB config values.
        /// </summary>
        public Dictionary<string, object> AccessibleConfig { get; private set; }

        /// <summary>
        ///     The initial remote config for new user
        ///     In case new user failed to get remote config this value will always be null for life
        /// </summary>
        public SwRemoteConfig InitialRemoteConfig { get; private set; }

        /// <summary>
        ///     The latest successful remote config for a returning user
        ///     In case the user failed to get remote config this value will be it's predecessor
        /// </summary>
        public SwRemoteConfig LatestRemoteConfig { get; private set; }

        /// <summary>
        ///     Current remote config for this session.
        /// </summary>
        public SwRemoteConfig RemoteConfig { get; protected set; }

        /// <summary>
        ///     The remote config error after fetching if happened
        /// </summary>
        public SwRemoteConfigError RemoteConfigError { get; private set; }

        #endregion


        #region --- Construction ---

        public SwCoreRemoteConfigRepository(ISwSettings settings, SwUserData userData, ISwWebRequestClient webRequestClient, SwCoreTracker tracker, SwStoreKeys storeKeys, ISwRemoteConfigListener listener)
        {
            _webRequestClient = webRequestClient;
            _userData = userData;
            _settings = settings;
            _tracker = tracker;
            _storeKeys = storeKeys;
            _listener = listener;
        }

        #endregion


        #region --- Public Methods ---

        public void AddOnLoadedListener(OnLoaded onLoadedCallback)
        {
            OnLoadedEvent += onLoadedCallback;
        }

        public bool DidResolve ()
        {
            return _didResolve;
        }

        public IEnumerator Fetch ()
        {
            if (!_didInit)
            {
                throw new SwException("SwCoreRemoteConfig | Fetch | Init method must be called before calling Fetch.");
            }

            var counter = new SwMillisecondsCounter();
            var response = new SwWebResponse();

            yield return _webRequestClient.Post(ConfigEndPoint, CreatePayload(), response, ConfigRequestTimeout, null, true);
            _tracker.TrackInfraEvent("ServerResponse", counter.Measure().ToString(), response.code.ToString(), response.error.ToString());
            HandleResponse(response);
        }

        /// <summary>
        ///     Ab Config
        /// </summary>
        public SwAbConfig GetAb ()
        {
            return _ab;
        }

        public void Init ()
        {
            var initialRemoteConfigStr = SwInfra.KeyValueStore.GetString(_storeKeys.Config);
            InitialRemoteConfig = DeserializeRemoteConfig(initialRemoteConfigStr);
            var latestRemoteConfigStr = SwInfra.KeyValueStore.GetString(_storeKeys.LatestSuccessfulConfigResponse);
            LatestRemoteConfig = DeserializeRemoteConfig(latestRemoteConfigStr);
            _didInit = true;
        }

        public void RemoveOnLoadedListener(OnLoaded onLoadedCallback)
        {
            OnLoadedEvent -= onLoadedCallback;
        }

        #endregion


        #region --- Private Methods ---

        private static SwRemoteConfigError ResolveRemoteConfigError(SwWebRequestError webRequestError, Dictionary<string, object> accessibleConfig, SwAbConfig abConfig)
        {
            switch (webRequestError)
            {
                case SwWebRequestError.None:
                    return SwRemoteConfigError.NoError;
                case SwWebRequestError.Http:
                case SwWebRequestError.Network:
                    return SwRemoteConfigError.Network;
                case SwWebRequestError.Timeout:
                    return SwRemoteConfigError.RequestTimeout;
            }

            // in case there are no available values in both ab config & the accessible config
            if ((abConfig == null || !abConfig.IsValid) && accessibleConfig.Count == 0)
            {
                return SwRemoteConfigError.Empty;
            }

            return default;
        }

        protected virtual SwRemoteConfigRequestPayload CreatePayload ()
        {
            var organizationAdvertisingId = _userData.OrganizationAdvertisingId;

            return new SwRemoteConfigRequestPayload
            {
                bundle = _userData.BundleIdentifier,
                gameId = _settings.GetGameId(),
                os = _userData.Platform,
                osver = SystemInfo.operatingSystem,
                uuid = _userData.Uuid,
                session = _userData.ImmutableUserState().SessionId,
                device = SystemInfo.deviceModel,
                version = Application.version,
                sdkVersion = SwConstants.SdkVersion,
                sdkVersionId = SwConstants.SdkVersionId,
                stage = SwStageUtils.CurrentStage.sdkStage,
                sysLang = Application.systemLanguage.ToString(),
                isNew = _userData.IsNew ? "0" : "1",
                apiVersion = ApiVersion,
#if UNITY_IOS
                idfv = organizationAdvertisingId,
#endif
#if UNITY_ANDROID
                appSetId = organizationAdvertisingId,
#endif
            };
        }

        protected virtual SwRemoteConfig InstantiateRemoteConfig(string remoteConfigStr)
        {
            return JsonUtility.FromJson<SwRemoteConfig>(remoteConfigStr);
        }

        private SwRemoteConfig DeserializeRemoteConfig(string remoteConfigStr)
        {
            SwRemoteConfig remoteConfig = null;

            try
            {
                remoteConfig = InstantiateRemoteConfig(remoteConfigStr);
            }
            catch (Exception e)
            {
                SwInfra.Logger.LogError($"SwCoreRemoteConfigRepository | DeserializeRemoteConfig | parse error | {e.Message} | {remoteConfigStr}");
            }

            remoteConfig?.SetDynamicData(remoteConfigStr);

            return remoteConfig;
        }

        private void HandleResponse(SwWebResponse response)
        {
            RemoteConfig = DeserializeRemoteConfig(response.Text);

            var responseIsValid = response.DidSucceed && RemoteConfig != null;

            if (responseIsValid)
            {
                if (_userData.IsNew)
                {
                    SaveInitialResponse(RemoteConfig, response.Text);
                    // At this point RemoteConfig == InitialRemoteConfig
                }
                else
                {
                    SaveLatestSuccessfulConfigResponse(RemoteConfig, response.Text);
                    // Config is replaced with the fresh values from the response
                }
            }

            // Ab is retrieved only from initial remote config
            _ab = InitialRemoteConfig?.ab ?? new SwAbConfig();
            Unavailable = RemoteConfig?.unavailable ?? false;

            //Guard in case the first launch is done with the remote config being null
            AccessibleConfig =
                new Dictionary<string, object>().SwMerge(LatestRemoteConfig?.AccessibleConfig,
                    RemoteConfig?.AccessibleConfig);            

            RemoteConfigError = ResolveRemoteConfigError(response.error, AccessibleConfig, _ab);

            _listener.OnResolve(RemoteConfigError, RemoteConfig, _ab, AccessibleConfig);

            _didResolve = true;

            try
            {
                if (RemoteConfigError == SwRemoteConfigError.NoError)
                {
                    SwInfra.Logger.Log(() => $"SwRemoteConfigRepository | OnLoadedEvent | {JsonUtility.ToJson(AccessibleConfig)}");
                }

                OnLoadedEvent?.Invoke(RemoteConfigError == SwRemoteConfigError.NoError, RemoteConfigError);
            }
            catch (Exception e)
            {
                _tracker.TrackInfraEvent("HostAppErrorOnCallback", e.Message, e.StackTrace);
                SwInfra.Logger.LogError(e.Message);
            }
        }

        private void SaveInitialResponse(SwRemoteConfig remoteConfig, string responseText)
        {
            InitialRemoteConfig = remoteConfig;
            SwInfra.KeyValueStore.SetString(_storeKeys.Config, responseText);
        }

        private void SaveLatestSuccessfulConfigResponse(SwRemoteConfig remoteConfig, string responseText)
        {
            LatestRemoteConfig = remoteConfig;
            SwInfra.KeyValueStore.SetString(_storeKeys.LatestSuccessfulConfigResponse, responseText);
        }

        #endregion
    }
}