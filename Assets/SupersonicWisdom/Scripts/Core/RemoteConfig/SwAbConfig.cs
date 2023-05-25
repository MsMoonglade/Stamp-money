using System;
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    [Serializable]
    internal class SwAbConfig
    {
        #region --- Members ---

        public string group;
        public string id;
        public string key;
        public string value;

        #endregion


        #region --- Properties ---

        public bool IsValid
        {
            get { return !string.IsNullOrEmpty(key); }
        }

        #endregion
    }
}