using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.Networking;

namespace SupersonicWisdomSDK.Editor
{
    internal static class SwPlatformCommunication
    {
        #region --- Public Methods ---

        public static Dictionary<string, string> CreateAuthorizationHeadersDictionary ()
        {
            return string.IsNullOrWhiteSpace(SwAccountUtils.AccountToken) ? new Dictionary<string, string>() : new Dictionary<string, string>
            {
                { "authorization", "Bearer " + SwAccountUtils.AccountToken }
            };
        }

        #endregion


        #region --- Inner Classes ---

        internal static class URLs
        {
            #region --- Constants ---

            internal const string CURRENT_STAGE_API = BASE_WISDOM_PARTNERS + "current-stage";
            internal const string DOWNLOAD_WISDOM_PACKAGE = BASE_WISDOM_PARTNERS + "download-package";
            internal const string WISDOM_PACKAGE_MANIFEST = BASE_WISDOM_PARTNERS + "package-manifest";

            internal const string LOGIN = BASE_PARTNERS + "login";

            internal const string TITLES = BASE_PARTNERS + "titles?embed=games&limit=0&order=asc&page=0&sort=name&includePrototypes=1";
            
            private const string BASE = "https://super-api.supersonic.com/v1/";
            private const string BASE_PARTNERS = BASE + "partners/";
            private const string BASE_WISDOM_PARTNERS = BASE + "partners/wisdom/";
            
            internal const string WISDOM_UPDATE_CONFIG_URL = "https://assets.mobilegamestats.com/docs/self-update-config-v1.json";
            
            #endregion
        }

        #endregion
    }
}