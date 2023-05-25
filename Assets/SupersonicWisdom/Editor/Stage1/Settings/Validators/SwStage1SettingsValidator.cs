#if SW_STAGE_STAGE1_OR_ABOVE

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal abstract class SwStage1SettingsValidator : SwSettingsValidator
    {
        #region --- Properties ---

        private bool IsAndroid
        {
            get { return this is SwStage1AndroidSettingsValidator; }
        }

        #endregion


        #region --- Private Methods ---

        internal override MissingParam CheckMissingParams ()
        {
            var (gaGameKey, gaSecretKey) = SwStage1EditorUtils.GetGameAnalyticsKeys(IsAndroid ? RuntimePlatform.Android : RuntimePlatform.IPhonePlayer);

            var paramsToCheck = new List<Tuple<MissingParam, string>>().Append(new Tuple<MissingParam, string>(MissingParam.GaGameKey, gaGameKey)).Append(new Tuple<MissingParam, string>(MissingParam.GaSecretKey, gaSecretKey));

            return SwSettingsValidator.CheckMissingParams(paramsToCheck);
        }

        #endregion
    }
}

#endif