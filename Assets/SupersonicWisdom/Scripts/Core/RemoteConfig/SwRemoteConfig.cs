using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SupersonicWisdomSDK
{
    [Serializable]
    internal class SwRemoteConfig
    {
        #region --- Constants ---

        private const string ConfigKey = "config";

        #endregion


        #region --- Members ---

        public bool unavailable;

        /// <summary>
        ///     The dictionary under "config" key.
        ///     This cannot be simply deserialized since it's a dynamic dictionary
        /// </summary>
        [NotNull] public Dictionary<string, object> AccessibleConfig = new Dictionary<string, object>();

        public SwAbConfig ab;
        public SwAgentConfig agent;
        public SwNativeEventsConfig events;

        #endregion


        #region --- Public Methods ---

        public void SetDynamicData(string json)
        {
            var responseDictionary = SwJsonParser.DeserializeToDictionary(json);
            AccessibleConfig = responseDictionary.SwSafelyGet(ConfigKey, AccessibleConfig) as Dictionary<string, object> ?? AccessibleConfig;
        }

        #endregion
    }
}