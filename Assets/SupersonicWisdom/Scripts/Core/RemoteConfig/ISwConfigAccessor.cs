using System.Collections.Generic;
using JetBrains.Annotations;

namespace SupersonicWisdomSDK
{
    /// <summary>
    ///     Config values accessor by keys
    ///     It gets the accessible dictionary and the ab config and determine which value to return per key
    /// </summary>
    internal interface ISwConfigAccessor
    {
        #region --- Public Methods ---

        int GetConfigValue(string key, int defaultVal);
        float GetConfigValue(string key, float defaultVal);
        bool GetConfigValue(string key, bool defaultVal);
        string GetConfigValue(string key, string defaultVal);

        /// <summary>
        ///     Does key exist in config
        /// </summary>
        /// <param name="key">Key to lookup</param>
        /// <returns></returns>
        bool HasConfigKey(string key);

        /// <summary>
        ///     Init the config accessor with ab config and accessible config
        /// </summary>
        /// <param name="ab"></param>
        /// <param name="accessibleConfig"></param>
        void Init([NotNull] SwAbConfig ab, [NotNull] Dictionary<string, object> accessibleConfig);

        #endregion
    }
}