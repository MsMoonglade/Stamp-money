using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwDevTools
    {
        #region --- Constants ---

        private const int DevtoolsTapCount = 3;

        #endregion


        #region --- Members ---

        private readonly SwFilesCacheManager _filesCacheManager;
        private bool _isDevtoolEnabled;

        private SwDevToolsPopup _devToolsPopup;

        #endregion


        #region --- Construction ---

        public SwDevTools(SwFilesCacheManager filesCacheManager)
        {
            _filesCacheManager = filesCacheManager;
        }

        #endregion


        #region --- Public Methods ---

        public void EnableDevtools(bool isEnabled)
        {
            if (SwUtils.IsRunningOnDevice())
            {
                _isDevtoolEnabled = isEnabled;

                if (_isDevtoolEnabled)
                {
                    _devToolsPopup = SwDevToolsPopup.Create(_filesCacheManager);
                }
                else if (_devToolsPopup != null)
                {
                    Object.Destroy(_devToolsPopup.gameObject);
                    _devToolsPopup = null;
                }
            }
        }

        public void OnUpdate ()
        {
            if (!_isDevtoolEnabled || !SwUtils.IsRunningOnDevice())
            {
                return;
            }

            if (Input.touches.Length == 1)
            {
                var touch = Input.GetTouch(0);

                if (touch.fingerId == 0 && touch.phase == TouchPhase.Ended && touch.tapCount == DevtoolsTapCount && !_devToolsPopup.IsVisible)
                {
                    OpenDevtools();
                }
            }
        }

        #endregion


        #region --- Private Methods ---

        private void CloseDevtools ()
        {
            if (SwUtils.IsRunningOnDevice())
            {
                if (_isDevtoolEnabled && _devToolsPopup != null)
                {
                    _devToolsPopup.Hide();
                }
            }
        }

        private void OpenDevtools ()
        {
            if (SwUtils.IsRunningOnDevice())
            {
                if (_isDevtoolEnabled && _devToolsPopup != null)
                {
                    _devToolsPopup.Show();
                }
            }
        }

        #endregion
    }
}