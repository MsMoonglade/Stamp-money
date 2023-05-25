using System;

namespace SupersonicWisdomSDK
{
    [Serializable]
    internal class SwRemoteConfigRequestPayload
    {
        public string bundle;
        public string gameId;
        public string sysLang;
        public string os;
        public string osver;
        public string version;
        public string device;
        public string session;
        public string uuid;
        public string abid;
        public string isNew;
        public string apiVersion;
        public string sdkVersion;
        public long sdkVersionId;
        public long stage;
        public string testId;
#if UNITY_IOS
        public string idfv;
#endif
#if UNITY_ANDROID
        public string appSetId;
#endif
    }
}