#if SW_STAGE_STAGE1_OR_ABOVE
using System.Collections;

namespace SupersonicWisdomSDK
{
    internal class SwStage1InitAppsflyer : ISwAsyncRunnable
    {
        #region --- Members ---

        private readonly SwStage1AppsFlyerAdapter _appsFlyerAdapter;

        #endregion


        #region --- Construction ---

        public SwStage1InitAppsflyer(SwStage1AppsFlyerAdapter appsFlyerAdapter)
        {
            _appsFlyerAdapter = appsFlyerAdapter;
        }

        #endregion


        #region --- Public Methods ---

        public IEnumerator Run ()
        {
            _appsFlyerAdapter.Init();

            yield break;
        }

        #endregion
    }
}
#endif