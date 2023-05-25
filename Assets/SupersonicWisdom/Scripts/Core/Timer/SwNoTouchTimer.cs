using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwNoTouchTimer : SwTimer
    {
        #region --- Mono Override ---

        protected override void Update ()
        {
            if (SwUtils.IsRunningOnDevice() && Input.touchCount > 0 || !SwUtils.IsRunningOnDevice() && Input.GetMouseButton(0))
            {
                if (IsEnabled || DidFinish)
                {
                    SwInfra.Logger.Log($"SwTimer | StartTimer due to touch | {Name}");
                    StartTimer();

                    return;
                }
            }

            base.Update();
        }

        #endregion


        #region --- Public Methods ---

        public static SwNoTouchTimer Create(GameObject gameObject, string name = "", float duration = 0)
        {
            var instance = gameObject.AddComponent<SwNoTouchTimer>();
            instance.Name = string.IsNullOrEmpty(name) ? instance.Name : name;
            instance.Duration = duration;

            return instance;
        }

        #endregion
    }
}