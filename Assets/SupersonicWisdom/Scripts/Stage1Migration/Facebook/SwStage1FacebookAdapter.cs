#if SW_STAGE_STAGE1_OR_ABOVE
using System;
using Facebook.Unity;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    public delegate void OnFacebookInit ();

    public delegate void OnFacebookInitComplete ();

    public delegate void OnFacebookUnityHide(bool isUnityShown);

    internal static class SwStage1FacebookConstants
    {
        #region --- Constants ---

        public const string FacebookInitInternalEventName = "FacebookInit";

        #endregion
    }

    internal class SwStage1FacebookAdapter : ISwAdapter
    {
        #region --- Events ---

        public event OnFacebookInit OnFacebookInitEvent;
        public event OnFacebookInitComplete OnFacebookInitCompleteEvent;
        public event OnFacebookUnityHide OnFacebookUnityHideEvent;

        #endregion


        #region --- Members ---

        private bool _canInit = true;
        private bool _didApplyAfterComplete;
        private bool _didInit;
        private string _sdkStatus;

        #endregion


        #region --- Mono Override ---

        internal void OnApplicationPause(bool isPaused)
        {
            if (!isPaused) // Application resume
            {
                if (_didInit)
                {
                    InitInternal(true);
                }
            }
        }

        #endregion


        #region --- Private Methods ---

        protected virtual void BeforeInitialAppActivation ()
        {
            if (SwStageUtils.CurrentStageNumber == 1)
            {
                // As part of the prototype stage, logging of app events is allowed, in later stages (stage 2) we determine the state based on user agreement
                FB.Mobile.SetAutoLogAppEventsEnabled(true);
            }
        }

        internal void Init ()
        {
            if (_didInit)
            {
                return;
            }

            _didInit = true;
            InitInternal();
        }

        private void AfterInitComplete ()
        {
            _canInit = true;

            if (_didApplyAfterComplete)
            {
                FB.ActivateApp();

                return;
            }

            _didApplyAfterComplete = true;
            BeforeInitialAppActivation();
            // Signal an app activation App Event
            FB.ActivateApp();

            if (SwStageUtils.CurrentStageNumber == 1)
            {
                // Continue with Facebook SDK as in later stages we call it as part of the privacy control
                FB.Mobile.SetAdvertiserTrackingEnabled(true);
            }

            OnFacebookInitCompleteEvent?.Invoke();
            _sdkStatus = true.ToString();
        }

        private void FacebookUnityHide(bool isUnityShown)
        {
            OnFacebookUnityHideEvent?.Invoke(isUnityShown);
        }

        private void InitInternal(bool isAppResume = false)
        {
            if (!_canInit)
            {
                return;
            }
            
            _canInit = false;

            if (!FB.IsInitialized)
            {
                try
                {
                    FB.Init(AfterInitComplete, FacebookUnityHide);
                    OnFacebookInitEvent?.Invoke();
                    SwInternalEvent.Invoke(SwStage1FacebookConstants.FacebookInitInternalEventName);
                    SwInfra.Logger.Log("Facebook | init");
                }
                catch (Exception e)
                {
                    SwInfra.Logger.LogError(e.Message);
                    _canInit = true;
                    _sdkStatus = e.Message;
                }
            }
            else
            {
                // this condition is for cases where FB is initialized more than one time in test environment
                // When running profile tests in a sequence, FB will be initialized effectively only once.
                // But, we still want to notify test client that FB was "initialized", 
                if (SwTestUtils.IsRunningTests && !isAppResume)
                {
                    SwInternalEvent.Invoke(SwStage1FacebookConstants.FacebookInitInternalEventName);
                }

                AfterInitComplete();
            }
        }
        
        public SwAdapterData GetAdapterStatusAndVersion()
        {
            var adapterData = new SwAdapterData
            {
                adapterName = nameof(Facebook),
                adapterStatus = _sdkStatus,
                adapterVersion = FacebookSdkVersion.Build
            };

            return adapterData;
        }

        #endregion
    }
}
#endif