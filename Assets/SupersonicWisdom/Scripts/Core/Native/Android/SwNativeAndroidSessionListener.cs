using UnityEngine;
using UnityEngine.Scripting;

namespace SupersonicWisdomSDK
{
    internal class SwNativeAndroidSessionListener : AndroidJavaProxy
    {
        #region --- Constants ---

        private const string FullyQualifiedClassName = "wisdom.library.api.listener.IWisdomSessionListener";

        #endregion


        #region --- Members ---

        public OnSessionEnded OnSessionEndedEvent;

        public OnSessionStarted OnSessionStartedEvent;

        #endregion


        #region --- Construction ---

        public SwNativeAndroidSessionListener () : base(FullyQualifiedClassName)
        { }

        #endregion


        #region --- Public Methods ---

        [Preserve]
        // ReSharper disable once InconsistentNaming
        public void onSessionEnded(string sessionId)
        {
            SwInfra.MainThreadRunner.RunOnMainThread(() => { OnSessionEndedEvent?.Invoke(sessionId); });
        }

        [Preserve]
        // ReSharper disable once InconsistentNaming
        public void onSessionStarted(string sessionId)
        {
            SwInfra.MainThreadRunner.RunOnMainThread(() => { OnSessionStartedEvent?.Invoke(sessionId); });
        }

        #endregion
    }
}