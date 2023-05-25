#if SW_STAGE_STAGE1_OR_ABOVE

namespace SupersonicWisdomSDK
{
    internal class SwStage1RemoteConfigRepository : SwCoreRemoteConfigRepository
    {
        #region --- Construction ---

        public SwStage1RemoteConfigRepository(ISwSettings settings, SwUserData userData, ISwWebRequestClient webRequestClient, SwCoreTracker tracker, SwStoreKeys storeKeys, ISwRemoteConfigListener listener) : base(settings, userData, webRequestClient, tracker, storeKeys, listener)
        { }

        #endregion


        #region --- Private Methods ---

        protected override SwRemoteConfigRequestPayload CreatePayload ()
        {
            var payload = base.CreatePayload();
            payload.testId = SwInfra.KeyValueStore.GetString(SwStage1DeepLinkConstants.TestIdStorageKey);

            return payload;
        }

        #endregion
    }
}
#endif