using System.Collections;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal abstract class SwNativeApi : ISwAdvertisingIdsGetter
    {
        #region --- Members ---

        protected static OnSessionEnded OnSessionEndedCallbacks;
        protected static OnSessionStarted OnSessionStartedCallbacks;

        protected readonly SwNativeBridge NativeBridge;

        #endregion


        #region --- Construction ---

        public SwNativeApi(SwNativeBridge nativeBridge)
        {
            NativeBridge = nativeBridge;
        }

        #endregion


        #region --- Public Methods ---

        public virtual void Destroy ()
        {
            RemoveAllSessionCallbacks();
            NativeBridge.Destroy();
        }

        public virtual string GetAdvertisingId ()
        {
            return NativeBridge.GetAdvertisingId();
        }

        public virtual string GetOrganizationAdvertisingId ()
        {
            return NativeBridge.GetOrganizationAdvertisingId();
        }

        public virtual void InitializeSession(SwEventMetadataDto metadata)
        {
            NativeBridge.InitializeSession(metadata);
        }

        public virtual void UpdateMetadata(SwEventMetadataDto metadata)
        {
            NativeBridge.UpdateEventMetadata(metadata);
        }

        public virtual void UpdateWisdomConfiguration(SwNativeConfig configuration)
        {
            NativeBridge.UpdateWisdomConfiguration(configuration);
        }

        public abstract void AddSessionEndedCallback(OnSessionEnded callback);
        public abstract void AddSessionStartedCallback(OnSessionStarted callback);
        public abstract IEnumerator Init(SwNativeConfig configuration);
        public abstract bool IsSupported ();
        public abstract void RemoveSessionEndedCallback(OnSessionEnded callback);
        public abstract void RemoveSessionStartedCallback(OnSessionStarted callback);

        public abstract bool ToggleBlockingLoader(bool shouldPresent);

        public abstract void RequestRateUsPopup();

        #endregion


        #region --- Private Methods ---

        protected virtual void RemoveAllSessionCallbacks ()
        {
            ClearDelegates();
        }

        protected abstract void ClearDelegates ();

        internal virtual void TrackEvent(string eventName, string customsJson, string extraJson)
        {
            NativeBridge.TrackEvent(eventName, customsJson, extraJson);
        }

        #endregion
    }
}