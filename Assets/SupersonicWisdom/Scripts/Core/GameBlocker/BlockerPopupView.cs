using UnityEngine;
using UnityEngine.UI;

namespace SupersonicWisdomSDK
{
    [RequireComponent(typeof(Canvas))]
    public class BlockerPopupView : MonoBehaviour
    {
        #region --- Constants ---

        private const string DefaultBlockMessage = "This game is unavailable in your country";

        private const string ErrorResourceNotFound = "SupersonicWisdom: Rsource is missing!\n " + "Please make sure that BlockerCanvas prefab locate on: Assets/SupersonicWisdom/Resources/Core/GameBlocker/BlockerCanvas.prefab";

        #endregion


        #region --- Inspector ---

        [SerializeField] private Image appIcon;

        [SerializeField] private Text message;

        #endregion


        #region --- Members ---

        private Canvas _canvas;

        #endregion


        #region --- Mono Override ---

        private void Awake ()
        {
            _canvas = GetComponent<Canvas>();
            message.text = DefaultBlockMessage;
        }

        private void OnValidate ()
        {
            if (SwUtils.Load<GameObject>(SwGameBlocker.BlockerPath) == null)
            {
                Debug.LogWarning(ErrorResourceNotFound);
            }
        }

        #endregion


        #region --- Public Methods ---

        public void Setup(string message = null, Sprite appIcon = null)
        {
            if (!string.IsNullOrEmpty(message))
            {
                this.message.text = message;
            }

            if (appIcon != null)
            {
                this.appIcon.sprite = appIcon;
            }

            gameObject.DontDestroyOnLoad();
            _canvas.RenderLast();
        }

        #endregion
    }
}