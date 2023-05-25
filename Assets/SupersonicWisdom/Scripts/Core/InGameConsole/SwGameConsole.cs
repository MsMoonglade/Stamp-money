using System;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwGameConsole
    {
        #region --- Members ---

        private static readonly Lazy<SwGameConsoleMono> LazyInGameConsole = new Lazy<SwGameConsoleMono>(CreateConsole);
        private static SwGameConsoleLogType _consoleLogType;

        #endregion


        #region --- Properties ---

        private static SwGameConsoleMono InGameConsole
        {
            get { return LazyInGameConsole.Value; }
        }

        #endregion


        #region --- Public Methods ---

        public static void InitConsole(string logLevel)
        {
            switch (logLevel)
            {
                case "Warning":
                    _consoleLogType = SwGameConsoleLogType.Warning;

                    break;
                case "Error":
                    _consoleLogType = SwGameConsoleLogType.Error;

                    break;
                case "Exception":
                    _consoleLogType = SwGameConsoleLogType.Exception;

                    break;
                default:
                    SwInfra.Logger.LogWarning("Unknown LogType level, InGameConsole will not be created.");

                    return;
            }

            Application.logMessageReceivedThreaded -= LogToConsole;
            Application.logMessageReceivedThreaded += LogToConsole;
        }

        #endregion


        #region --- Private Methods ---

        private static SwGameConsoleMono CreateConsole ()
        {
            var instance = new GameObject("InGameConsole");
            var consoleMono = instance.AddComponent<SwGameConsoleMono>();
            consoleMono.HideConsole();
            SwInfra.Logger.Log("Deeplink | SwGameConsoleMono has been created");

            return consoleMono;
        }

        private static bool ShouldShowLog(LogType type)
        {
            //Determines if we should show the log or filter it based on the log level chosen via the deepLink tool
            //We filter Assets and Log level logs by default
            if (type == LogType.Assert || type == LogType.Log) return false;

            var logTypeName = type.ToString();
            SwGameConsoleLogType result;

            if (Enum.TryParse(logTypeName, true, out result))
            {
                if (result < _consoleLogType) return false;
            }

            return true;
        }

        #endregion


        #region --- Event Handler ---

        private static void LogToConsole(string log, string stacktrace, LogType type)
        {
            SwInfra.MainThreadRunner.RunOnMainThread(() =>
            {
                if (ShouldShowLog(type))
                {
                    InGameConsole.InternalLogToConsole(log, stacktrace, type.ToString());
                }
            });
        }

        #endregion
    }
}