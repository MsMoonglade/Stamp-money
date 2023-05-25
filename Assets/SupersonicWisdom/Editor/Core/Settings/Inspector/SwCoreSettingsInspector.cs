using Facebook.Unity.Editor;
using Facebook.Unity.Settings;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwCoreSettingsInspector : UnityEditor.Editor, ISwRepaintDelegate
    {
        #region --- Members ---

        internal static int? SelectedTabIndex;
        protected SerializedObject _soSettings;
        protected SwSettings Settings;
        private int _currentTabIndex;
        
        private string _currentStage;
        private SwCoreSettingsTab[] _tabs = new SwCoreSettingsTab[]{ };

        #endregion


        #region --- Properties ---

        protected int SpaceBetweenFields
        {
            get { return SwCoreSettingsTab.SPACE_BETWEEN_FIELDS; }
        }

        // public bool DidGameAnalyticsChange { get; set; }
        private SwCoreSettingsTab[] SettingsTabs
        {
            get { return Tabs(); }
        }

        #endregion


        #region --- Mono Override ---

        private void OnEnable ()
        {
            // Fix for unpredictable bug in unity editor
            // OnEnable can sometime be called automatically in play mode
            // https://answers.unity.com/answers/1854373/view.html
            if (target == null)
            {
                return;
            }

            Settings = target as SwSettings;
            _soSettings = new SerializedObject(Settings);

            if (FacebookSettings.NullableInstance == null)
            {
                FacebookSettingsEditor.Edit();
                Selection.activeObject = SwEditorUtils.SwSettings;
            }
        }

        private void OnValidate ()
        {
            CheckIfStageChanged();
        }

        #endregion


        #region --- Public Methods ---

        public override void OnInspectorGUI()
        {
            CheckIfStageChanged();
            BeforeDrawSelectedTab();

            _soSettings.Update();
            GUILayout.BeginVertical();
            GUILayout.Space(SpaceBetweenFields * 3);
            DrawSupersonicTitle();
            GUILayout.Space(SpaceBetweenFields * 3);
            var didGuiChange = GUI.changed;
            _currentTabIndex = SelectTab(SelectedTabIndex ?? _currentTabIndex);
            SelectedTabIndex = null;
            GUI.changed = didGuiChange;
            DrawSelectedTab(_currentTabIndex, this);
            GUILayout.EndVertical();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(Settings);
            }

            AfterDrawSelectedTab();
        }

        public void SwRepaint ()
        {
            Repaint();
        }

        #endregion


        #region --- Private Methods ---

        protected virtual void AfterDrawSelectedTab ()
        { }

        protected virtual void BeforeDrawSelectedTab ()
        { }

        protected virtual SwCoreSettingsTab[] StageTabs()
        {
            return new SwCoreSettingsTab[]
            {
                new SwGeneralCoreSettingsTab(_soSettings),
                new SwIosCoreSettingsTab(_soSettings),
                new SwAndroidCoreSettingsTab(_soSettings),
                new SwDebugCoreSettingsTab(_soSettings)
            };
        }

        private void CheckIfStageChanged ()
        {
            if (!(_currentStage?.Equals(SwStageUtils.CurrentStageName) ?? false))
            {
                _currentStage = SwStageUtils.CurrentStageName;
                OnStageChanged();
            }
        }
        
        private void DrawSelectedTab(int selectedTabIndex, ISwRepaintDelegate repaintDelegate)
        {
            EditorGUI.BeginChangeCheck();
            var selectedTab = SettingsTabs.SwSafelyGet(selectedTabIndex, null);

            if (selectedTab != null)
            {
                selectedTab.RepaintDelegate = repaintDelegate;
                selectedTab.DrawContent();
            }

            if (EditorGUI.EndChangeCheck())
            {
                _soSettings.ApplyModifiedProperties();
            }
        }

        private void DrawSupersonicTitle()
        {
            GUILayout.BeginHorizontal();
            SwEditorUtils.DrawGuiLayoutSwLogoLabel(32);
            GUILayout.BeginVertical();
            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            var title = $" SupersonicWisdom SDK v.{SwConstants.SdkVersion} [{SwStageUtils.CurrentStageName}]";
#if UNITY_2019_1_OR_NEWER
            title = $"{title}{(EditorUtility.IsDirty(Settings) ? " *" : "")}";
#endif
            GUILayout.Label(title, EditorStyles.largeLabel);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void OnStageChanged()
        {
            ResetTabs();
        }

        private void ResetTabs()
        {
            _tabs = new SwCoreSettingsTab[] { };
        }

        private int SelectTab(int tabToSelect)
        {
            EditorGUI.BeginChangeCheck();
            var tabNames = SettingsTabs.Select(e => e.Name()).ToArray();
            var selectedTab = GUILayout.Toolbar(tabToSelect, tabNames);

            if (EditorGUI.EndChangeCheck())
            {
                _soSettings.ApplyModifiedProperties();
                GUI.FocusControl(null);
            }

            return selectedTab;
        }

        private SwCoreSettingsTab[] Tabs ()
        {
            if (_tabs.Length > 0) return _tabs;

            _tabs = StageTabs();

            return _tabs;
        }

        #endregion
    }
}