using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwDeepLinkHandler
    {
        #region --- Constants ---

        private const string GetCurrentUserEndpoint = "https://super-api.supersonic.com/v1/partners/users/me";
        private const string SettingsPrefix = "settings.";

        #endregion


        #region --- Members ---

        protected internal readonly Dictionary<string, string> DeepLinkParams = new Dictionary<string, string>();
        protected readonly ISwSettings Settings;
        protected readonly ISwWebRequestClient WebRequestClient;

        #endregion


        #region --- Properties ---

        protected internal Dictionary<string, string> DeepLinkParamsClone { get; private set; } = new Dictionary<string, string>();

        #endregion


        #region --- Construction ---

        public SwDeepLinkHandler(ISwSettings settings, ISwWebRequestClient webRequestClient)
        {
            Settings = settings;
            WebRequestClient = webRequestClient;
        }

        #endregion


        #region --- Public Methods ---

        public IEnumerator SetupDeepLink ()
        {
            Application.deepLinkActivated += OnDeepLinkActivated;

            // Handle deep link when app is opened via deep link
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                yield return ApplyDeepLinkByUrl(Application.absoluteURL);
            }
        }

        #endregion


        #region --- Private Methods ---

        internal static string GetDeepLinkScheme(string gameId)
        {
            return $"sw.{gameId.ToLower()}";
        }

        /// <summary>
        ///     Extract parameters from wisdom deep link url
        /// </summary>
        /// <param name="url"></param>
        protected internal IEnumerator ApplyDeepLinkByUrl(string url)
        {
            SwInfra.Logger.Log($"OnDeepLinkActivated | {url}");
            var uri = new Uri(url);

            if (uri.Scheme.Equals(GetDeepLinkScheme(Settings.GetGameId())))
            {
                var paramsDictionary = new Dictionary<string, string>();
                var urlParts = uri.PathAndQuery.Split('?');

                if (urlParts.Length != 2) yield break;
                var paramAndValueItems = urlParts[1].Split('&');

                foreach (var paramAndValue in paramAndValueItems)
                {
                    var keyValue = paramAndValue.Split('=');
                    var key = "";
                    var value = "true";

                    if (keyValue.Length > 1)
                    {
                        key = keyValue[0];
                        value = Uri.UnescapeDataString(keyValue[1]);
                    }
                    else
                    {
                        key = paramAndValue;
                    }

                    value = paramsDictionary[key] = value;
                    SwInfra.Logger.Log($"Deep Link Param | {key}={value}");
                }

                yield return VerifyDeepLinkParams(paramsDictionary);

                if (paramsDictionary.Any())
                {
                    DeepLinkParams.SwMerge(paramsDictionary);
                    DeepLinkParamsClone = new Dictionary<string, string>(DeepLinkParams);
                }

                OnDeepLinkParamsResolve();
            }
        }

        protected virtual bool DoesKeyRequireAdminVerification(string key)
        {
            return key.StartsWith(SettingsPrefix) || key.Equals(SwConfigConstants.SwDictionaryKey);
        }

        protected virtual void OnDeepLinkParamsResolve ()
        {
            HandleSettingsOverwriteParams();
        }

        private void HandleSettingsOverwriteParams ()
        {
            // pass only pairs where the key start with "settings." (and remove this prefix)
            Settings.OverwritePartially(DeepLinkParams.Where(pair => pair.Key.StartsWith(SettingsPrefix)).ToDictionary(k => k.Key.Replace(SettingsPrefix, ""), k => (object)k.Value), SwInfra.KeyValueStore);
        }

        private IEnumerator VerifyDeepLinkParams(Dictionary<string, string> deepLinkParams)
        {
            if (!deepLinkParams.Any() || Debug.isDebugBuild)
            {
                yield break;
                ;
            }

            var isAdminVerificationRequired = deepLinkParams.Keys.Where(DoesKeyRequireAdminVerification).Any();
            var isVerified = false;

            var response = new SwWebResponse();
            var authorizationHeaderValue = deepLinkParams.SwSafelyGet("authorization", "");

            if (!string.IsNullOrEmpty(authorizationHeaderValue))
            {
                var headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {authorizationHeaderValue.Replace("Bearer ", "")}" }
                };

                yield return WebRequestClient.Get(GetCurrentUserEndpoint, response, 0, headers);

                if (response.DidSucceed)
                {
                    try
                    {
                        var platformUser = JsonUtility.FromJson<SwPlatformUser>(response.Text);

                        if (platformUser == null)
                        {
                            throw new SwException("platformUser == null");
                        }

                        if (isAdminVerificationRequired)
                        {
                            isVerified = platformUser.role.isAdmin;
                        }
                        else
                        {
                            isVerified = true;
                        }
                    }
                    catch (Exception e)
                    {
                        SwInfra.Logger.LogError($"SwDeepLinkHandler | VerifyDeepLinkParams | Cannot deserialize SwPlatformUser from response={response.Text} | error={e.Message}");
                    }
                }
            }

            if (isVerified)
            {
                SwInfra.Logger.Log(() => $"DeepLink | Verified deep link params | {SwJsonParser.Serialize(deepLinkParams)}");
            }
            else
            {
                SwInfra.Logger.LogError(() => $"Deep link params verification failed | {SwJsonParser.Serialize(deepLinkParams)}");
                deepLinkParams.Clear();
            }
        }

        #endregion


        #region --- Event Handler ---

        private void OnDeepLinkActivated(string url)
        {
            SwInfra.CoroutineService.StartCoroutine(ApplyDeepLinkByUrl(url));
        }

        #endregion


        #region --- Inner Classes ---

        [Serializable]
        private class SwPlatformUser
        {
            #region --- Members ---

            public SwPlatformUserRole role;

            #endregion
        }

        [Serializable]
        private class SwPlatformUserRole
        {
            #region --- Members ---

            public bool isAdmin;

            #endregion
        }

        #endregion
    }

    internal class SwDeepLinkConstants
    {
        #region --- Constants ---

        internal const string DeepLinkHost = "supersonic.com";

        #endregion
    }
}