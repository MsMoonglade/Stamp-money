using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
using Facebook.Unity.Settings;
using JetBrains.Annotations;
using UnityEditor;

namespace SupersonicWisdomSDK
{
    /// <summary>
    ///     Marker attribute for overridable SwSettings fields by remote config `settings` dictionary
    /// </summary>
    public class SwSettings : ScriptableObject, ISwSettings
    {
        [SerializeField] public string appName;
        [SerializeField] public string iosAppId;
        [SerializeField] public string iosChinaAppId;

        [SerializeField] public string iosGameId;
        [SerializeField] public string iosChinaGameId;
        [SerializeField] public string androidGameId;

        [SerializeField] public string ironSourceAndroidAppKey;
        [SerializeField] public string ironSourceIosAppKey;
        [SerializeField] public string ironSourceIosChinaAppKey;

        [SerializeField] public string iosGameAnalyticsGameKey;
        [SerializeField] public string iosGameAnalyticsSecretKey;
        [SerializeField] public string androidGameAnalyticsGameKey;
        [SerializeField] public string androidGameAnalyticsSecretKey;

        [SerializeField] public string adMobIosAppId;
        [SerializeField] public string adMobIosChinaAppId;
        [SerializeField] public string adMobAndroidAppId;

        [SerializeField] [HideInInspector] public bool areTextMeshProEssentialsInstalled;

        [SerializeField] public bool disableFacebookInit;

        [SerializeField] public SwPrivacyPolicy testPrivacyPolicy = SwPrivacyPolicy.None;

        [SerializeField] public bool customInterstitialMinimumInterval;

        [SerializeField]
        public int interstitialMinimumInterval = DefaultInterstitialMinimumInterval;

        [SerializeField] public bool customInterstitialMinimumLevel;
        [SerializeField] public int interstitialMinimumLevel = DefaultInterstitialMinimumLevel;

        [SerializeField] public bool iosChinaBuildEnabled;
        [SerializeField] public string iapNoAdsAndroid;
        [SerializeField] public string iapNoAdsIOS;
        [SerializeField] public string iosChinaGameAnalyticsGameKey;
        [SerializeField] public string iosChinaGameAnalyticsSecretKey;

        [SerializeField] public bool isTimeBased;
        [SerializeField] public bool enableManagedInterstitialAds;
        [SerializeField] public bool enableManagedBannerAds;
        [SerializeField] public bool enableManagedRateUsPopup = true;

        // RV ads are loaded automatically and shown on demand via API invocation.
        // This fixed property is for writing consistent ads strategy code and avoid treating RV ads as a special case
        // in the sense of loading the RV ads.
        // Test Ads player requires loading RV ads before it can be stated as ready
        // IronSource Ads adsplayer loads RV ads automatically and so is their readiness.
        [NonSerialized] public bool enableManagedRewardedVideoAds = true;

        [SerializeField] [SwOverwritableSettingsField]
        public bool enableDebug;

        [SerializeField] [SwOverwritableSettingsField]
        public bool debugIronSource;

        [SerializeField] [SwOverwritableSettingsField]
        public bool testBlockingApiInvocation;

        [SerializeField] [SwOverwritableSettingsField]
        public bool enableTestAds;

        [SerializeField] [SwOverwritableSettingsField]
        public bool enableDevtools;

        [SerializeField] [SwOverwritableSettingsField]
        public bool logViaNetwork;

        [SerializeField] private List<SwProductDescriptor> iapProductsAndroid;
        [SerializeField] private List<SwProductDescriptor> iapProductsIos;

        [SerializeField] public bool enableAndroidIap;
        [SerializeField] public bool enableIosIap;

        public bool IsIosIapEnabled
        {
            get { return enableIosIap && !iosChinaBuildEnabled; }
        }

        public bool IsAndroidIapEnabled
        {
            get { return enableAndroidIap; }
        }

        public bool IsIapEnabled
        {
            get
            {
                if (SwUtils.IsAndroidTarget())
                {
                    return IsAndroidIapEnabled;
                }

                if (SwUtils.IsIosTarget())
                {
                    return IsIosIapEnabled;
                }

                return false;
            }
        }

        public bool IsIosIapForbidden
        {
            get { return enableIosIap && iosChinaBuildEnabled; }
        }

        public List<SwProductDescriptor> IapProductDescriptors
        {
            get
            {
#if UNITY_IOS && !UNITY_EDITOR
                return iapProductsIos;
#elif UNITY_ANDROID && !UNITY_EDITOR
                return iapProductsAndroid;
#else
                return new List<SwProductDescriptor>(0);
#endif
            }
        }

#pragma warning disable 0649
        [SerializeField] public bool validateBeforeBuild = true;

        /* Local State */
        internal SwSettings SettingsFromResource;
#pragma warning restore 0649

        public const int DefaultInterstitialMinimumInterval = 20;
        public const int DefaultInterstitialMinimumLevel = 1;

        private const string SettingsStoragePrefix = "SupersonicWisdomSettings_";

        // Account
        public string accountEmail;
        public int selectedGameIndex;
        public bool isTitleSelectedAutomatically;
        public bool wasLoggedIn;

        public string IosGameId
        {
            set { iosGameId = value ?? ""; }
            get { return iosChinaBuildEnabled ? iosChinaGameId : iosGameId; }
        }

        public string AndroidGameId
        {
            set { androidGameId = value ?? ""; }
            get { return androidGameId; }
        }

        public string GameId
        {
            get
            {
                if (SwUtils.IsAndroidTarget())
                {
                    return AndroidGameId;
                }

                if (SwUtils.IsIosTarget())
                {
                    return IosGameId;
                }

                return "";
            }
        }

        public string IronSourceAppKey
        {
            get
            {
                if (SwUtils.IsAndroidTarget())
                {
                    return ironSourceAndroidAppKey;
                }

                if (SwUtils.IsIosTarget())
                {
                    return IronSourceIosAppKey;
                }

                return "";
            }
        }

        public string IosAppId
        {
            get { return iosChinaBuildEnabled ? iosChinaAppId : iosAppId; }
        }

        public string IronSourceIosAppKey
        {
            get { return iosChinaBuildEnabled ? ironSourceIosChinaAppKey : ironSourceIosAppKey; }
        }

        public string AdMobIosAppId
        {
            get { return iosChinaBuildEnabled ? adMobIosChinaAppId : adMobIosAppId; }
        }

        public string NoAdsProductId =>
#if UNITY_ANDROID
            iapNoAdsAndroid;
#elif UNITY_IOS
            iapNoAdsIOS;
#else
            "";
#endif

        public string[] EnabledDevelopmentFlags
        {
            get
            {
                var flags = new List<string>();

                if (enableDebug)
                {
                    flags.Add("Debug SDK");
                }

                if (debugIronSource)
                {
                    flags.Add("Debug IronSource");
                }

                if (testBlockingApiInvocation)
                {
                    flags.Add("Test Blocking Notifications");
                }

                if (testPrivacyPolicy != SwPrivacyPolicy.None)
                {
                    flags.Add($"Test Privacy Policy: {testPrivacyPolicy}");
                }

                if (enableDevtools)
                {
                    flags.Add("Enable Devtools");
                }

                if (enableTestAds)
                {
                    flags.Add("Enable Test Ads");
                }

                return flags.ToArray();
            }
        }

        /// <summary>
        ///     deep link can include values for overwriting overwritable SwSettings fields.
        ///     Values are saved in PlayerPrefs
        ///     In this method, for each SwSettings' overridable field:
        ///     - if value for field exists in PlayerPrefs override SwSettings.field
        ///     - else restore value from the original settings obtained by the resource Resources/SupersonicWisdom/Settings.asset
        /// </summary>
        /// <seealso cref="OverwritePartially" />
        /// <seealso cref="SwOverwritableSettingsField" />
        private void ReadSettings([NotNull] ISwKeyValueStore keyValueStore)
        {
            var writableFields = typeof(SwSettings).GetFields().Where(field => field.IsDefined(typeof(SwOverwritableSettingsField), false)).ToArray();

            foreach (var field in writableFields)
            {
                var fieldStorageKey = GetSettingsStorageKey(field.Name);

                if (PlayerPrefs.HasKey(fieldStorageKey))
                {
                    if (field.FieldType == typeof(bool))
                    {
                        field.SetValue(this, keyValueStore.GetBoolean(fieldStorageKey));
                    }
                }
                else
                {
                    field.SetValue(this, field.GetValue(SettingsFromResource));
                }
            }
        }

        /// <summary>
        ///     In this method, for each SwSettings' overwritable field:
        ///     - if value for field exists in the remoteConfigSettingsDict/DeepLinkDict overwrite its value within SwSettings and
        ///     write it to PlayerPrefs
        ///     - else delete it from PlayerPrefs and restore its original value from from the original settings obtained by the
        ///     resource Resources/SupersonicWisdom/Settings.asset
        /// </summary>
        /// <param name="dict">Settings dictionary</param>
        /// <param name="keyValueStore">Key-Value store</param>
        /// <seealso cref="ReadSettings" />
        /// <seealso cref="SwOverwritableSettingsField" />
        public void OverwritePartially(IReadOnlyDictionary<string, object> dict, [NotNull] ISwKeyValueStore keyValueStore)
        {
            var writableFields = typeof(SwSettings).GetFields().Where(field => field.IsDefined(typeof(SwOverwritableSettingsField), false)).ToArray();

            foreach (var field in writableFields)
            {
                var fieldStorageKey = GetSettingsStorageKey(field.Name);

                if (dict.ContainsKey(field.Name))
                {
                    if (field.FieldType == typeof(bool))
                    {
                        bool result;

                        switch (dict[field.Name])
                        {
                            case bool boolValue:
                                result = boolValue;

                                break;
                            case string stringValue when stringValue.Equals("true"):
                                result = true;

                                break;
                            case string stringValue when stringValue.Equals("false"):
                                result = false;

                                break;
                            default:
                                continue;
                        }

                        keyValueStore.SetBoolean(fieldStorageKey, result);
                        keyValueStore.Save();
                        field.SetValue(this, result);
                    }
                }
                else
                {
                    PlayerPrefs.DeleteKey(fieldStorageKey);
                    field.SetValue(this, field.GetValue(SettingsFromResource));
                }
            }
        }

        private static string GetSettingsStorageKey(string fieldName)
        {
            return $"{SettingsStoragePrefix}{fieldName}";
        }

        public bool AreIosChinaSettingsValid
        {
            get { return !string.IsNullOrEmpty(iosChinaAppId) && !string.IsNullOrEmpty(iosChinaGameId) && !string.IsNullOrEmpty(iosChinaGameAnalyticsGameKey) && !string.IsNullOrEmpty(iosChinaGameAnalyticsSecretKey) && !string.IsNullOrEmpty(adMobIosChinaAppId) && !string.IsNullOrEmpty(ironSourceIosChinaAppKey); }
        }

        public void Init(ISwKeyValueStore keyValueStore)
        {
            SettingsFromResource = this;

            if (keyValueStore != null)
            {
                ReadSettings(keyValueStore);
            }
        }

        public bool IsDebugEnabled ()
        {
            return enableDebug;
        }

        public bool IsPrivacyPolicyEnabled ()
        {
            return !iosChinaBuildEnabled;
        }

        public string GetGameId ()
        {
            return GameId;
        }

        public string GetAppKey ()
        {
            return IronSourceAppKey;
        }
    }
}