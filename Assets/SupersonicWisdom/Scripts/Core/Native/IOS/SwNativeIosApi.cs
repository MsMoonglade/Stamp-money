using System;
using System.Collections;

namespace SupersonicWisdomSDK
{
    internal class SwNativeIosApi : SwNativeApi
    {
        #region --- Construction ---

        public SwNativeIosApi(SwNativeBridge nativeBridge) : base(nativeBridge)
        { }

        #endregion


        #region --- Public Methods ---

        [AOT.MonoPInvokeCallback(typeof(OnSessionEnded))]
        public static void OnSessionEnded(string sessionId)
        {
            OnSessionEndedCallbacks?.Invoke(sessionId);
        }

        [AOT.MonoPInvokeCallback(typeof(OnSessionStarted))]
        public static void OnSessionStarted(string sessionId)
        {
            OnSessionStartedCallbacks?.Invoke(sessionId);
        }

        public override void AddSessionEndedCallback(OnSessionEnded callback)
        {
            OnSessionEndedCallbacks += callback;
        }

        public override void AddSessionStartedCallback(OnSessionStarted callback)
        {
            OnSessionStartedCallbacks += callback;
        }

        public override void Destroy ()
        {
            NativeBridge.UnregisterSessionStartedCallback(OnSessionStartedCallbacks);
            NativeBridge.UnregisterSessionEndedCallback(OnSessionEndedCallbacks);
            base.Destroy();
        }

        public override IEnumerator Init(SwNativeConfig configuration)
        {
            yield return NativeBridge.InitSdk(configuration);
            NativeBridge.RegisterSessionStartedCallback(OnSessionStarted);
            NativeBridge.RegisterSessionEndedCallback(OnSessionEnded);
        }

        public override bool IsSupported ()
        {
            return true;
        }

        public override void RemoveSessionEndedCallback(OnSessionEnded callback)
        {
            OnSessionEndedCallbacks -= callback;
        }

        public override void RemoveSessionStartedCallback(OnSessionStarted callback)
        {
            OnSessionStartedCallbacks -= callback;
        }

        public override bool ToggleBlockingLoader(bool shouldPresent)
        {
            return NativeBridge.ToggleBlockingLoader(shouldPresent);
        }

        public override void RequestRateUsPopup()
        {
            NativeBridge.RequestRateUsPopup();
        }

        #endregion


        #region --- Private Methods ---

        protected override void ClearDelegates ()
        {
            var delegates = OnSessionStartedCallbacks?.GetInvocationList();

            if (delegates != null)
            {
                foreach (var item in delegates)
                {
                    OnSessionStartedCallbacks -= item as OnSessionStarted;
                }
            }

            delegates = OnSessionEndedCallbacks?.GetInvocationList();

            if (delegates == null) return;

            foreach (var item in delegates)
            {
                OnSessionEndedCallbacks -= item as OnSessionEnded;
            }
        }

        protected override void RemoveAllSessionCallbacks ()
        {
            base.RemoveAllSessionCallbacks();
            OnSessionStartedCallbacks = null;
            OnSessionEndedCallbacks = null;
        }

        #endregion
    }
}