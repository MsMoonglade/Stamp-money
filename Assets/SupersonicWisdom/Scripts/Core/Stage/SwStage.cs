using System;

namespace SupersonicWisdomSDK
{
    [Serializable]
    public class SwStage
    {
        #region --- Members ---

        public int sdkStage;
        public string name;
        public string packageNameSuffix;
        public string[] defineSymbols;

        #endregion
    }
}