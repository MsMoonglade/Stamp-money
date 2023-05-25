#if SW_STAGE_STAGE1_OR_ABOVE

using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEditor;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
#if SW_STAGE_STAGE1
    [CustomEditor(typeof(SwSettings))]
#endif
    internal class SwStage1SettingsInspector : SwCoreSettingsInspector
    {
        #region --- Members ---

        private bool _didChangeGameAnalyticsKeys;

        #endregion


        #region --- Private Methods ---

        protected override void AfterDrawSelectedTab ()
        {
            base.AfterDrawSelectedTab();

            if (_didChangeGameAnalyticsKeys)
            {
                EditorUtility.SetDirty(GameAnalytics.SettingsGA);
                _didChangeGameAnalyticsKeys = false;
            }
        }

        protected override void BeforeDrawSelectedTab ()
        {
            base.BeforeDrawSelectedTab();
            SwStage1EditorUtils.OnGameAnalyticsKeysChangedEvent -= OnGameAnalyticsKeysChanged;
            SwStage1EditorUtils.OnGameAnalyticsKeysChangedEvent += OnGameAnalyticsKeysChanged;
            _didChangeGameAnalyticsKeys = false;
        }

        protected override SwCoreSettingsTab[] StageTabs ()
        {
            return new SwCoreSettingsTab[]
            {
                new SwGeneralStage1SettingsTab(_soSettings),
                new SwIosStage1SettingsTab(_soSettings),
                new SwAndroidStage1SettingsTab(_soSettings),
                new SwDebugStage1SettingsTab(_soSettings)
            };
        }

        #endregion


        #region --- Event Handler ---

        private void OnGameAnalyticsKeysChanged ()
        {
            _didChangeGameAnalyticsKeys = true;
        }

        #endregion
    }
}
#endif