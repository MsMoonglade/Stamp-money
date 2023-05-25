using UnityEngine;
#if UNITY_EDITOR
using System.IO;

#endif

namespace SupersonicWisdomSDK
{
    public static class SwStageUtils
    {
        public static readonly int CurrentStageNumber;

        static SwStageUtils()
        {
            CurrentStageNumber =
#if SUPERSONIC_WISDOM_TEST
                SwTestUtils.ForceStage > -1 ? SwTestUtils.ForceStage :
#endif
#if SW_STAGE_STAGE1
            1;
#elif SW_STAGE_STAGE2
            2;
#elif SW_STAGE_STAGE3
            3;
#else
            -1;
#endif
        }

        public const int MAX_STAGE_NUMBER = 1000;

        private static SwStage _currentStage;

        public static string CurrentStageName
        {
            get { return "Stage" + CurrentStageNumber; }
        }

        public static SwStage CurrentStage
        {
            get { return _currentStage ??= LoadStage(); }
        }

        private static SwStage LoadStage()
        {
            return JsonUtility.FromJson<SwStage>(StageMetadataContent);
        }

        public static string StageMetadataContent =>
#if SUPERSONIC_WISDOM_TEST && UNITY_EDITOR
            File.ReadAllText(Path.Combine(Application.dataPath, "Editor", "Stages", CurrentStageName, "Resources", "Core", "StageMetadata.StageResource.json.template"));
#elif UNITY_EDITOR
            File.ReadAllText(Path.Combine(
            Application.dataPath,
            "SupersonicWisdom",
            "Resources",
            "Core",
            "StageMetadata.StageResource.json"));
#else
            Resources.Load<TextAsset>("Core/StageMetadata.StageResource").text;
#endif

#if SUPERSONIC_WISDOM_TEST
        internal static void Reset()
        {
            _currentStage = null;
        }
#endif
    }
}