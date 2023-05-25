using System;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    [Serializable]
    public class SwProduct : ISerializationCallbackReceiver
    {
        #region --- Constants ---

        private const string IapConsumableKey = "CONSUMABLE";
        private const string IapNonConsumableKey = "NON_CONSUMABLE";
        private const string IapSubscriptionKey = "SUBSCRIPTION";

        #endregion


        #region --- Members ---

        public bool isNoAds;
        public string id;
        public string inAppPurchaseType;

        public string productId;
        public string referenceName;
        public SwProductType productType;

        #endregion


        #region --- Public Methods ---

        public void OnAfterDeserialize ()
        {
            var type = SwProductType.NonConsumable;

            if (!string.IsNullOrEmpty(inAppPurchaseType))
            {
                switch (inAppPurchaseType)
                {
                    case IapNonConsumableKey:
                        type = SwProductType.NonConsumable;
                        break;
                    case IapConsumableKey:
                        type = SwProductType.Consumable;
                        break;
                    case IapSubscriptionKey:
                        //Removed The Subscription type from usage - https://supersonicstudio.monday.com/boards/883112163/pulses/3412912009
                        break;
                }
            }

            productType = type;
        }

        public void OnBeforeSerialize ()
        { }

        #endregion
    }
}