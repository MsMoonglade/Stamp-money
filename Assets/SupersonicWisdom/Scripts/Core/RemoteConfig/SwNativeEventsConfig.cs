using System;

namespace SupersonicWisdomSDK
{
    [Serializable]
    internal class SwNativeEventsConfig
    {
        #region --- Constants ---

        private const bool DefaultEnabled = true;
        private const int DefaultConnectTimeout = 15;
        private const int DefaultInitialSyncInterval = 8;
        private const int DefaultReadTimeout = 10;

        #endregion


        #region --- Members ---

        public bool enabled = DefaultEnabled;
        public int connectTimeout = DefaultConnectTimeout;
        public int initialSyncInterval = DefaultInitialSyncInterval;
        public int readTimeout = DefaultReadTimeout;

        #endregion
    }
}