using SupersonicWisdomSDK;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwDevToolsPopup : MonoBehaviour
    {
        #region --- Inspector ---

        [SerializeField] private GameObject container;

        #endregion


        #region --- Members ---

        private SwFilesCacheManager _filesCacheManager;

        #endregion


        #region --- Properties ---

        public bool IsVisible
        {
            get { return container != null && container.gameObject.activeSelf; }
        }

        #endregion


        #region --- Mono Override ---

        private void Awake ()
        {
            DontDestroyOnLoad(gameObject);

            // Verify container is inactive even though it should be by default
            container.gameObject.SetActive(false);
        }

        #endregion


        #region --- Public Methods ---

        public static SwDevToolsPopup Create(SwFilesCacheManager filesCacheManager)
        {
            var swDevToolsPrefab = Resources.Load("Core/DevTools/SwDevToolsPopup", typeof(SwDevToolsPopup)) as SwDevToolsPopup;

            if (swDevToolsPrefab == null)
            {
                return null;
            }

            var devToolsPopup = Instantiate(swDevToolsPrefab);
            devToolsPopup._filesCacheManager = filesCacheManager;

            return devToolsPopup;
        }

        public void ClearSDKCache ()
        {
            var files = _filesCacheManager?.GetAllFilesFromCache() ?? new string[] { };

            foreach (var file in files)
            {
                _filesCacheManager?.DeleteFile(file);
            }
        }

        public void DeletePlayerPrefs ()
        {
            SwInfra.KeyValueStore.DeleteAll();
        }

        public void Hide ()
        {
            container.gameObject.SetActive(false);
        }

        public void QuitApplication ()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        public void Show ()
        {
            if (container.gameObject.activeSelf)
            {
                SwInfra.Logger.LogError("SupersonicWisdom Devtools is already visible.");
            }

            container.gameObject.SetActive(true);
        }

        #endregion
    }
}