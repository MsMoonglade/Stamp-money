using System.Collections;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwNativeUnsupportedApi : SwNativeApi
    {
        #region --- Construction ---

        public SwNativeUnsupportedApi(SwNativeBridge nativeBridge) : base(nativeBridge)
        { }

        #endregion


        #region --- Public Methods ---

        public override void AddSessionEndedCallback(OnSessionEnded callback)
        {
            SwInfra.Logger.Log("SwNativeUnsupportedApi | AddSessionEndedCallback");
        }

        public override void AddSessionStartedCallback(OnSessionStarted callback)
        {
            SwInfra.Logger.Log("SwNativeUnsupportedApi | AddSessionStartedCallback");
        }

        public override void Destroy ()
        {
            SwInfra.Logger.Log("SwNativeUnsupportedApi | Destroy");
        }

        public override string GetAdvertisingId ()
        {
            return SystemInfo.deviceUniqueIdentifier;
        }

        public override string GetOrganizationAdvertisingId ()
        {
            return SystemInfo.deviceUniqueIdentifier;
        }

        public override IEnumerator Init(SwNativeConfig configuration)
        {
            SwInfra.Logger.Log($"SwNativeUnsupportedApi | Init | {configuration}");

            yield break;
        }

        public override void InitializeSession(SwEventMetadataDto metadata)
        {
            SwInfra.Logger.Log($"SwNativeUnsupportedApi | InitializeSession | Uuid: {metadata.uuid}, CustomInstallationId: {metadata.swInstallationId}");
        }

        public override bool IsSupported ()
        {
            return false;
        }

        public override void RemoveSessionEndedCallback(OnSessionEnded callback)
        {
            SwInfra.Logger.Log("SwNativeUnsupportedApi | RemoveSessionEndedCallback");
        }

        public override void RemoveSessionStartedCallback(OnSessionStarted callback)
        {
            SwInfra.Logger.Log("SwNativeUnsupportedApi | RemoveSessionStartedCallback");
        }

        public override bool ToggleBlockingLoader(bool shouldPresent)
        {
            SwInfra.Logger.Log("SwNativeUnsupportedApi | ToggleBlockingLoader(" + shouldPresent + ") - returning `false`");

            return false;
        }

        public override void UpdateMetadata(SwEventMetadataDto metadata)
        {
            SwInfra.Logger.Log($"SwNativeUnsupportedApi | UpdateMetadata | Uuid: {metadata.uuid}, CustomInstallationId: {metadata.swInstallationId}");
        }

        public override void UpdateWisdomConfiguration(SwNativeConfig configuration)
        {
            SwInfra.Logger.Log($"SwNativeUnsupportedApi | UpdateWisdomConfiguration | configuration: {configuration}");
        }

        public override void RequestRateUsPopup()
        {
            SwInfra.Logger.Log($"SwNativeUnsupportedApi | RequestRateUsPopup | Not supported on platforms other than Android and iOS");
        }

        #endregion


        #region --- Private Methods ---

        protected override void ClearDelegates ()
        {
            SwInfra.Logger.Log("SwNativeUnsupportedApi | clearDelegates");
        }

        protected override void RemoveAllSessionCallbacks ()
        {
            SwInfra.Logger.Log("SwNativeUnsupportedApi | RemoveAllSessionCallbacks");
        }

        internal override void TrackEvent(string eventName, string customsJson, string extraJson)
        {
            SwInfra.Logger.Log($"SwNativeUnsupportedApi | TrackEvent | {eventName} | {customsJson} | {extraJson}");
        }

        #endregion
    }
}