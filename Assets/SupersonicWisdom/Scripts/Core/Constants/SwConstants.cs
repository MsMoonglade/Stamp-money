using System;
using System.IO;

namespace SupersonicWisdomSDK
{
    public static class SwConstants
    {
        #region --- Members ---
        
        public const string GameObjectName = "SupersonicWisdom";
        public const int DefaultRequestTimeout = 10;
        public const string Feature = "";
        public const long FeatureVersion = 0;
        public const string BuildNumber = "5907";
        public const string GitCommit = "28736d0";

        public const string SdkVersion = "7.2.11";
        public const string SettingsResourcePath = "SupersonicWisdom/Settings";
        public const string ExtractedResourcesDirName = "Extracted";
        public const string CrashlyticsDependenciesFilePath = "Firebase/Editor/";
        public const string CrashlyticsDependenciesFileName = "CrashlyticsDependencies.xml";
        public const string FirebaseVersionTextFileName = "FirebaseUnityWrapperVersion";
        public const string IronsourceEditorFolder = "IronSource/Editor/";
        public const string IronsourceAdapterVersionsCacheFilename = "IronSourceAdapterVersions";
        
        public static readonly long SdkVersionId = SwUtils.ComputeVersionId(SdkVersion);

        public const string APP_ICON_RESOURCE_NAME = "AppIcon";
        public static readonly string AppIconResourcesPath = Path.Combine("Extracted", APP_ICON_RESOURCE_NAME);
        
        #endregion
    }
}
