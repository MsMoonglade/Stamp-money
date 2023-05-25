#if SW_STAGE_STAGE1_OR_ABOVE
using System;
using System.Collections.Generic;
using System.Linq;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwStage1AndroidSettingsValidator : SwStage1SettingsValidator
    {
        #region --- Private Methods ---

        internal override MissingParam CheckMissingParams ()
        {
            var missingParams = base.CheckMissingParams();

            if (missingParams != MissingParam.No) return missingParams;

            var paramsToCheck = new List<Tuple<MissingParam, string>>().Append(new Tuple<MissingParam, string>(MissingParam.GameId, SwEditorUtils.SwSettings.androidGameId)).Append(new Tuple<MissingParam, string>(MissingParam.FbAppId, SwEditorUtils.FacebookAppId));

            return SwSettingsValidator.CheckMissingParams(paramsToCheck);
        }

        #endregion
    }
}

#endif