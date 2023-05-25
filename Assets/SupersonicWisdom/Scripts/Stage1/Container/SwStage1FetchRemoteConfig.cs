#if SW_STAGE_STAGE1_OR_ABOVE
using System.Collections;

namespace SupersonicWisdomSDK
{
    internal class SwStage1FetchRemoteConfig : ISwAsyncRunnable
    {
        #region --- Members ---

        private readonly SwStage1RemoteConfigRepository _remoteConfigRepository;

        #endregion


        #region --- Construction ---

        public SwStage1FetchRemoteConfig(SwStage1RemoteConfigRepository remoteConfigRepository)
        {
            _remoteConfigRepository = remoteConfigRepository;
        }

        #endregion


        #region --- Public Methods ---

        public IEnumerator Run ()
        {
            yield return _remoteConfigRepository.Fetch();
        }

        #endregion
    }
}
#endif