namespace SupersonicWisdomSDK.Editor
{
    internal static class SwEditorConstants
    {
        #region --- Constants ---

        internal const string ASSETS = "Assets";
        public const string EMPTY_STRINGIFIED_JSON = "{}";
        public const string DEFAULT_STAGE_INTEGRATION_GUIDE_URL = "https://assets.mobilegamestats.com/docs/supersonic-default-stage-update-integration-guide.pdf";
        public const string DEFAULT_CHANGE_LOG_URL = "https://assets.mobilegamestats.com/docs/wisdom-latest-release-notes.pdf";

        #endregion


        #region --- Inner Classes ---

        internal struct GamePlatformKey
        {
            #region --- Constants ---

            internal const string Android = "google_play";
            internal const string Ios = "app_store";
            internal const string IosChina = "app_store_cn";

            #endregion
        }

        internal struct SwKeys
        {
            #region --- Constants ---

            internal const string IMPORTED_WISDOM_UPDATE_INFO = "Sw.ImportedWisdomUpdateInfo";
            internal const string LAST_LOGIN_ALERT_TIMESTAMP = "Sw.LastLoginAlertTimestamp";
            internal const string LAST_WISDOM_UPDATE_CHECKUP_TIMESTAMP = "Sw.LastWisdomUpdateCheckupTimestamp";
            internal const string TEMP_AUTH_TOKEN = "Sw.TempToken"; // Should be saved for a small period
            internal const string UPDATED_SDK_STAGE_NUMBER = "Sw.UpdatedSdkStageNumber";
            internal const string UPDATED_SDK_VERSION_ID = "Sw.UpdatedSdkVersionId";

            #endregion
        }

        internal struct UI
        {
            #region --- Constants ---

            public const string ANDROID = "Android";
            public const string APPLIED_BACKUP_RULES = "Applied Backup rules successfully";
            public const string CANNOT_APPLY_BACKUP_RULES = "Cannot apply backup rules.\nOpen the editor console to see failure reason.";
            public const string CANNOT_DEBUGGABLE_NETWORK_CONFIGURATION_APPLIED_SUCCESSFULLY = "Cannot apply debuggable network configuration.\n Open editor console to see the failure reason.";
            public const string CANT_CHECK_UPDATES = "Can't check updates";
            public const string CANT_CHECK_UPDATES_EMPTY_GAMES_LIST = "Can't check updates - Games list is empty, try to fix it with re-login";
            public const string CHOOSE_TITLE_MANUALLY = "Please choose your game from the drop down list to retrieve the credential IDs.";
            public const string CONFIGURATION_IS_UP_TO_DATE = "Configuration is up to date!";
            public const string CONFIGURATION_SUCCESSFULLY_COPIED = "Configuration successfully copied to settings";
            public const string DEBUGGABLE_NETWORK_CONFIGURATION_APPLIED_SUCCESSFULLY = "Debuggable network configuration applied successfully";
            public const string DUPLICATE_ITEMS = "Looks like game platforms list has a duplicated item, actual list is: {0}";
            public const string DUPLICATE_PRODUCT = "There is a duplication of \"{0} \" in the \" Products\" list.\nPlease remove the duplication.";
            public const string FAILED_TO_CHECK_CURRENT_STAGE = "Failed to check current stage. Error: {0}";
            public const string FAILED_TO_DOWNLOAD_UNITY_PACKAGE = "Failed to download Unity package";
            public const string FAILED_TO_GENERATE_REMOTE_URL = "Unexpected error: remote URL failed to generate.";
            public const string FAILED_TO_UPDATE_WISDOM_PACKAGE = "Failed to update Wisdom package";
            public const string INVALID_EMAIL_ADDRESS = "That's not a valid email address";

            public const string LOGIN_EXPIRED = "Login expired";
            public const string MISSING_PASSWORD = "Missing password";
            public const string MISSING_UNITY_IAP = "Missing Unity IAP Package";
            public const string NEW_CREDENTIALS_IDS_IN_SETTINGS = "New credential IDs for {0} were populated to Wisdom SDK";
            public const string NO_NEED_TO_UPDATE = "No updates available";
            public const string PARAM_IS_MISSING = "The following param: \"{0}\" is missing.\nPlease add it.";
            public const string PLEASE_CHOOSE_TITLE = "Please choose title";
            public const string REACH_TECHNICAL_SUPPORT = "Please contact technical support support@supersonic.com or try again";
            public const string WISDOM_UPDATED_SUCCESSFULLY = "SupersonicWisdom updated successfully";
            public const string WISDOM_UPDATE_FAILED_DUE_TO_COMPILATION = "Wisdom update failed due to compilation!";
            public const string WISDOM_UPDATE_CHECK_FAILED = "Wisdom update check failed!";
            public const string SUPERSONIC_WISDOM = "Supersonic Wisdom";
            public const string SUPERSONIC_WISDOM_SDK = "Supersonic Wisdom SDK";
            public const string UPDATE_IN_PROGRESS = "An update is currently still in progress, wait until the current update is done before retry.";
            public const string VERIFY_GA_SETTINGS = "The following GameAnalytics Advanced Settings should be set to true:\nSend Version\nSubmit Errors\n";
            public const string WELCOME_MESSAGE = "Please go to settings window and Login with your Supersonic platform credentials to automatically retrieve the relevant credential IDs";
            public const string WELCOME_TITLE = "Welcome to Supersonic Wisdom SDK!";

            #endregion


            #region --- Members ---

            public static readonly string SupersonicWisdomSDKError = $"{SUPERSONIC_WISDOM_SDK}: Error";
            public static readonly string SupersonicWisdomSDKWarning = $"{SUPERSONIC_WISDOM_SDK}: Warning";

            #endregion


            #region --- Public Methods ---

            public static string DownloadingUpdatePackage(string downloadedSize, string percentagesDownloaded)
            {
                return $"Please do not compile your code during this process ({downloadedSize}, {percentagesDownloaded}%)";
            }

            #endregion


            #region --- Inner Classes ---

            internal struct ButtonTitle
            {
                #region --- Constants ---

                public const string Awesome = "Awesome!";
                public const string Cancel = "Cancel";
                public const string Close = "Close";
                public const string GoToSettings = "Go to Settings";
                public const string LoginNow = "Login now";
                public const string Ok = "OK";
                public const string SetToTrueAndSave = "Set to true & Save";
                public const string Thanks = "Thanks";

                #endregion
            }

            #endregion
        }

        #endregion
    }

    public static class SwErrors
    {
        #region --- Enums ---

        public enum EMenu
        {
            CannotApplyDebuggableNetworkConfiguration = 1130,
            CannotApplyBackupRules = 1131
        }

        public enum ESettings
        {
            InvalidEmail = 1100,
            MissingPassword = 1101,
            LoginEndpoint = 1102,
            LoginExpired = 1103,
            MissingTitles = 1104,
            DuplicatePlatform = 1105,
            ChooseTitle = 1106
        }

        public enum ESelfUpdate
        {
            ImportFailed = 1140,
            CheckCurrentStage = 1142,
            DownloadUpdatePackageFailed = 1143,
            CompilationInterference = 1144,
            CheckUpdatesFailed = 1145,
            CheckFailedDueToEmptyGamesList = 1146,
            InvalidUnityPackageRemoteUrl = 1355,
            DownloadUpdatePackageCanceled = 1356,
            DownloadChecksumMismatch = 1357,
        }

        public enum ECommunication
        {
            RequestFailed = 2140,
        }

        #endregion
    }
}