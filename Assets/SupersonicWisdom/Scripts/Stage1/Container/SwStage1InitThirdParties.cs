#if SW_STAGE_STAGE1_OR_ABOVE
using System.Collections;

namespace SupersonicWisdomSDK
{
    internal class SwStage1InitThirdParties : ISwAsyncRunnable
    {
        #region --- Members ---

        private readonly SwStage1FacebookAdapter _facebookAdapter;
        private readonly SwStage1GameAnalyticsAdapter _gameAnalyticsAdapter;

        #endregion


        #region --- Construction ---

        public SwStage1InitThirdParties(SwStage1FacebookAdapter facebookAdapter, SwStage1GameAnalyticsAdapter gameAnalyticsAdapter)
        {
            _facebookAdapter = facebookAdapter;
            _gameAnalyticsAdapter = gameAnalyticsAdapter;
        }

        #endregion


        #region --- Public Methods ---

        public IEnumerator Run ()
        {
            _facebookAdapter.Init();
            _gameAnalyticsAdapter.Init();

            yield break;
        }

        #endregion
    }
}
#endif