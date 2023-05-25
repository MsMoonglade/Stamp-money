#if SW_STAGE_STAGE1_OR_ABOVE
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal class SwStage1RemoteConfigListener : SwCoreRemoteConfigListener
    {
        #region --- Constants ---

        private const string AppsFlyerDefaultDomain = "appsflyersdk.com";
        private const string AppsFlyerDomain = "appsFlyerDomain";

        #endregion


        #region --- Members ---

        private readonly SwStage1AppsFlyerAdapter _appsFlyerAdapter;

        #endregion


        #region --- Construction ---

        public SwStage1RemoteConfigListener(ISwSettings settings, ISwConfigAccessor configAccessor, SwNativeAdapter wisdomNativeAdapter, SwCoreTracker tracker, SwUserData userData, SwStage1AppsFlyerAdapter appsFlyerAdapter) : base(settings, configAccessor, wisdomNativeAdapter, tracker, userData)
        {
            _appsFlyerAdapter = appsFlyerAdapter;
        }

        #endregion


        #region --- Private Methods ---

        private void HandleAppsFlyerDomainAdjustment ()
        {
            var appsFlyerHostname = ConfigAccessor.GetConfigValue(AppsFlyerDomain, AppsFlyerDefaultDomain);
            _appsFlyerAdapter.SetHost(appsFlyerHostname);
        }

        #endregion


        #region --- Event Handler ---

        protected override void Handle(SwRemoteConfigError error, SwRemoteConfig remoteConfig, SwAbConfig abConfig, Dictionary<string, object> accessibleConfig)
        {
            base.Handle(error, remoteConfig, abConfig, accessibleConfig);
            HandleAppsFlyerDomainAdjustment();
        }

        #endregion
    }
}
#endif