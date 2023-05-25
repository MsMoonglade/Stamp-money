namespace SupersonicWisdomSDK
{
    internal static class SwNativeApiFactory
    {
        #region --- Public Methods ---

        public static SwNativeApi GetInstance ()
        {
            if (SwUtils.IsRunningOnAndroid())
            {
                return new SwNativeAndroidApi(new SwNativeAndroidBridge());
            }

            if (SwUtils.IsRunningOnIos())
            {
                return new SwNativeIosApi(new SwNativeIosBridge());
            }

            return new SwNativeUnsupportedApi(null);
        }

        #endregion
    }
}