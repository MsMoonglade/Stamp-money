#if SW_STAGE_STAGE1_OR_ABOVE

using AppsFlyerSDK;

namespace SupersonicWisdomSDK
{
    internal class SwStage1UserData : SwUserData
    {
        #region --- Properties ---

        public string AppsFlyerId { get; set; }

        #endregion


        #region --- Construction ---

        public SwStage1UserData(ISwSettings settings, SwStoreKeys storeKeys, ISwAdvertisingIdsGetter idsGetter) : base(settings, storeKeys, idsGetter)
        { }

        #endregion
    }
}

#endif