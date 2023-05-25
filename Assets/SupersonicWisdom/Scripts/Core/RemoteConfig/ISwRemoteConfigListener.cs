using System.Collections.Generic;
using JetBrains.Annotations;

namespace SupersonicWisdomSDK
{
    /// <summary>
    ///     Remote config listener
    /// </summary>
    internal interface ISwRemoteConfigListener
    {
        #region --- Public Methods ---

        /// <summary>
        ///     Called upon remote config resolve (success/fail)
        /// </summary>
        /// <param name="error"></param>
        /// <param name="remoteConfig"></param>
        /// <param name="abConfig"></param>
        /// <param name="accessibleConfig"></param>
        void OnResolve(SwRemoteConfigError error, [CanBeNull] SwRemoteConfig remoteConfig, [NotNull] SwAbConfig abConfig, [NotNull] Dictionary<string, object> accessibleConfig);

        #endregion
    }
}