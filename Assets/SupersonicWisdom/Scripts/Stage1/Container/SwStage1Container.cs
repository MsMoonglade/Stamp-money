#if SW_STAGE_STAGE1_OR_ABOVE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace SupersonicWisdomSDK
{
    internal class SwStage1Container : SwCoreContainer
    {
        #region --- Members ---

        protected internal readonly SwBlockingApiHandler BlockingApiHandler;
        protected internal readonly SwStage1FacebookAdapter FacebookAdapter;
        protected internal readonly SwStage1GameAnalyticsAdapter GameAnalyticsAdapter;
        protected internal readonly SwStage1Tracker Stage1Tracker;

        #endregion


        #region --- Construction ---

        internal SwStage1Container(Dictionary<string, object> initParamsDictionary, SwStage1MonoBehaviour mono, ISwAsyncCatchableRunnable stageSpecificCustomInitRunnable, SwSettingsManager<SwSettings> settingsManager, ISwReadyEventListener[] readyEventListeners, ISwUserStateListener[] userStateListeners, ISwLocalConfigProvider[] configProviders, ISwAdapter[] coreAdapters, SwStage1NativeAdapter wisdomNativeAdapter, SwStage1DeepLinkHandler deepLinkHandler, SwLocalConfigHandler localConfigHandler, SwDevTools devTools, SwUserData userData, SwStage1Tracker tracker, ISwRemoteConfigRepository remoteConfigRepository, ISwConfigAccessor configAccessor, SwBlockingApiHandler blockingApiHandler, SwStage1AppsFlyerAdapter appsFlyerAdapter, SwStage1FacebookAdapter facebookAdapter, SwStage1GameAnalyticsAdapter gameAnalyticsAdapter, SwTimerManager timerManager, SwGameBlocker gameBlocker) : base(initParamsDictionary, mono, stageSpecificCustomInitRunnable, settingsManager, readyEventListeners, userStateListeners, configProviders, coreAdapters, wisdomNativeAdapter, deepLinkHandler, localConfigHandler, devTools, userData, tracker, remoteConfigRepository, configAccessor, timerManager, gameBlocker)
        {
            BlockingApiHandler = blockingApiHandler;
            FacebookAdapter = facebookAdapter;
            GameAnalyticsAdapter = gameAnalyticsAdapter;
            FacebookAdapter.OnFacebookInitCompleteEvent += OnFacebookInitComplete;
            Stage1Tracker = tracker;
        }

        #endregion


        #region --- Mono Override ---

        public override void OnApplicationPause(bool pauseStatus)
        {
            FacebookAdapter.OnApplicationPause(pauseStatus);
            SwInfra.Logger.Log($"SwStage1Container | OnApplicationPause | {pauseStatus}");
        }

        public override void OnApplicationQuit ()
        {
            SwInfra.Logger.Log("SwStage1Container | OnApplicationQuit");
        }

        #endregion


        #region --- Public Methods ---

        [Preserve]
        public new static ISwContainer GetInstance(Dictionary<string, object> initParamsDictionary)
        {
            var mono = SwContainerUtils.InstantiateSupersonicWisdom<SwStage1MonoBehaviour>("Stage1/SupersonicWisdomStage1");
            SwInfra.InitializeCoreDefaults(mono);

            var filesCacheManager = new SwFilesCacheManager();
            var settingsManager = new SwSettingsManager<SwSettings>(SwInfra.KeyValueStore);
            var storeKeys = new SwStoreKeys();
            var wisdomNativeApi = SwNativeApiFactory.GetInstance();
            var userData = new SwStage1UserData(settingsManager.Settings, storeKeys, wisdomNativeApi);
            var sessionListener = new SwStage1SessionListener(userData);
            ISwSessionListener[] swSessionListeners = { sessionListener };
            var wisdomNativeAdapter = new SwStage1NativeAdapter(wisdomNativeApi, settingsManager.Settings, userData, swSessionListeners);
            var webRequestClient = new SwUnityWebRequestClient();
            var deepLinkHandler = new SwStage1DeepLinkHandler(settingsManager.Settings, webRequestClient, storeKeys);
            var localConfigHandler = new SwLocalConfigHandler();
            var devTools = new SwDevTools(filesCacheManager);
            var timerManager = new SwTimerManager(mono);
            var tracker = new SwStage1Tracker(wisdomNativeAdapter, userData, webRequestClient, timerManager);
            var configAccessor = new SwConfigAccessor(deepLinkHandler, localConfigHandler);
            var appsFlyerEventDispatcher = mono.GetComponent<SwAppsFlyerEventDispatcher>();
            var appsFlyerAdapter = new SwStage1AppsFlyerAdapter(appsFlyerEventDispatcher, userData, settingsManager, tracker);
            var facebookAdapter = new SwStage1FacebookAdapter();
            var gameAnalyticsAdapter = new SwStage1GameAnalyticsAdapter();
            var blockingApiHandler = new SwBlockingApiHandler(settingsManager.Settings, tracker, userData, null, new ISwGameProgressionListener[] { gameAnalyticsAdapter, tracker, timerManager });
            var remoteConfigListener = new SwStage1RemoteConfigListener(settingsManager.Settings, configAccessor, wisdomNativeAdapter, tracker, userData, appsFlyerAdapter);
            var remoteConfigRepository = new SwStage1RemoteConfigRepository(settingsManager.Settings, userData, webRequestClient, tracker, storeKeys, remoteConfigListener);
            var gameBlocker = new SwGameBlocker(tracker);

            var initThirdPartiesStep = new SwStage1InitThirdParties(facebookAdapter, gameAnalyticsAdapter);
            var loadRemoteConfigStep = new SwStage1FetchRemoteConfig(remoteConfigRepository);
            var initAppsflyerStep = new SwStage1InitAppsflyer(appsFlyerAdapter);

            var stageSpecificCustomInitRunnable = new SwAsyncFlow(new[]
            {
                new SwAsyncFlowStep(loadRemoteConfigStep, 0),
                new SwAsyncFlowStep(initThirdPartiesStep, 0),
                //We excluded the initialization of AppsFlyer due to a dependency in a remote config value that determines the AF hostname.
                new SwAsyncFlowStep(initAppsflyerStep, 1)
            });

            ISwAdapter[] swAdapters = { appsFlyerAdapter, gameAnalyticsAdapter, facebookAdapter };
            ISwReadyEventListener[] readyEventListeners = { appsFlyerAdapter, wisdomNativeAdapter };
            ISwUserStateListener[] userStateListeners = { };
            ISwLocalConfigProvider[] configProviders = { appsFlyerAdapter };

            return new SwStage1Container(initParamsDictionary, mono, stageSpecificCustomInitRunnable, settingsManager, readyEventListeners, userStateListeners, configProviders, swAdapters, wisdomNativeAdapter, deepLinkHandler, localConfigHandler, devTools, userData, tracker, remoteConfigRepository, configAccessor, blockingApiHandler, appsFlyerAdapter, facebookAdapter, gameAnalyticsAdapter, timerManager, gameBlocker);
        }

        public override ISwInitParams CreateInitParams ()
        {
            return new SwStage1InitParams();
        }

        public override void OnAwake ()
        {
            base.OnAwake();
            SwInfra.Logger.Log("SwStage1Container | OnAwake");
        }

        public override void OnStart ()
        {
            SwInfra.Logger.Log("SwStage1Container | OnStart");
        }

        public override void PopulateInitParams(Dictionary<string, object> initParamsDictionary)
        {
            base.PopulateInitParams(initParamsDictionary);
        }

        #endregion


        #region --- Private Methods ---

        protected override IEnumerator BeforeReady ()
        {
            yield return base.BeforeReady();
            yield return BlockingApiHandler.NotifyGameStarted();
        }

        internal SwUserState CopyOfUserState ()
        {
            return UserData.ImmutableUserState();
        }

        private IEnumerator VerifyFirstLaunchWithApple()
        {
            yield return new WaitForSeconds(1);
            SwSKAdNetworkAdapter.UpdatePostbackConversionValue( 
                conversionValue: 0,
                callback: authorizationStatusString => Stage1Tracker.TrackConversionValueEvent(0, authorizationStatusString));
        }

        #endregion


        #region --- Event Handler ---

        private void OnFacebookInitComplete ()
        {
            if (SwUtils.IsRunningOnIos() && UserData.IsNew)
            {
                SwInfra.CoroutineService.StartCoroutine(VerifyFirstLaunchWithApple());
            }
        }

        #endregion
    }
}
#endif