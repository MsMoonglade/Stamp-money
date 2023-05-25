#if SW_STAGE_STAGE1_OR_ABOVE
using UnityEditor.Android;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwFacebookAdvertiserIdCollectionAndroidInjector : IPostGenerateGradleAndroidProject
    {
        #region --- Properties ---

        public int callbackOrder
        {
            get { return 1; }
        }

        #endregion


        #region --- Public Methods ---

        public void OnPostGenerateGradleAndroidProject(string basePath)
        {
            var swAndroidManifest = new SwAndroidManifest(basePath);

            if (SwStageUtils.CurrentStageNumber == 1)
            {
                swAndroidManifest.SetMetadataElement("com.facebook.sdk.AdvertiserIDCollectionEnabled", "true");
            }

            swAndroidManifest.Save();
        }

        #endregion
    }
}

#endif