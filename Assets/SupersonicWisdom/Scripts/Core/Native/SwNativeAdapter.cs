using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwNativeAdapter : ISwReadyEventListener
    {
        #region --- Constants ---

        private const string EventsRemoteConfigStorageKey = "SupersonicWisdomEventsConfig";

        #endregion


        #region --- Members ---

        protected readonly SwUserData UserData;
        private bool _didFirstSessionStart;

        private string _abId = "";
        private string _abName = "";
        private string _abVariant = "";
        private readonly ISwSessionListener[] _sessionListeners;
        private readonly ISwSettings _settings;

        private readonly SwNativeApi _wisdomNativeApi;

        #endregion


        #region --- Construction ---

        public SwNativeAdapter(SwNativeApi wisdomNativeApi, ISwSettings settings, SwUserData userData, [CanBeNull] ISwSessionListener[] listeners)
        {
            _wisdomNativeApi = wisdomNativeApi;
            _settings = settings;
            UserData = userData;
            _sessionListeners = listeners;
        }

        #endregion


        #region --- Public Methods ---

        public virtual IEnumerator InitNativeSession ()
        {
            _wisdomNativeApi.InitializeSession(GetEventMetadata());

            if (_wisdomNativeApi.IsSupported() && GetEventsConfig().enabled)
            {
                while (!_didFirstSessionStart)
                {
                    yield return null;
                }
            }
        }

        public virtual IEnumerator InitSDK ()
        {
            var eventsConfig = GetEventsConfig();

            if (!eventsConfig.enabled)
            {
                SwInfra.Logger.LogWarning("SwNativeAdapter | InitSDK | " + $"enabled: {eventsConfig.enabled}");

                yield break;
            }

            SwInfra.Logger.Log($"SwNativeAdapter | InitSDK | enabled: {eventsConfig.enabled}");

            yield return _wisdomNativeApi.Init(GetWisdomNativeConfiguration());

            _wisdomNativeApi.AddSessionStartedCallback(OnSessionStarted);
            _wisdomNativeApi.AddSessionEndedCallback(OnSessionEnded);

            UpdateMetadata();
        }

        public void StoreNativeConfig(SwNativeEventsConfig config)
        {
            var jsonConfig = JsonUtility.ToJson(config);

            if (string.IsNullOrEmpty(jsonConfig))
            {
                SwInfra.Logger.Log("SwNativeAdapter.NativeConfig | StoreNativeConfig | config is null");

                return;
            }

            SwInfra.KeyValueStore.SetString(EventsRemoteConfigStorageKey, jsonConfig);
            SwInfra.KeyValueStore.Save();
        }

        public bool ToggleBlockingLoader(bool shouldPresent)
        {
            return _wisdomNativeApi.ToggleBlockingLoader(shouldPresent);
        }

        public void TrackEvent(string eventName, string customsJson, string extraJson)
        {
            SwInfra.Logger.Log($"SwNativeAdapter | TrackEvent | eventName={eventName} | customsJson={customsJson} | extraJson = {extraJson}");
            _wisdomNativeApi.TrackEvent(eventName, customsJson, extraJson);
        }

        public void UpdateAbData(string abId, string abName, string abVariant)
        {
            _abId = abId;
            _abName = abName;
            _abVariant = abVariant;
        }

        public void UpdateConfig ()
        {
            if (GetEventsConfig().enabled)
            {
                _wisdomNativeApi.UpdateWisdomConfiguration(GetWisdomNativeConfiguration());
            }
            else
            {
                _wisdomNativeApi.Destroy();
            }
        }

        public void UpdateMetadata ()
        {
            _wisdomNativeApi.UpdateMetadata(GetEventMetadata());
        }

        public virtual void RequestRateUsPopup()
        {
            _wisdomNativeApi.RequestRateUsPopup();
        }

        public void OnSwReady ()
        {
            // Waiting for readiness for updating appsFlyerId which is available only after appsFlyer init complete
            UpdateMetadata();
        }

        #endregion


        #region --- Private Methods ---

        protected virtual SwNativeEventsConfig GetDefaultConfig ()
        {
            return new SwNativeEventsConfig();
        }

        protected virtual SwEventMetadataDto GetEventMetadata ()
        {
            var attStatus = SwAttUtils.GetStatus();

            var eventMetadata = new SwEventMetadataDto
            {
                bundle = UserData.BundleIdentifier,
                os = UserData.Platform,
                osVer = SystemInfo.operatingSystem,
                uuid = UserData.Uuid,
                swInstallationId = UserData.CustomUuid,
                device = SystemInfo.deviceModel,
                version = Application.version,
                sdkVersion = SwConstants.SdkVersion,
                sdkVersionId = SwConstants.SdkVersionId,
                sdkStage = SwStageUtils.CurrentStage.sdkStage.ToString(),
                installDate = UserData.InstallDate,
                apiKey = _settings.GetAppKey(),
                gameId = _settings.GetGameId(),
                feature = SwConstants.Feature,
                featureVersion = SwConstants.FeatureVersion,
                unityVersion = SwUtils.UnityVersion,
                attStatus = attStatus == SwAttAuthorizationStatus.Unsupported ? "" : $"{attStatus}",
                abId = _abId,
                abName = _abName,
                abVariant = _abVariant
            };

            var organizationAdvertisingId = UserData.OrganizationAdvertisingId;
#if UNITY_IOS
            eventMetadata.sandbox = SwUtils.IsIosSandbox ? "1" : "0";
            eventMetadata.idfv = organizationAdvertisingId;
#endif
#if UNITY_ANDROID
            eventMetadata.appSetId = organizationAdvertisingId;
#endif

            return eventMetadata;
        }

        protected virtual SwNativeEventsConfig GetEventsConfig ()
        {
            var jsonConfig = SwInfra.KeyValueStore.GetString(EventsRemoteConfigStorageKey, null);

            return JsonUtility.FromJson<SwNativeEventsConfig>(jsonConfig) ?? GetDefaultConfig();
        }

        protected virtual string GetSubdomain ()
        {
            var version = Application.version.Replace('.', '-');

            return $"{version}-{_settings.GetGameId()}";
        }

        protected virtual SwNativeConfig GetWisdomNativeConfiguration ()
        {
            return CreateWisdomNativeConfiguration();
        }

        private SwNativeConfig CreateWisdomNativeConfiguration ()
        {
            var config = GetEventsConfig();
            var blockingLoaderResourceRelativePath = SwUtils.IsRunningOnIos() ? "SupersonicWisdom/LoaderFrames" : "SupersonicWisdom/LoaderGif/animated_loader.gif";

            return new SwNativeConfig
            {
                Subdomain = GetSubdomain(),
                ConnectTimeout = config.connectTimeout,
                ReadTimeout = config.readTimeout,
                IsLoggingEnabled = _settings.IsDebugEnabled(),
                InitialSyncInterval = config.initialSyncInterval,
                StreamingAssetsFolderPath = Application.streamingAssetsPath,
                BlockingLoaderResourceRelativePath = blockingLoaderResourceRelativePath,
                BlockingLoaderViewportPercentage = 20
            };
        }

        private void OnSessionEnded(string sessionId)
        {
            if (_sessionListeners == null) return;

            foreach (var sessionListener in _sessionListeners)
            {
                sessionListener.OnSessionEnded(sessionId);
            }
        }

        private void OnSessionStarted(string sessionId)
        {
            if (_sessionListeners != null)
            {
                foreach (var sessionListener in _sessionListeners)
                {
                    sessionListener.OnSessionStarted(sessionId);
                }
            }

            if (!_didFirstSessionStart)
            {
                _didFirstSessionStart = true;
            }
        }

        #endregion
    }
}