using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

namespace SupersonicWisdomSDK
{
    internal abstract class SwCoreContainer : ISwContainer
    {
        protected readonly ISwInitParams InitParams;
        protected readonly SwCoreMonoBehaviour Mono;
        private readonly ISwAsyncCatchableRunnable _stageSpecificCustomInitRunnable;
        private readonly SwGameBlocker _gameBlocker;
        protected internal readonly SwSettings Settings;
        protected readonly SwNativeAdapter WisdomNativeAdapter;
        protected internal readonly SwDeepLinkHandler DeepLinkHandler;
        protected internal readonly SwLocalConfigHandler LocalConfigHandler;
        protected readonly SwDevTools DevTools;
        protected internal readonly SwUserData UserData;
        protected readonly SwCoreTracker Tracker;
        protected internal readonly ISwRemoteConfigRepository RemoteConfigRepository;
        protected internal readonly ISwConfigAccessor ConfigAccessor;
        private readonly ISwAdapter[] _swAdapters;
        private readonly ISwReadyEventListener[] _readyEventListeners;
        private readonly ISwUserStateListener[] _userStateListeners;
        private readonly ISwLocalConfigProvider[] _configProviders;
        protected internal SwTimerManager TimerManager;
        private bool _isBlocked;

        protected bool WasDestroyed;
        public bool IsReady { get; private set; }

        private bool IsBlocked
        {
            get { return _isBlocked; }
            set { _isBlocked |= value; }
        }

        public event OnReady OnReadyEvent;

        public SwCoreContainer(Dictionary<string, object> initParamsDictionary, SwCoreMonoBehaviour mono, ISwAsyncCatchableRunnable stageSpecificCustomInitRunnable, SwSettingsManager<SwSettings> settingsManager, ISwReadyEventListener[] readyEventListeners, ISwUserStateListener[] userStateListeners, ISwLocalConfigProvider[] configProviders, ISwAdapter[] swAdapters, SwNativeAdapter wisdomNativeAdapter, SwDeepLinkHandler deepLinkHandler, SwLocalConfigHandler localConfigHandler, SwDevTools devTools, SwUserData userData, SwCoreTracker tracker, ISwRemoteConfigRepository remoteConfigRepository, ISwConfigAccessor configAccessor, SwTimerManager timerManager, SwGameBlocker gameBlocker)
        {
            // ReSharper disable VirtualMemberCallInConstructor
            InitParams = CreateInitParams();
            PopulateInitParams(initParamsDictionary ?? new Dictionary<string, object>());
            // ReSharper restore VirtualMemberCallInConstructor

            _stageSpecificCustomInitRunnable = stageSpecificCustomInitRunnable;
            _readyEventListeners = readyEventListeners;
            _swAdapters = swAdapters;
            
            Settings = settingsManager.Settings;
            WisdomNativeAdapter = wisdomNativeAdapter;
            DeepLinkHandler = deepLinkHandler;
            LocalConfigHandler = localConfigHandler;
            DevTools = devTools;
            UserData = userData;
            Tracker = tracker;
            _configProviders = configProviders;
            RemoteConfigRepository = remoteConfigRepository;
            ConfigAccessor = configAccessor;
            TimerManager = timerManager;

            if (userStateListeners != null)
            {
                foreach (var userStateListener in userStateListeners)
                {
                    UserData.OnUserStateChangeEvent += userStateListener.OnUserStateChange;
                }
            }

            Mono = mono;
            Mono.LifecycleListener = this;
            _gameBlocker = gameBlocker;
        }

        public SwCoreMonoBehaviour GetMono ()
        {
            return Mono;
        }

        public static ISwContainer GetInstance(Dictionary<string, object> initParamsDictionary)
        {
            throw new NotImplementedException();
        }

        public abstract ISwInitParams CreateInitParams ();

        public virtual void PopulateInitParams(Dictionary<string, object> initParamsDictionary)
        { }

        public virtual void OnAwake ()
        {
            SwContainerUtils.InitContainerAsync(this, _stageSpecificCustomInitRunnable);
        }

        public virtual IEnumerator InitAsync ()
        {
            yield return DeepLinkHandler.SetupDeepLink();
            DevTools.EnableDevtools(Debug.isDebugBuild || Settings.enableDevtools);
            LocalConfigHandler.Setup(_configProviders);

            yield return WisdomNativeAdapter.InitSDK();
            RemoteConfigRepository.Init();
            UserData.Load(InitParams);

            yield return WisdomNativeAdapter.InitNativeSession();
            Tracker.TrackInfraEvent("AppOpen", UserData.IsNew ? "first" : "");
        }

        public virtual void AfterInit(Exception exception)
        {
            if (exception != null)
            {
                SwInfra.Logger.LogError($"Container init error: {exception.Message}\n{exception.StackTrace}");
                // @todo [next] log error to (some) cloud so it can be monitored
            }

            SwInfra.CoroutineService.StartCoroutine(NotifyReadiness());
        }

        protected virtual IEnumerator BeforeReady ()
        {
            IsBlocked = RemoteConfigRepository.Unavailable;

            yield break;
        }

        protected IEnumerator NotifyReadiness ()
        {
            if (IsReady || IsBlocked) yield break;

            yield return BeforeReady();
            
            if (IsBlocked)
            {
                _gameBlocker.ShowPopup(ConfigAccessor.GetConfigValue(SwGameBlocker.AvailabilityMessageKey, null));

                yield break;
            }

            IsReady = true;

            try
            {
                foreach (var readyEventListener in _readyEventListeners)
                {
                    readyEventListener?.OnSwReady();
                }

                Tracker.TrackInfraEvent("Ready", GetAdapterVersionAndStatus());
                SwInfra.Logger.Log("SwCoreContainer | OnReadyEvent");
                OnReadyEvent?.Invoke();
            }
            catch (Exception e)
            {
                SwInfra.Logger.LogError($"SwCoreContainer | OnReadyEvent | Error | {e.Message}");
                Tracker.TrackInfraEvent("ReadyError", e.Message, e.Source);
            }
        }

        public abstract void OnStart ();

        public virtual void OnUpdate ()
        {
            DevTools.OnUpdate();
        }

        public abstract void OnApplicationPause(bool pauseStatus);

        public abstract void OnApplicationQuit ();

        public virtual void Destroy ()
        {
            WasDestroyed = true;
        }

        public bool IsDestroyed ()
        {
            return WasDestroyed;
        }
        
        public List<string> Validate ()
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            var validationErrors = new List<string>();
#if UNITY_IOS && !UNITY_EDITOR
            var skAdNetworks = _swGetSkAdNetworks();
            if (string.IsNullOrEmpty(skAdNetworks))
            {
                validationErrors.Add($"Missing {SwAttributionConstants.SkAdNetworkItemsKey} in Info.plist. Make sure internet access is available when building the unity project.");
            }

            var advertisingAttributionReportEndpoint = _swGetAdvertisingAttributionReportEndpoint();
            if (string.IsNullOrEmpty(advertisingAttributionReportEndpoint))
            {
                validationErrors.Add($"{SwAttributionConstants.AdvertisingAttributionReportEndpointKey} in Info.plist does not equal to {SwAttributionConstants.AdvertisingAttributionReportEndpoint}");
            }
#endif
            return validationErrors;
        }

        private string GetAdapterVersionAndStatus()
        {
            if (_swAdapters.Length <= 0) return string.Empty;
            var listOfAdapters = new List<SwAdapterData>();

            foreach (var adapter in _swAdapters)
            {
                listOfAdapters.Add(adapter.GetAdapterStatusAndVersion());
            }

            return JsonConvert.SerializeObject(listOfAdapters);
        }
        
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string _swGetSkAdNetworks();

        [DllImport("__Internal")]
        private static extern string _swGetAdvertisingAttributionReportEndpoint();
#endif
    }
}