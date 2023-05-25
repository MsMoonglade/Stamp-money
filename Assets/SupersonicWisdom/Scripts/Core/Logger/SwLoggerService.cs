using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using UnityDebug = UnityEngine.Debug;

namespace SupersonicWisdomSDK
{
    internal class SwLoggerService : ISwLogger
    {
        #region --- Constants ---

        private const string DefaultPrefix = "SupersonicWisdom: ";

        #endregion


        #region --- Members ---

        private readonly ISwSettings _settings;
        private string _prefix = DefaultPrefix;

        #endregion


        #region --- Properties ---

        public bool LogViaNetwork { get; set; } = false;

        internal string Prefix
        {
            get { return _prefix; }
            set { _prefix = string.IsNullOrEmpty(value) ? DefaultPrefix : value; }
        }

        #endregion


        #region --- Construction ---

        public SwLoggerService(ISwSettings settings)
        {
            _settings = settings;
        }

        #endregion


        #region --- Public Methods ---

        public bool IsEnabled ()
        {
            return _settings.IsDebugEnabled();
        }

        public void Log(string msg)
        {
            if (_settings.IsDebugEnabled())
            {
                var message = RefineMessage(msg);
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                UnityDebug.Log($"{Prefix}{message}");

                if (LogViaNetwork)
                {
                    SwInfra.CoroutineService.StartCoroutine(LogThroughNetwork("info", message));
                }
            }
        }

        public void Log(Func<string> msgFn)
        {
            if (_settings.IsDebugEnabled())
            {
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                Log(msgFn());
            }
        }

        public void LogError(Exception exception)
        {
            LogError(exception.ToString());
        }
        
        public void LogError(string msg)
        {
            if (_settings.IsDebugEnabled())
            {
                var message = RefineMessage(msg);
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                UnityDebug.LogError($"{_prefix}{message}");

                if (LogViaNetwork)
                {
                    SwInfra.CoroutineService.StartCoroutine(LogThroughNetwork("error", message));
                }
            }
        }

        public void LogError(Func<string> msgFn)
        {
            if (_settings.IsDebugEnabled())
            {
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                LogError(msgFn());
            }
        }

        public void LogWarning(string msg)
        {
            if (_settings.IsDebugEnabled())
            {
                var message = RefineMessage(msg);
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                UnityDebug.LogWarning($"{_prefix}{message}");

                if (LogViaNetwork)
                {
                    SwInfra.CoroutineService.StartCoroutine(LogThroughNetwork("warning", message));
                }
            }
        }

        public void LogWarning(Func<string> msgFn)
        {
            if (_settings.IsDebugEnabled())
            {
                LogWarning(msgFn());
            }
        }

        #endregion


        #region --- Private Methods ---

        /// <summary>
        ///     Log the message to a dummy url in oder to see wisdom logs via Proxy app.
        /// </summary>
        /// <param name="level">Log Level</param>
        /// <param name="message">Log Message</param>
        /// <returns></returns>
        private IEnumerator LogThroughNetwork(string level, string message)
        {
            var webRequest = new UnityWebRequest($"http://supersonic-wisdom.log/{level}?message={Uri.EscapeUriString(message)}");

            yield return webRequest.SendWebRequest();
        }

        private string RefineMessage(string msg)
        {
            if (msg.IndexOf(";base64", StringComparison.Ordinal) != -1)
            {
                return Regex.Replace(msg, ";base64,(.*?)\"", ";base64,DATA\"");
            }

            return msg;
        }

        #endregion
    }
}