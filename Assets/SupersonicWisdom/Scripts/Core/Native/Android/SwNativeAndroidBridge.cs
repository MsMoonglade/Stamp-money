using System.Collections;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwNativeAndroidBridge : SwNativeBridge
    {
        #region --- Constants ---

        //Fields
        private const string CurrentActivityField = "currentActivity";
        private const string DestroyMethod = "destroy";
        private const string EventMetadataDtoClass = "wisdom.library.domain.events.dto.EventMetadataDto";
        private const string ExtraEventDetailsDtoClass = "wisdom.library.domain.events.dto.ExtraEventDetailsDto";
        private const string GetAdvertisingIdentifierMethod = "getAdvertisingIdentifier";
        private const string GetAppSetIdentifierMethod = "getAppSetIdentifier";
        private const string InitializeSessionMethod = "initializeSession";

        //Methods
        private const string InitMethod = "init";
        private const string RegisterInitListenerMethod = "registerInitListener";
        private const string RegisterSessionListenerMethod = "registerSessionListener";
        private const string SetEventsMetadataMethod = "setEventsMetadata";
        private const string SwBlockingLoaderResourceRelativePathField = "blockingLoaderResourceRelativePath";
        private const string SwBlockingLoaderViewportPercentageField = "blockingLoaderViewportPercentage";
        private const string SwConnectTimeoutField = "connectTimeout";
        private const string SwInitialSyncIntervalField = "initialSyncInterval";
        private const string SwIsLoggingEnabled = "isLoggingEnabled";
        private const string SwReadTimeoutField = "readTimeout";
        private const string SwStreamingAssetsFolderPathField = "streamingAssetsFolderPath";
        private const string SwSubdomainField = "subdomain";
        private const string ToggleBlockingLoaderMethod = "toggleBlockingLoader";
        private const string RequestRateUsPopupMethod = "requestRateUsPopup";
        private const string TrackEventMethod = "trackEvent";

        //Classes
        private const string UnityPlayerClass = "com.unity3d.player.UnityPlayer";
        private const string UnregisterInitListenerMethod = "unregisterInitListener";
        private const string UnregisterSessionListenerMethod = "unregisterSessionListener";
        private const string UpdateEventsMetadataMethod = "updateEventsMetadata";
        private const string UpdateWisdomConfigurationMethod = "updateWisdomConfiguration";
        private const string WisdomConfigurationClass = "wisdom.library.api.dto.WisdomConfigurationDto";
        private const string WisdomSDKClass = "wisdom.library.api.WisdomSDK";

        #endregion


        #region --- Members ---

        private readonly AndroidJavaObject _nativeSdk = new AndroidJavaClass(WisdomSDKClass);
        private readonly SwNativeAndroidInitListener _initListener;
        private readonly SwNativeAndroidSessionListener _sessionListener;

        #endregion


        #region --- Construction ---

        public SwNativeAndroidBridge ()
        {
            _sessionListener = new SwNativeAndroidSessionListener();
            _initListener = new SwNativeAndroidInitListener();
        }

        #endregion


        #region --- Public Methods ---

        public override void Destroy ()
        {
            _nativeSdk.CallStatic(DestroyMethod);
        }

        public override string GetAdvertisingId ()
        {
            var advertisingId = _nativeSdk.CallStatic<string>(GetAdvertisingIdentifierMethod);

            return advertisingId;
        }

        public override string GetOrganizationAdvertisingId ()
        {
            var appSetId = _nativeSdk.CallStatic<string>(GetAppSetIdentifierMethod);

            return appSetId;
        }

        public override void InitializeSession(SwEventMetadataDto metadata)
        {
            _nativeSdk.CallStatic(InitializeSessionMethod, JsonUtility.ToJson(metadata));
        }

        public override IEnumerator InitSdk(SwNativeConfig configuration)
        {
            var didFinishInit = false;
            _initListener.OnInitEnded += () => { didFinishInit = true; };

            _nativeSdk.CallStatic(RegisterInitListenerMethod, _initListener);
            _nativeSdk.CallStatic(InitMethod, GetCurrentActivity(), CreateWisdomConfig(configuration));

            while (!didFinishInit)
            {
                yield return null;
            }

            RegisterSessionListener();

            _nativeSdk.CallStatic(UnregisterInitListenerMethod, _initListener);
        }

        public override void RegisterSessionEndedCallback(OnSessionEnded callback)
        {
            _sessionListener.OnSessionEndedEvent += callback;
        }

        public override void RegisterSessionStartedCallback(OnSessionStarted callback)
        {
            _sessionListener.OnSessionStartedEvent += callback;
        }

        public override void SetEventMetadata(SwEventMetadataDto metadata)
        {
            _nativeSdk.CallStatic(SetEventsMetadataMethod, JsonUtility.ToJson(metadata));
        }

        public override bool ToggleBlockingLoader(bool shouldPresent)
        {
            return _nativeSdk.CallStatic<bool>(ToggleBlockingLoaderMethod, shouldPresent);
        }

        public override void TrackEvent(string eventName, string customsJson, string extraJson)
        {
            _nativeSdk.CallStatic(TrackEventMethod, eventName, customsJson, extraJson);
        }

        public override void UnregisterSessionEndedCallback(OnSessionEnded callback)
        {
            _sessionListener.OnSessionEndedEvent -= callback;
        }

        public override void UnregisterSessionStartedCallback(OnSessionStarted callback)
        {
            _sessionListener.OnSessionStartedEvent -= callback;
        }

        public override void UpdateEventMetadata(SwEventMetadataDto metadata)
        {
            _nativeSdk.CallStatic(UpdateEventsMetadataMethod, JsonUtility.ToJson(metadata));
        }

        public override void UpdateWisdomConfiguration(SwNativeConfig configuration)
        {
            _nativeSdk.CallStatic(UpdateWisdomConfigurationMethod, CreateWisdomConfig(configuration));
        }
        public override void RequestRateUsPopup()
        {
            _nativeSdk.CallStatic(RequestRateUsPopupMethod);
        }
        #endregion


        #region --- Private Methods ---

        private static AndroidJavaObject CreateWisdomConfig(SwNativeConfig config)
        {
            var readTimeoutMillis = config.ReadTimeout * 1000;
            var connectTimeoutMillis = config.ConnectTimeout * 1000;
            var initialSyncIntervalMillis = config.InitialSyncInterval * 1000;

            var nativeConfig = new AndroidJavaObject(WisdomConfigurationClass);
            nativeConfig.Set(SwIsLoggingEnabled, config.IsLoggingEnabled);
            nativeConfig.Set(SwSubdomainField, config.Subdomain);
            nativeConfig.Set(SwReadTimeoutField, readTimeoutMillis);
            nativeConfig.Set(SwConnectTimeoutField, connectTimeoutMillis);
            nativeConfig.Set(SwInitialSyncIntervalField, initialSyncIntervalMillis);
            nativeConfig.Set(SwStreamingAssetsFolderPathField, config.StreamingAssetsFolderPath);
            nativeConfig.Set(SwBlockingLoaderResourceRelativePathField, config.BlockingLoaderResourceRelativePath);
            nativeConfig.Set(SwBlockingLoaderViewportPercentageField, config.BlockingLoaderViewportPercentage);

            return nativeConfig;
        }

        private static AndroidJavaObject GetCurrentActivity ()
        {
            var unityPlayer = new AndroidJavaClass(UnityPlayerClass);

            return unityPlayer.GetStatic<AndroidJavaObject>(CurrentActivityField);
        }

        private void RegisterSessionListener ()
        {
            _nativeSdk.CallStatic(RegisterSessionListenerMethod, _sessionListener);
        }

        private void UnregisterSessionListener ()
        {
            _nativeSdk.CallStatic(UnregisterSessionListenerMethod, _sessionListener);
        }

        #endregion
    }
}