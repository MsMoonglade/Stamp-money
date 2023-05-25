using System.Collections.Generic;
using JetBrains.Annotations;

namespace SupersonicWisdomSDK
{
    internal class SwConfigAccessor : ISwConfigAccessor
    {
        #region --- Members ---

        private readonly Dictionary<string, object> _mergedConfig = new Dictionary<string, object>();
        private readonly SwDeepLinkHandler _deepLinkHandler;
        private readonly SwLocalConfigHandler _localConfigHandler;

        #endregion


        #region --- Construction ---

        public SwConfigAccessor(SwDeepLinkHandler deepLinkHandler, SwLocalConfigHandler localConfigHandler)
        {
            _deepLinkHandler = deepLinkHandler;
            _localConfigHandler = localConfigHandler;
        }

        #endregion


        #region --- Public Methods ---

        public Dictionary<string, object> AsDictionary ()
        {
            return new Dictionary<string, object>(_mergedConfig);
        }

        public int GetConfigValue(string key, int defaultVal)
        {
            var val = GetValue(key);

            if (val is string)
            {
                var ret = 0;

                if (int.TryParse((string)val, out ret))
                    return ret;
            }
            else if (val is int)
            {
                return (int)val;
            }
            else if (val is long)
            {
                var ret = 0;

                if (int.TryParse(val.ToString(), out ret))
                    return ret;
            }

            return defaultVal;
        }

        public bool GetConfigValue(string key, bool bDefaultVal)
        {
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            var val = GetValue(key);

            if (val is string)
            {
                var ret = false;

                if (bool.TryParse((string)val, out ret))
                    return ret;
            }
            else if (val is bool)
            {
                return (bool)val;
            }

            return bDefaultVal;
        }

        public float GetConfigValue(string key, float defaultVal)
        {
            var val = GetValue(key);

            if (val is string)
            {
                float ret = 0;

                if (float.TryParse((string)val, out ret))
                    return ret;
            }
            else if (val is float f)
            {
                return f;
            }
            else if (val is double)
            {
                float ret = 0;

                if (float.TryParse(val.ToString(), out ret))
                    return ret;
            }

            return defaultVal;
        }

        public string GetConfigValue(string key, string defaultVal)
        {
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            var val = GetValue(key);

            if (val is string)
            {
                return (string)val;
            }

            return defaultVal;
        }

        public bool HasConfigKey(string key)
        {
            var hasKey = _mergedConfig.ContainsKey(key);
            SwInfra.Logger.Log($"SwConfigAccessor | HasConfigKey | key={key} | {hasKey}");

            return hasKey;
        }

        public void Init(SwAbConfig ab, Dictionary<string, object> accessibleConfig)
        {
            var configFromAb = ResolveAbConfig(ab);
            var configFromDeepLink = ResolveDeepLinkConfig(_deepLinkHandler.DeepLinkParams);
            var configFromLocalConfig = _localConfigHandler.LocalConfigValues;
            //The order of configs entering the _mergedConfig is important, each one overwrites duplicated values of it's predecessor.
            _mergedConfig.SwMerge(configFromLocalConfig, accessibleConfig, configFromAb, configFromDeepLink);
        }

        #endregion


        #region --- Private Methods ---

        /// <summary>
        ///     Get value per key
        ///     It favors data in the AB config over the accessible config
        /// </summary>
        /// <param name="key">key for value</param>
        /// <returns></returns>
        private object GetValue(string key)
        {
            var value = _mergedConfig.SwSafelyGet(key, null);

            if (SwInfra.Logger.IsEnabled())
            {
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                SwInfra.Logger.Log($"SwConfigAccessor | GetValue | key={key} | {value}");
            }

            return value;
        }

        private Dictionary<string, object> ResolveAbConfig([NotNull] SwAbConfig ab)
        {
            var configFromAb = new Dictionary<string, object>();

            if (ab.IsValid)
            {
                if (ab.key.Equals(SwConfigConstants.SwDictionaryKey))
                {
                    configFromAb.SwMerge(ab.value.SwToJsonDictionary());
                    SwInfra.Logger.Log($"SwConfigAccessor | Init | Got partial config via AB: {ab.value}");
                }
                else
                {
                    configFromAb[ab.key] = ab.value;
                }
            }

            return configFromAb;
        }

        private Dictionary<string, object> ResolveDeepLinkConfig([NotNull] Dictionary<string, string> deepLinkParams)
        {
            var configFromDeepLink = new Dictionary<string, object>();

            if (deepLinkParams.ContainsKey(SwConfigConstants.SwDictionaryKey))
            {
                var partialConfigDeepLinkRaw = deepLinkParams[SwConfigConstants.SwDictionaryKey];
                configFromDeepLink.SwMerge(SwJsonParser.DeserializeToDictionary(partialConfigDeepLinkRaw));
                SwInfra.Logger.Log($"SwConfigAccessor | Init | Got partial config via Deep Link: {partialConfigDeepLinkRaw}");
            }

            return configFromDeepLink;
        }

        #endregion
    }
}