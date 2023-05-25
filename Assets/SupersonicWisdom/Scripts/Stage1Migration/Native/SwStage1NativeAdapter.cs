#if SW_STAGE_STAGE1_OR_ABOVE

using AppsFlyerSDK;
using JetBrains.Annotations;

namespace SupersonicWisdomSDK
{
    internal class SwStage1NativeAdapter : SwNativeAdapter
    {
        #region --- Construction ---

        public SwStage1NativeAdapter(SwNativeApi wisdomNativeApi, ISwSettings settings, SwUserData userData, [CanBeNull] ISwSessionListener[] listeners) : base(wisdomNativeApi, settings, userData, listeners)
        { }

        #endregion


        #region --- Private Methods ---

        protected override SwEventMetadataDto GetEventMetadata ()
        {
            var eventMetadata = base.GetEventMetadata();
            var stage1UserData = (SwStage1UserData)UserData;
            eventMetadata.appsFlyerId = stage1UserData.AppsFlyerId;

            return eventMetadata;
        }

        #endregion
    }
}
#endif