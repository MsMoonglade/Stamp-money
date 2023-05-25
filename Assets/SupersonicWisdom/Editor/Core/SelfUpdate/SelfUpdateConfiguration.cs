using System.Collections.Generic;
using SwJsonDictionary = System.Collections.Generic.Dictionary<string, object>;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwSelfUpdateConfiguration
    {
        #region --- Members ---

        public readonly bool ShouldUpdateVersion;
        public readonly bool ShouldVerifyUpdate;
        public readonly bool ShouldUpdateStage;
        public readonly SwJsonDictionary StageUpdate;
        public readonly SwJsonDictionary StageAndVersionUpdate;
        public readonly SwJsonDictionary VersionUpdate;
        public readonly int UpdatedStage;

        public readonly long UpdatedVersionId;

        #endregion


        #region --- Properties ---

        public bool ShouldUpdate
        {
            get { return ShouldUpdateStage || ShouldUpdateVersion; }
        }

        public static int CurrentStage
        {
            get { return SwStageUtils.CurrentStage.sdkStage; }
        }

        public static long CurrentVersionId
        {
            get { return SwConstants.SdkVersionId; }
        }

        #endregion


        #region --- Construction ---

        public SwSelfUpdateConfiguration(SwSelfUpdateRawConfiguration rawConfig, int updatedStage, long updatedVersionId)
        {
            var sourceVersionsBlackList = rawConfig?.sourceVersionsBlackList ?? new List<string>();
            var targetVersionsBlackList = rawConfig?.targetVersionsBlackList ?? new List<string>();
            var sourceVersionsPostInstallVerificationBlackList = rawConfig?.sourceVersionsPostInstallVerificationBlackList ?? new List<string>();
            var targetVersionsPostInstallVerificationBlackList = rawConfig?.targetVersionsPostInstallVerificationBlackList ?? new List<string>();
            StageUpdate = rawConfig?.stageUpdate ?? new SwJsonDictionary();
            StageAndVersionUpdate = rawConfig?.stageAndVersionUpdate ?? new SwJsonDictionary();
            VersionUpdate = rawConfig?.versionUpdate ?? new SwJsonDictionary();
            UpdatedStage = updatedStage;
            UpdatedVersionId = updatedVersionId;

            var currentStage = SwStageUtils.CurrentStage.sdkStage;
            ShouldUpdateStage = currentStage < UpdatedStage;
            var currentVersionId = SwConstants.SdkVersionId;
            ShouldUpdateVersion = UpdatedVersionId > currentVersionId;

            if (ShouldUpdateVersion)
            {
                var versionString = SwUtils.ComputeVersionString(UpdatedVersionId);
                
                if (sourceVersionsBlackList.Contains(SwConstants.SdkVersion))
                {
                    ShouldUpdateVersion = false; 
                    SwEditorLogger.Log($"Will skip version update from {SwConstants.SdkVersion}, as configured remotely.");
                }

                if (targetVersionsBlackList.Contains(versionString))
                {
                    ShouldUpdateVersion = false; 
                    SwEditorLogger.Log($"Will skip version update to {versionString}, as configured remotely.");
                }
                
                ShouldVerifyUpdate = !sourceVersionsPostInstallVerificationBlackList.Contains(SwConstants.SdkVersion);
                if (!ShouldVerifyUpdate)
                {
                    SwEditorLogger.Log($"Will skip update validation from {SwConstants.SdkVersion}, as configured remotely.");
                }
                else
                {
                    ShouldVerifyUpdate &= !targetVersionsPostInstallVerificationBlackList.Contains(versionString);
                    if (!ShouldVerifyUpdate)
                    {
                        SwEditorLogger.Log($"Will skip update validation to {versionString}, as configured remotely.");
                    }
                }
            }
        }

        #endregion
    }
}