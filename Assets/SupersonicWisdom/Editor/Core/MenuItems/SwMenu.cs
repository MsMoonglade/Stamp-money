using System;
using System.Collections.Generic;
using Facebook.Unity.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal static class SwMenu
    {
        #region --- Public Methods ---

        [MenuItem(SwEditorConstants.UI.SUPERSONIC_WISDOM + "/Allow credentials override", false, 403)]
        public static void AllowEditingSettings ()
        {
            SwEditorUtils.AllowEditingSettings = true;
            InternalEditorUtility.RepaintAllViews();
        }

        [MenuItem(SwEditorConstants.UI.SUPERSONIC_WISDOM + "/" + SwEditorConstants.UI.ANDROID + "/Apply Backup Rules", false, 201)]
        public static void ApplyAndroidBackupRules ()
        {
            var isSuccess = SwAndroidBackupRules.ApplyAndroidBackupRules();

            if (isSuccess)
            {
                SwEditorAlerts.AlertApplyBackupRules();
            }
            else
            {
                SwEditorAlerts.AlertCannotApplyBackupRules();
            }
        }

        [MenuItem(SwEditorConstants.UI.SUPERSONIC_WISDOM + "/" + SwEditorConstants.UI.ANDROID + "/Apply Debuggable Network Config", false, 202)]
        public static void ApplyAndroidNetworkSecurityConfig ()
        {
            var isSuccess = SwAndroidNetworkSecurityConfig.ApplyAndroidNetworkSecurityConfig();

            if (isSuccess)
            {
                SwEditorAlerts.AlertDebuggableNetworkConfigurationAppliedSuccessfully();
            }
            else
            {
                SwEditorAlerts.AlertCannotDebuggableNetworkConfigurationAppliedSuccessfully();
            }
        }

        [MenuItem(SwEditorConstants.UI.SUPERSONIC_WISDOM + "/Check for updates...", false, 402)]
        public static void CheckForWisdomUpdates()
        {
            // Ignoring warning: "Async method invocation without await expression"
#pragma warning disable 4014
            SwSelfUpdate.CheckForUpdates(true);
#pragma warning restore 4014
        }

        [MenuItem(SwEditorConstants.UI.SUPERSONIC_WISDOM + "/Edit Settings", false, 101)]
        public static void EditSettings ()
        {
            SwEditorUtils.OpenSettings();
        }

#if SW_STAGE_STAGE2_OR_ABOVE
        [MenuItem(SwEditorConstants.UI.SUPERSONIC_WISDOM + "/Open stage integration guide", false, 403)]
        public static void OpenIntegrationGuide()
        {
            var integrationGuideUrl = SwEditorUtils.SwAccountData.IntegrationGuideUrl;
            Application.OpenURL(!string.IsNullOrEmpty(integrationGuideUrl) ? integrationGuideUrl : SwEditorConstants.DEFAULT_STAGE_INTEGRATION_GUIDE_URL);
        }
#endif

        [MenuItem(SwEditorConstants.UI.SUPERSONIC_WISDOM + "/" + SwEditorConstants.UI.ANDROID + "/Re-generate Manifest", false, 200)]
        public static void UpdateAndroidManifest()
        {
            FacebookSettingsEditor.Edit();
            ManifestMod.GenerateManifest();
        }

        #endregion
    }
}