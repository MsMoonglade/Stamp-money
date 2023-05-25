#if SW_STAGE_STAGE1_OR_ABOVE
using System;
using System.Collections.Generic;
using AppsFlyerSDK;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal static class SwStage1AppsFlyerConstants
    {
        #region --- Constants ---

        public const string AppsFlyerInitInternalEventName = "AppsFlyerInit";

        #endregion
    }

    internal class SwStage1AppsFlyerAdapter : ISwReadyEventListener, ISwAppsFlyerListener, ISwLocalConfigProvider, ISwAdapter
    {
        #region --- Constants ---

        /// <summary>
        ///     This key is read by Wisdom Native. Do not rename it.
        /// </summary>
        private const string AppsFlyerConversionDataKey = "AFConversionData";

        private const string AppsFlyerDevKey = "nYwfftoacbopmuszWBPGnd";

        #endregion


        #region --- Members ---

        protected readonly SwCoreTracker Tracker;
        protected bool DidApplyAppsFlyerAnonymize;
        protected bool DidAppsFlyerRespond;

        protected bool IsSwReady;
        protected bool? ShouldAnonymizeAppsFlyer = null;

        private readonly SwAppsFlyerEventDispatcher _eventDispatcher;
        private readonly SwSettingsManager<SwSettings> _settingsManager;
        private readonly SwStage1UserData _userData;

        private bool _didInitAppsFlyer;
        private bool _didTrackConversionDataFail;
        private ISwAppsFlyerListener _listener;
        private string _appsFlyerHostname;
        private string _conversionDataFailError;
        private string _sdkStatus;

        #endregion


        #region --- Construction ---

        public SwStage1AppsFlyerAdapter(SwAppsFlyerEventDispatcher eventDispatcher, SwStage1UserData userData, SwSettingsManager<SwSettings> settingsManager, SwCoreTracker tracker)
        {
            _eventDispatcher = eventDispatcher;
            _settingsManager = settingsManager;
            _userData = userData;
            Tracker = tracker;
        }

        #endregion


        #region --- Public Methods ---

        public virtual void Init ()
        {
            if (_didInitAppsFlyer)
            {
                return;
            }

            _didInitAppsFlyer = true;
            SwInfra.Logger.Log("AppsFlyer | init");

            try
            {
                _eventDispatcher.AddListener(this);

                if (!string.IsNullOrEmpty(_appsFlyerHostname))
                {
                    AppsFlyer.setHost("", _appsFlyerHostname);
                }

                AppsFlyer.setIsDebug(_settingsManager.Settings.enableDebug);
                AppsFlyer.setCurrencyCode("USD");
                AppsFlyer.initSDK(AppsFlyerDevKey, _settingsManager.Settings.IosAppId, _eventDispatcher);
                SwInternalEvent.Invoke(SwStage1AppsFlyerConstants.AppsFlyerInitInternalEventName);
                AppsFlyer.startSDK();
                
                _userData.AppsFlyerId = AppsFlyer.getAppsFlyerId();
                _didInitAppsFlyer = true;
                _sdkStatus = _didInitAppsFlyer.ToString();

            }
            catch (Exception e)
            {
                SwInfra.Logger.LogError(e.Message);
                _sdkStatus = e.Message;
            }
        }

        public Dictionary<string, string> CreateEventValues ()
        {
            var organizationAdvertisingId = _userData.OrganizationAdvertisingId;
            var eventValues = new Dictionary<string, string>();

            if (SwUtils.IsRunningOnAndroid())
            {
                eventValues["appSetId"] = organizationAdvertisingId;
            }
            else if (SwUtils.IsRunningOnIos())
            {
                eventValues["idfv"] = organizationAdvertisingId;
            }

            eventValues["af_sw_stage"] = SwStageUtils.CurrentStage.sdkStage.ToString();

            return eventValues;
        }

        public SwLocalConfig GetLocalConfig ()
        {
            return new SwStage1AppsFlyerLocalConfig();
        }
        
        public SwAdapterData GetAdapterStatusAndVersion()
        {
            var adapterData = new SwAdapterData
            {
                adapterName = nameof(AppsFlyer),
                adapterStatus = _sdkStatus,
                adapterVersion = AppsFlyer.getSdkVersion()
            };

            return adapterData;
        }

        public void onAppOpenAttribution(string attributionData)
        {
            AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
            SwInfra.Logger.Log($"onAppOpenAttribution | {attributionData}");
        }

        public void onAppOpenAttributionFailure(string error)
        {
            AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
        }

        public void onConversionDataFail(string error)
        {
            AppsFlyer.AFLog("didReceiveConversionDataWithError", error);
            _conversionDataFailError = error;
            TrackConversionDataFailIfNeeded();
        }

        public void onConversionDataSuccess(string conversionData)
        {
            AppsFlyer.AFLog("didReceiveConversionData", conversionData);

            // This key is being read in wisdom native for it to be sent in all events
            SwInfra.KeyValueStore.SetString(AppsFlyerConversionDataKey, conversionData);
        }

        public void OnSwReady ()
        {
            IsSwReady = true;
            TrackConversionDataFailIfNeeded();
        }

        public void SetHost(string appsflyerHostname)
        {
            _appsFlyerHostname = appsflyerHostname;
        }

        #endregion


        #region --- Private Methods ---

        protected void AnonymizeIfNeeded ()
        {
            if (!DidApplyAppsFlyerAnonymize && ShouldAnonymizeAppsFlyer != null)
            {
                try
                {
                    AppsFlyer.anonymizeUser((bool)ShouldAnonymizeAppsFlyer);
                    SwInfra.Logger.Log($"Privacy | {SwPrivacyPolicy.Gdpr} | AppsFlyer | anonymizeUser | {(bool)ShouldAnonymizeAppsFlyer}");
                    DidApplyAppsFlyerAnonymize = true;
                }
                catch (Exception e)
                {
                    SwInfra.Logger.LogError(e.Message);
                }
            }
        }

        protected void SendEvent(string eventName, Dictionary<string, string> eventValues)
        {
            AppsFlyer.sendEvent(eventName, CreateEventValues().SwMerge(eventValues));
        }

        private void TrackConversionDataFailIfNeeded ()
        {
            if (!_didTrackConversionDataFail && IsSwReady && !string.IsNullOrEmpty(_conversionDataFailError))
            {
                _didTrackConversionDataFail = true;
                Tracker.TrackInfraEvent("AFConversionFailure", _conversionDataFailError);
            }
        }

        #endregion


        #region --- Event Handler ---

        public void OnAppsFlyerRequestResponse(object sender, EventArgs args)
        {
            if (args is AppsFlyerRequestEventArgs appsFlyerArgs && !DidAppsFlyerRespond)
            {
                DidAppsFlyerRespond = true;
                SwInfra.Logger.Log($"AppsFlyer | OnAppsFlyerRequestResponse | statusCode = {appsFlyerArgs.statusCode}");
                AnonymizeIfNeeded();
            }
        }

        #endregion
    }
}
#endif