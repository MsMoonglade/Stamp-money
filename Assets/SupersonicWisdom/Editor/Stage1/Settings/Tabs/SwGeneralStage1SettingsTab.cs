#if SW_STAGE_STAGE1_OR_ABOVE
using System;
using Facebook.Unity.Editor;
using Facebook.Unity.Settings;
using UnityEditor;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwGeneralStage1SettingsTab : SwGeneralCoreSettingsTab
    {
        #region --- Members ---

        private readonly GUIContent _facebookAppIdLabel = new GUIContent("Facebook SDK App ID", "Your Facebook App ID");

        #endregion


        #region --- Construction ---

        public SwGeneralStage1SettingsTab(SerializedObject soSettings) : base(soSettings)
        { }

        #endregion


        protected override void OnTitleSelected()
        {
            base.OnTitleSelected();
            
            SwStage1EditorUtils.SetupGameAnalytics();
            SwSelfUpdate.CheckForUpdates();
        }


        #region --- Private Methods ---

        private static void HandleFacebookChange ()
        {
            EditorUtility.SetDirty(FacebookSettings.NullableInstance);
            ManifestMod.GenerateManifest();
        }

        protected internal override void DrawContent()
        {
            base.DrawContent();

            DrawFacebook();
            GUILayout.Space(SPACE_BETWEEN_FIELDS);
            DrawTimeBased();
        }

        private void DrawFacebook ()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(_facebookAppIdLabel, GUILayout.Width(LABEL_WIDTH));
            var beforeAppId = SwEditorUtils.FacebookAppId;
            SwEditorUtils.FacebookAppId = TextFieldWithoutSpaces(beforeAppId);
            GUILayout.EndHorizontal();

            if (!SwEditorUtils.FacebookAppId.Equals(beforeAppId))
            {
                EditorUtility.SetDirty(FacebookSettings.Instance);
                HandleFacebookChange();
            }
        }

        private void DrawTimeBased ()
        {
            GUILayout.BeginHorizontal();
            Settings.isTimeBased = GUILayout.Toggle(Settings.isTimeBased, "  Time Based Game");
            GUILayout.EndHorizontal();
        }

        #endregion
    }
}
#endif