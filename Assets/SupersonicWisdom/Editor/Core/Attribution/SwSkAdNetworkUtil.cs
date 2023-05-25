#if UNITY_IOS
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor.iOS.Xcode;

namespace SupersonicWisdomSDK.Editor
{
    internal static class SwSkAdNetworkUtil
    {
        [Serializable]
        class SwSkAdNetworksResponse
        {
            public string[] networks;
        }
    
        private const string SkAdNetworksUrl = "https://assets.mobilegamestats.com/wisdom/skadnetwork/networks.json";

        public static string[] FetchSkAdNetworks()
        {
            UnityWebRequest unityWebRequest =
                UnityWebRequest.Get(SkAdNetworksUrl);
            var webRequest = unityWebRequest.SendWebRequest();

            while (!webRequest.isDone)
            {
                Thread.Sleep(300);
            }

            if (unityWebRequest.isHttpError || unityWebRequest.isNetworkError)
            {
                throw new SwException("Cannot fetch SKAdNetwork JSON");
            }

            var response = JsonUtility.FromJson<SwSkAdNetworksResponse>(unityWebRequest.downloadHandler.text);
            if (response == null)
            {
                throw new SwException("Cannot parse SKAdNetwork JSON");
            }

            return response.networks ?? new string[]{};
        }

        public static void InjectSkAdNetworks(PlistDocument plist, string[] skAdNetworks )
        {
            PlistElementArray networks = plist.root.CreateArray(SwAttributionConstants.SkAdNetworkItemsKey);
            foreach (var network in skAdNetworks)
            {
                PlistElementDict networkDict = networks.AddDict();
                networkDict.SetString(SwAttributionConstants.SkAdNetworkIdentifierKey, network);
            }
        }
    }
}
#endif