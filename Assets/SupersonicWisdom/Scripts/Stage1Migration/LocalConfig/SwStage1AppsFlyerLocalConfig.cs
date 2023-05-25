#if SW_STAGE_STAGE1_OR_ABOVE
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal class SwStage1AppsFlyerLocalConfig : SwLocalConfig
    {
        #region --- Constants ---

        private const string AppsFlyerDefaultDomainDefaultValue = "appsflyersdk.com";
        private const string AppsFlyerDefaultDomainKey = "appsFlyerDomain";

        #endregion


        #region --- Properties ---

        public override Dictionary<string, object> LocalConfigValues
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { AppsFlyerDefaultDomainKey, AppsFlyerDefaultDomainDefaultValue }
                };
            }
        }

        #endregion
    }
}
#endif