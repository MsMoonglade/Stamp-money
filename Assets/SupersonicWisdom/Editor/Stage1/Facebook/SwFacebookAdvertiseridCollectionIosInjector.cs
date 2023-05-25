#if SW_STAGE_STAGE1_OR_ABOVE
#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace SupersonicWisdomSDK.Editor
{
    internal static class SwFacebookAdvertiseridCollectionIosInjector
    {
        [PostProcessBuild(201)]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
        {
            if (buildTarget == BuildTarget.iOS)
            {
                string plistPath = Path.Combine(path, "Info.plist");
                PlistDocument plist = new PlistDocument();
                plist.ReadFromFile(plistPath);
                if (SwStageUtils.CurrentStageNumber == 1)
                {
                    plist.root.SetBoolean("FacebookAdvertiserIDCollectionEnabled", true);
                }
                File.WriteAllText(plistPath, plist.WriteToString());
            }
        }
    }    
}

#endif
#endif