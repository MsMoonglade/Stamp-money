using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace SupersonicWisdomSDK
{
    internal class SwLocalConfigHandler
    {
        #region --- Members ---

        public Dictionary<string, object> LocalConfigValues;

        #endregion


        #region --- Public Methods ---

        public void Setup(ISwLocalConfigProvider[] configProviders)
        {
            LocalConfigValues = ResolveLocalConfigFromSetters(configProviders);
        }

        #endregion


        #region --- Private Methods ---

        private Dictionary<string, object> ResolveLocalConfigFromSetters(ISwLocalConfigProvider[] configProviders)
        {
            var arrayLength = configProviders.Length;

            if (arrayLength == 0) return new Dictionary<string, object>();

            var defaultConfigValues = new Dictionary<string, object>[arrayLength];

            for (var i = 0; i < arrayLength; i++)
            {
                var config = configProviders[i].GetLocalConfig();
                defaultConfigValues[i] = config.LocalConfigValues;
            }

            var configToMerge = new Dictionary<string, object>().SwMerge(defaultConfigValues);
            SwInfra.Logger.Log("SwLocalConfigHandler | ResolveLocalConfigFromSetters | " + $"Resolved {configToMerge.Count} pairs");

            return configToMerge;
        }

        #endregion
    }
}