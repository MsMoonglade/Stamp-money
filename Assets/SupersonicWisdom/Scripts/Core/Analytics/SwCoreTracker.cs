using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SupersonicWisdomSDK
{
    internal class SwCoreTracker : ISwGameProgressionListener
    {
        #region --- Constants ---

        private const string InfraEventType = "Infra";
        private const string ProgressEventType = "Progress";

        #endregion


        #region --- Members ---

        private static readonly Dictionary<NetworkReachability, string> ConnectionDictionary = new Dictionary<NetworkReachability, string>
        {
            [NetworkReachability.NotReachable] = "offline",
            [NetworkReachability.ReachableViaLocalAreaNetwork] = "wifi",
            [NetworkReachability.ReachableViaCarrierDataNetwork] = "carrier"
        };

        private readonly ISwWebRequestClient _webRequestClient;
        private readonly SwNativeAdapter _wisdomNativeAdapter;
        private readonly SwTimerManager _timerManager;
        private readonly SwUserData _userData;

        #endregion


        #region --- Properties ---

        private float PlaytimeElapsed
        {
            get { return _timerManager?.PlaytimeStopWatch?.Elapsed ?? -1f; }
        }

        #endregion


        #region --- Construction ---

        public SwCoreTracker(SwNativeAdapter wisdomNativeAdapter, SwUserData userData, ISwWebRequestClient webRequestClient, SwTimerManager timerManager)
        {
            _wisdomNativeAdapter = wisdomNativeAdapter;
            _userData = userData;
            _webRequestClient = webRequestClient;
            _timerManager = timerManager;
        }

        #endregion


        #region --- Public Methods ---

        public void OnTimeBasedGameStarted () { }

        public void OnLevelCompleted(long level, string levelName, long attempts, long revives)
        {
            TrackGameProgressEvent(SwProgressEvent.LevelCompleted, level, attempts, PlaytimeElapsed, revives);
        }

        public void OnLevelFailed(long level, string levelName, long attempts, long revives)
        {
            TrackGameProgressEvent(SwProgressEvent.LevelFailed, level, attempts, PlaytimeElapsed, revives);
        }

        public void OnLevelRevived(long level, string levelName, long attempts, long revives)
        {
            TrackGameProgressEvent(SwProgressEvent.LevelRevived, level, attempts, PlaytimeElapsed, revives);
        }

        public void OnLevelSkipped(long level, string levelName, long attempts, long revives)
        {
            TrackGameProgressEvent(SwProgressEvent.LevelSkipped, level, attempts, PlaytimeElapsed, revives);
        }

        public void OnLevelStarted(long level, string levelName, long attempts, long revives)
        {
            TrackGameProgressEvent(SwProgressEvent.LevelStarted, level, attempts, 0, revives);
        }

        public IEnumerator SendEvent(string url, object data)
        {
            SwInfra.Logger.Log("SendEvent | endpoint | " + url);

            if (SwTestUtils.IsRunningTests) yield break;
            var response = new SwWebResponse();

            yield return _webRequestClient.Post(url, data, response, SwConstants.DefaultRequestTimeout);
            SwInfra.Logger.Log("SendEvent | sent");

            if (response.DidFail)
            {
                SwInfra.Logger.LogError("SendEvent | Fail | " + $"code: {response.code} | error: {response.error} | " + $"Internet Reachability: {Application.internetReachability}");
            }
            else
            {
                SwInfra.Logger.Log("SendEvent | success");
            }
        }

        public void SendUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            SwInfra.CoroutineService.StartCoroutine(SendUrlCoroutine(url));
        }

        public void TrackAdsNotifierEvent(string eventName, string custom1 = "", string custom2 = "", string custom3 = "", string custom4 = "", string custom5 = "", string custom6 = "")
        {
            TrackEventInternal(eventName, $"{custom1}", $"{custom2}", $"{custom3}", $"{custom4}", $"{custom5}", $"{custom6}");
        }

        public void TrackEvent(string evt, string custom1 = "", string custom2 = "", string custom3 = "")
        {
            SwInfra.Logger.Log("trackEvent " + evt);
            TrackEventInternal(evt, custom1, custom2, custom3);
        }

        public void TrackGameProgressEvent(SwProgressEvent progress, long levelNum, long attempts, float playtime = 0f, long revives = 0)
        {
            TrackEventInternal(ProgressEventType, $"{progress}", $"{levelNum}", $"{attempts}", $"{(int)Mathf.Round(playtime)}", $"{revives}");
        }

        public void TrackInfraEvent(string custom1, string custom2 = "", string custom3 = "", string custom4 = "")
        {
            TrackEventInternal(InfraEventType, $"{custom1}", $"{custom2}", $"{custom3}", $"{custom4}");
        }

        #endregion


        #region --- Private Methods ---

        private static EventCustoms CreateEventCustoms(string custom1, string custom2, string custom3, string custom4, string custom5, string custom6, string custom7, string custom8, string custom9, string custom10, string custom11, string custom12)
        {
            return new EventCustoms
            {
                custom1 = custom1,
                custom2 = custom2,
                custom3 = custom3,
                custom4 = custom4,
                custom5 = custom5,
                custom6 = custom6,
                custom7 = custom7,
                custom8 = custom8,
                custom9 = custom9,
                custom10 = custom10,
                custom11 = custom11,
                custom12 = custom12
            };
        }

        private static IEnumerator SendUrlCoroutine(string url)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                SwInfra.Logger.Log("SendEvent | error | network not reachable");

                yield break;
            }

            using (var webRequest = UnityWebRequest.Get(url))
            {
                yield return webRequest.SendWebRequest();
                SwInfra.Logger.Log($"SendUrl | {url}");
                var code = webRequest.responseCode;

                if (code == 0 || code >= 400)
                {
                    SwInfra.Logger.LogError("SendUrl | Fail |" + code);
                }
                else
                {
                    SwInfra.Logger.Log("SendUrl | success");
                }
            }
        }

        protected internal void TrackEventInternal(string eventName, string custom1 = "", string custom2 = "", string custom3 = "", string custom4 = "", string custom5 = "", string custom6 = "", string custom7 = "", string custom8 = "", string custom9 = "", string custom10 = "", string custom11 = "", string custom12 = "")
        {
            var eventCustoms = CreateEventCustoms(custom1, custom2, custom3, custom4, custom5, custom6, custom7, custom8, custom9, custom10, custom11, custom12);
            var customsJson = JsonUtility.ToJson(eventCustoms);
            var extraJson = JsonUtility.ToJson(GetEventDetailsExtra());

            _wisdomNativeAdapter.TrackEvent(eventName, customsJson, extraJson);
        }

        protected SwEventDetailsExtra GetEventDetailsExtra ()
        {
            var eventDetailsExtra = new SwEventDetailsExtra
            {
                lang = _userData.Language,
                country = _userData.Country
            };

            // The following properties are relying Unity API.
            // Unity API can be accessed only via main thread
            if (SwUtils.IsRunningOnMainThread)
            {
                eventDetailsExtra.connection = ConnectionDictionary[Application.internetReachability];
                eventDetailsExtra.dpi = $"{Screen.dpi}";
                eventDetailsExtra.resolutionWidth = $"{Screen.currentResolution.width}";
                eventDetailsExtra.resolutionHeight = $"{Screen.currentResolution.height}";
            }

            return eventDetailsExtra;
        }

        #endregion


        #region --- Inner Classes ---

        [Serializable]
        public class EventCustoms
        {
            #region --- Members ---

            public string custom1;
            public string custom10;
            public string custom11;
            public string custom12;
            public string custom2;
            public string custom3;
            public string custom4;
            public string custom5;
            public string custom6;
            public string custom7;
            public string custom8;
            public string custom9;

            #endregion
        }

        #endregion
    }
}