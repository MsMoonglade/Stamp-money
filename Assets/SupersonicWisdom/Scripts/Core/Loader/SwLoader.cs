namespace SupersonicWisdomSDK
{
    internal class SwLoader : ISwLoader
    {
        #region --- Events ---

        public event OnBlockingLoaderVisibilityChanged OnBlockingLoaderVisibilityChangedEvent;

        #endregion


        #region --- Members ---

        private readonly SwNativeAdapter _nativeAdapter;

        #endregion


        #region --- Properties ---

        public bool IsVisible { get; private set; }

        #endregion


        #region --- Construction ---

        public SwLoader(SwNativeAdapter wisdomNativeAdapter)
        {
            _nativeAdapter = wisdomNativeAdapter;
        }

        #endregion


        #region --- Public Methods ---

        public bool Hide ()
        {
            if (!_nativeAdapter.ToggleBlockingLoader(false)) return false;
            IsVisible = false;

            OnBlockingLoaderVisibilityChangedEvent?.Invoke(IsVisible);

            return true;
        }

        public bool Show ()
        {
            if (!_nativeAdapter.ToggleBlockingLoader(true)) return false;
            IsVisible = true;

            OnBlockingLoaderVisibilityChangedEvent?.Invoke(IsVisible);

            return true;
        }

        #endregion
    }
}