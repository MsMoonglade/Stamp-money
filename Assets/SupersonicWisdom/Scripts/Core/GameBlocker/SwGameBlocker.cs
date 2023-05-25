using System.IO;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwGameBlocker
    {
        #region --- Constants ---

        public const string AvailabilityMessageKey = "availabilityMessage";
        private const string AvailabilityEvent = "AvailabilityPopupImpression";

        private const string MissingCanvasPrefabError = "Missing canvas prefab!";
        private const string MissingSwBlockerViewComponentError = "Missing SBlockerPopupView component!";

        #endregion


        #region --- Members ---

        public static readonly string BlockerPath = Path.Combine("Core", "GameBlocker", "BlockerCanvas");

        private readonly SwCoreTracker _tracker;

        #endregion


        #region --- Construction ---

        public SwGameBlocker(SwCoreTracker tracker)
        {
            _tracker = tracker;
        }

        #endregion


        #region --- Public Methods ---

        public void ShowPopup(string blockMessage)
        {
            var popupGameObject = InstantiatePopup();

            if (popupGameObject == null)
            {
                SwInfra.Logger.LogError(MissingCanvasPrefabError);

                return;
            }

            var blockerPopupView = popupGameObject.GetComponent<BlockerPopupView>();

            if (blockerPopupView == null)
            {
                SwInfra.Logger.LogError(MissingSwBlockerViewComponentError);

                return;
            }

            blockerPopupView.Setup(blockMessage, SwUtils.AppIconSprite);
            SwUtils.LockUI();

            _tracker.TrackEvent(AvailabilityEvent);
        }

        #endregion


        #region --- Private Methods ---

        private static GameObject InstantiatePopup ()
        {
            var popupResource = SwUtils.Load<GameObject>(BlockerPath);

            return Object.Instantiate(popupResource);
        }

        #endregion
    }
}