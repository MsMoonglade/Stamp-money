using System.Collections;

namespace SupersonicWisdomSDK
{
    /// <summary>
    ///     Remote config repository
    ///     Responsible for fetching and persisting remote config and ab test data
    /// </summary>
    internal interface ISwRemoteConfigRepository
    {
        #region --- Properties ---

        /// <summary>
        ///     Get availability
        /// </summary>
        /// <returns></returns>
        bool Unavailable { get; }

        #endregion


        #region --- Public Methods ---

        /// <summary>
        ///     Add listener for finishing resolving remote config
        ///     The name "onLoaded" is used here because it's eventually
        ///     exposed via `SupersonicWisdom.Api.AddOnLoadedListener`
        /// </summary>
        /// <param name="onLoadedCallback"></param>
        void AddOnLoadedListener(OnLoaded onLoadedCallback);

        /// <summary>
        ///     Did finish process of fetching remote config (success/fail)
        /// </summary>
        /// <returns></returns>
        bool DidResolve ();

        /// <summary>
        ///     Fetch the remote config
        ///     After fetch OnLoaded listeners should be called
        ///     Fetch cannot be called before Init
        /// </summary>
        /// <returns></returns>
        IEnumerator Fetch ();

        /// <summary>
        ///     Get resolved AB config
        /// </summary>
        /// <returns></returns>
        SwAbConfig GetAb ();

        /// <summary>
        ///     Init the repository
        ///     This method must be called before Fetch to restore any persisted remote config
        /// </summary>
        void Init ();

        void RemoveOnLoadedListener(OnLoaded onLoadedCallback);

        #endregion
    }
}