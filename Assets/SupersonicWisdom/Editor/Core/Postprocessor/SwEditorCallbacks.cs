using UnityEditor;

namespace SupersonicWisdomSDK.Editor
{
    public static class SwEditorCallbacks
    {
        #region --- Construction ---
        
        static SwEditorCallbacks() // Static constructor, called only once, when the class gets loaded.
        {
            AssemblyReloadEvents.beforeAssemblyReload += OnPreCompilation;
        }

        #endregion
        
        
        #region --- Private Methods ---

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnPostCompilation()
        {
            SwAccountUtils.TryToRestoreLoginToken();
            WelcomeMessageUtils.TryShowWelcomeMessage();
            SwSelfUpdate.OnPostCompilation();
        }

        #endregion


        #region --- Event Handler ---

        private static void OnPreCompilation()
        {
            SwSelfUpdate.OnPreCompilation();
        }

        #endregion
    }
}