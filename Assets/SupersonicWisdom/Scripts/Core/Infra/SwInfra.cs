using System.Threading;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal static class SwInfra
    {
        #region --- Members ---

        public static Thread MainThread;

        #endregion


        #region --- Properties ---

        public static ISwKeyValueStore KeyValueStore { get; private set; }

        public static ISwLogger Logger { get; private set; }

        public static ISwMainThreadRunner MainThreadRunner { get; private set; }

        public static SwCoroutineService CoroutineService { get; private set; }

        #endregion


        #region --- Public Methods ---

        public static void Initialize(ISwLogger logger, SwCoroutineService coroutineService, ISwKeyValueStore keyValueStore, ISwMainThreadRunner mainThreadRunner)
        {
            Logger = logger;
            CoroutineService = coroutineService;
            KeyValueStore = keyValueStore;
            MainThreadRunner = mainThreadRunner;
            MainThread = Thread.CurrentThread;
        }

        public static void InitializeCoreDefaults(SwCoreMonoBehaviour mono)
        {
            var coroutineService = new SwCoroutineService(mono);
            var keyValueStore = new SwPlayerPrefsStore();
            var settingsManager = new SwSettingsManager<ISwSettings>(keyValueStore);
            var loggerService = new SwLoggerService(settingsManager.Settings);

            Initialize(loggerService, coroutineService, keyValueStore, mono);
        }

        #endregion
    }
}