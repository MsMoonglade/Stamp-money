using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace SupersonicWisdomSDK.Editor
{
    internal abstract class SwSettingsValidator
    {
        #region --- Public Methods ---

        public IapDuplicateParam CheckIapForDuplicates ()
        {
            return IsDuplicateIap(GetIapNoAdsId()) ? IapDuplicateParam.NoAdsIapDuplicate : IapDuplicateParam.No;
        }

        public void HandleDuplicateIap(IapDuplicateParam param)
        {
            SwEditorAlerts.AlertWarning(SwEditorConstants.UI.DUPLICATE_PRODUCT.Format(GetIapDuplicationDetails(param)), SwEditorConstants.UI.ButtonTitle.Ok);
        }

        public void HandleMissingParam(MissingParam param)
        {
            SwEditorAlerts.AlertWarning(SwEditorConstants.UI.PARAM_IS_MISSING.Format(GetDetails(param)), SwEditorConstants.UI.ButtonTitle.Ok);
        }

        #endregion


        #region --- Private Methods ---

        /// <summary>
        ///     Validates the values for specific params.
        /// </summary>
        /// <returns>If nothing missed will be returned NO_ERR otherwise id of error</returns>
        protected static MissingParam CheckMissingParams(IEnumerable<Tuple<MissingParam, string>> paramsToCheck)
        {
            var firstMissing = paramsToCheck.FirstOr(e => IsMissingParam(e.Item2), null);
            var missingParam = firstMissing?.Item1 ?? MissingParam.No;

            return missingParam;
        }

        protected static bool IsMissingParam(string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        private static bool IsDuplicateIap(string noAdsProductId)
        {
            return SwEditorUtils.SwSettings.IapProductDescriptors.Any(product => product.productID.Equals(noAdsProductId));
        }

        protected virtual string GetIapNoAdsId ()
        {
            return "";
        }

        internal abstract MissingParam CheckMissingParams ();

        private string GetDetails(MissingParam param)
        {
            switch (param)
            {
                case MissingParam.IosAppId:
                    return "iOS App ID";

                case MissingParam.GameId:
                    return "Game ID";

                case MissingParam.IsAppKey:
                    return "IronSource App Key";

                case MissingParam.GaGameKey:
                    return "GameAnalytics Game Key";

                case MissingParam.GaSecretKey:
                    return "GameAnalytics Secret Key";

                case MissingParam.FbAppId:
                    return "Facebook App ID";

                case MissingParam.AdmobAppId:
                    return "AdMob App ID";
                default:
                    return "Some required data";
            }
        }

        private string GetIapDuplicationDetails(IapDuplicateParam param)
        {
            switch (param)
            {
                case IapDuplicateParam.NoAdsIapDuplicate:
                    return "\"No Ads\" Product ID";
                default:
                    return "some iap product";
            }
        }

        #endregion


        #region --- Enums ---

        public enum IapDuplicateParam
        {
            No = 0,
            NoAdsIapDuplicate = -1
        }

        protected internal enum MissingParam
        {
            No = 0,
            IosAppId = -1,
            GameId = -2,
            IsAppKey = -3,
            GaGameKey = -4,
            GaSecretKey = -5,
            FbAppId = -6,
            AdmobAppId = -7
        }

        #endregion
    }
}