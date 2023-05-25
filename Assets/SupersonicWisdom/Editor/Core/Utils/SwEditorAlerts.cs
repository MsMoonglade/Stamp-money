using System.Collections.Generic;
using UnityEditor;

namespace SupersonicWisdomSDK.Editor
{
    public static class SwEditorAlerts
    {
        #region --- Public Methods ---

        #region --- Package import ---

        public static void AlertWelcomeMessage ()
        {
            if (EditorUtility.DisplayDialog(SwEditorConstants.UI.WELCOME_TITLE, SwEditorConstants.UI.WELCOME_MESSAGE, SwEditorConstants.UI.ButtonTitle.GoToSettings))
            {
                SwEditorUtils.OpenSettings();
            }
        }

        #endregion

        #endregion


        #region --- General ---

        public static bool Alert(string message, string okText, string cancel = "")
        {
            return EditorUtility.DisplayDialog(SwEditorConstants.UI.SUPERSONIC_WISDOM_SDK, message, okText, cancel);
        }

        public static bool AlertWarning(string message, string okText, string cancel = "")
        {
            return EditorUtility.DisplayDialog(SwEditorConstants.UI.SupersonicWisdomSDKWarning, message, okText, cancel);
        }

        public static bool AlertError(string message, long code, string buttonText, string cancel = "")
        {
            return EditorUtility.DisplayDialog(SwEditorConstants.UI.SupersonicWisdomSDKError, $"{message}\n\nError code: {(int)code}", buttonText, cancel);
        }

        #endregion


        #region --- Settings ---

        public static void AlertMissingTitles ()
        {
            SwEditorUtils.CallOnNextUpdate(() => { AlertError(SwEditorConstants.UI.REACH_TECHNICAL_SUPPORT, (int)SwErrors.ESettings.MissingTitles, SwEditorConstants.UI.ButtonTitle.Ok); });
        }

        public static void AlertDuplicatePlatform(List<GamePlatform> gamePlatforms)
        {
            SwEditorLogger.LogWarning(SwEditorConstants.UI.DUPLICATE_ITEMS.Format(gamePlatforms));

            AlertError(SwEditorConstants.UI.DUPLICATE_ITEMS.Format(gamePlatforms), (int)SwErrors.ESettings.DuplicatePlatform, SwEditorConstants.UI.ButtonTitle.Close);
        }

        public static void AlertSuccess(string selectedTitleName)
        {
            SwEditorUtils.CallOnNextUpdate(() => { Alert(SwEditorConstants.UI.NEW_CREDENTIALS_IDS_IN_SETTINGS.Format(selectedTitleName), SwEditorConstants.UI.ButtonTitle.Awesome); });
        }

        public static void AlertTitleNotFound(List<TitleDetails> current)
        {
            SwEditorUtils.CallOnNextUpdate(() =>
            {
                if (Alert(SwEditorConstants.UI.CHOOSE_TITLE_MANUALLY, SwEditorConstants.UI.ButtonTitle.GoToSettings))
                    SwEditorUtils.OpenSettings();
            });
        }

        public static void AlertLoginExpired ()
        {
            if (AlertError(SwEditorConstants.UI.LOGIN_EXPIRED, (int)SwErrors.ESettings.LoginExpired, SwEditorConstants.UI.ButtonTitle.LoginNow, SwEditorConstants.UI.ButtonTitle.Cancel))
            {
                SwAccountUtils.GoToLoginTab();
            }
        }

        public static void AlertConfigurationIsUpToDate ()
        {
            Alert(SwEditorConstants.UI.CONFIGURATION_IS_UP_TO_DATE, SwEditorConstants.UI.ButtonTitle.Ok);
        }

        public static void AlertMissingUnityPackage ()
        {
            Alert(SwEditorConstants.UI.MISSING_UNITY_IAP, SwEditorConstants.UI.ButtonTitle.Ok);
        }

        public static void TitleSavedToSettingsSuccess ()
        {
            Alert(SwEditorConstants.UI.CONFIGURATION_SUCCESSFULLY_COPIED, SwEditorConstants.UI.ButtonTitle.Ok);
        }

        #endregion


        #region --- Menu ---

        public static void AlertApplyBackupRules ()
        {
            EditorUtility.DisplayDialog(SwEditorConstants.UI.SUPERSONIC_WISDOM_SDK, SwEditorConstants.UI.APPLIED_BACKUP_RULES, SwEditorConstants.UI.ButtonTitle.Ok);
        }

        public static void AlertCannotApplyBackupRules ()
        {
            AlertError(SwEditorConstants.UI.CANNOT_APPLY_BACKUP_RULES, (int)SwErrors.EMenu.CannotApplyBackupRules, SwEditorConstants.UI.ButtonTitle.Ok);
        }

        public static void AlertDebuggableNetworkConfigurationAppliedSuccessfully ()
        {
            Alert(SwEditorConstants.UI.DEBUGGABLE_NETWORK_CONFIGURATION_APPLIED_SUCCESSFULLY, SwEditorConstants.UI.ButtonTitle.Ok);
        }

        public static void AlertCannotDebuggableNetworkConfigurationAppliedSuccessfully ()
        {
            AlertError(SwEditorConstants.UI.CANNOT_DEBUGGABLE_NETWORK_CONFIGURATION_APPLIED_SUCCESSFULLY, (int)SwErrors.EMenu.CannotApplyDebuggableNetworkConfiguration, SwEditorConstants.UI.ButtonTitle.Ok);
        }

        #endregion


        #region --- Stage update ---

        public static bool AlertUpdateInProgress()
        {
            return Alert(SwEditorConstants.UI.UPDATE_IN_PROGRESS, SwEditorConstants.UI.ButtonTitle.Ok);
        }

        public static bool AlertUpdateSuccess()
        {
            return Alert(SwEditorConstants.UI.WISDOM_UPDATED_SUCCESSFULLY,
                SwEditorConstants.UI.ButtonTitle.Awesome);

        }

        public static void AlertNoUpdateAvailable()
        {
            Alert(SwEditorConstants.UI.NO_NEED_TO_UPDATE, SwEditorConstants.UI.ButtonTitle.Thanks);
        }
        
        public static void AlertUpdateCheckFailed()
        {
            AlertError(SwEditorConstants.UI.WISDOM_UPDATE_CHECK_FAILED, (int)SwErrors.ESelfUpdate.CheckUpdatesFailed, SwEditorConstants.UI.ButtonTitle.Close);
        }
        
        public static bool AlertUpdateCheckFailedDueToEmptyGamesList()
        {
            return AlertError(SwEditorConstants.UI.CANT_CHECK_UPDATES_EMPTY_GAMES_LIST, (int) SwErrors.ESelfUpdate.CheckFailedDueToEmptyGamesList, SwEditorConstants.UI.ButtonTitle.Ok);
        }

        public static bool AlertFailedToUpdateWisdomPackage()
        {
            return AlertError(SwEditorConstants.UI.FAILED_TO_UPDATE_WISDOM_PACKAGE, (int)SwErrors.ESelfUpdate.ImportFailed, SwEditorConstants.UI.ButtonTitle.Close);
        }

        public static bool AlertFailedToDownloadPackage(int errorCode)
        {
            return AlertError(SwEditorConstants.UI.FAILED_TO_DOWNLOAD_UNITY_PACKAGE, errorCode, SwEditorConstants.UI.ButtonTitle.Close);
        }

        public static void AlertFailedToCheckCurrentStage(string errorMessage)
        {
            AlertError(SwEditorConstants.UI.FAILED_TO_CHECK_CURRENT_STAGE.Format(errorMessage), (int)SwErrors.ESelfUpdate.CheckCurrentStage, SwEditorConstants.UI.ButtonTitle.Close);
        }

        #endregion
    }
}