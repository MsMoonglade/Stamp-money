#if SW_STAGE_STAGE1_OR_ABOVE
using System;
using System.Linq;

namespace SupersonicWisdomSDK
{
    internal class SwStage1DeepLinkHandler : SwDeepLinkHandler
    {
        #region --- Constants ---

        private const string InGameConsoleDeepLinkParamName = "inGameConsole";
        private const string TestIdDeepLinkParamName = "testId";

        #endregion


        #region --- Members ---

        private readonly SwSettings _settings;
        private readonly SwStoreKeys _storeKeys;

        #endregion


        #region --- Construction ---

        public SwStage1DeepLinkHandler(SwSettings settings, ISwWebRequestClient webRequestClient, SwStoreKeys storeKeys) : base(settings, webRequestClient)
        {
            _storeKeys = storeKeys;
            _settings = settings;
        }

        #endregion


        #region --- Private Methods ---

        protected override void OnDeepLinkParamsResolve ()
        {
            base.OnDeepLinkParamsResolve();
            SwInfra.Logger.LogViaNetwork = _settings.logViaNetwork;
            HandleDeepLinkTestId();
            HandleDeepLinkInGameConsole();
        }

        private void HandleDeepLinkInGameConsole ()
        {
            if (DeepLinkParams.ContainsKey(InGameConsoleDeepLinkParamName))
            {
                var debugLevel = DeepLinkParams[InGameConsoleDeepLinkParamName];
                SwInfra.Logger.Log($"SwDeepLink | In Game Console enabled with debug level:{debugLevel}");
                SwGameConsole.InitConsole(debugLevel);
            }
        }

        private void HandleDeepLinkTestId ()
        {
            if (DeepLinkParams.ContainsKey(TestIdDeepLinkParamName))
            {
                var newTestId = DeepLinkParams[TestIdDeepLinkParamName];

                // In case of deep link with new testId param, remove old config (overkill) to cancel previous A/B config 
                if (SwInfra.KeyValueStore.HasKey(SwStage1DeepLinkConstants.TestIdStorageKey) && !SwInfra.KeyValueStore.GetString(SwStage1DeepLinkConstants.TestIdStorageKey).Equals(newTestId))
                {
                    SwInfra.KeyValueStore.DeleteKey(_storeKeys.Config);
                }

                // In case of deep link with testId as empty string param delete any previous cached testId
                if (string.IsNullOrEmpty(newTestId))
                {
                    SwInfra.KeyValueStore.DeleteKey(SwStage1DeepLinkConstants.TestIdStorageKey);
                }
                else
                {
                    SwInfra.KeyValueStore.SetString(SwStage1DeepLinkConstants.TestIdStorageKey, newTestId);
                }
            }
        }

        #endregion
    }
}
#endif