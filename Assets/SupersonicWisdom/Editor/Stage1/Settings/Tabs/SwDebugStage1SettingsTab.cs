#if SW_STAGE_STAGE1_OR_ABOVE
using UnityEditor;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwDebugStage1SettingsTab : SwDebugCoreSettingsTab
    {
        #region --- Construction ---

        public SwDebugStage1SettingsTab(SerializedObject soSettings) : base(soSettings)
        { }

        #endregion
    }
}
#endif