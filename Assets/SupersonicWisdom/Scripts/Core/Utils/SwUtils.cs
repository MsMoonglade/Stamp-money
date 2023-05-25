#if UNITY_EDITOR
using System.IO;
#endif
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
#if UNITY_IOS
using System.Runtime.InteropServices;
using UnityEngine.iOS;
#endif

namespace SupersonicWisdomSDK
{
    internal static class SwUtils
    {
        private const short NON_BETA_VERSION_COMPONENT = 99;
        
        internal static long ComputeVersionId(string sdkVersion)
        {
            if (sdkVersion == null) return 0;
            
            short major = 0, minor = 0, patch = 0, beta = 0;

            string[] components = sdkVersion.Split('.', '-');

            if (components.Length >= 3)
            {
                major = Convert.ToInt16(components[0]);
                minor = Convert.ToInt16(components[1]);
                patch = Convert.ToInt16(components[2]);
                beta = NON_BETA_VERSION_COMPONENT;
            }

            if (components.Length == 4)
            {
                beta = Convert.ToInt16(components[3]);
            }

            return (long) (major * 1e6 + minor * 1e4 + patch * 1e2 + beta);
        }

        internal static string ComputeVersionString(long sdkId)
        {
            var major = (int)(sdkId / 1e6 % 100);
            var minor = (int)(sdkId / 1e4 % 100);
            var patch = (int)(sdkId / 1e2 % 100);
            var beta = (int)(sdkId / 1e0 % 100);
            var betaString = "";
            if (beta != 0 && beta != NON_BETA_VERSION_COMPONENT)
            {
                betaString = "-" + beta;
            }
            
            return $"{major}.{minor}.{patch}{betaString}";
        }
        
        public static Dictionary<string, string> DeserializeQueryString(string str)
        {
            var result = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(str)) return result;

            var parts = str.Split('?');

            if (parts.Length == 0) return result;

            var queryString = parts.Last();
            var pairs = queryString.Split('&');

            foreach (var pair in pairs)
            {
                var entryPair = pair.Split('=');

                if (entryPair.Length == 2)
                {
                    result[entryPair[0]] = UnityWebRequest.UnEscapeURL(entryPair[1]);
                }
            }

            return result;
        }

        public static long OneDayInSeconds = ConvertHoursToSeconds(24);
        public static long TwoDaysInSeconds = ConvertHoursToSeconds(48);

        public static int ConvertHoursToSeconds(int hours)
        {
            return hours * 60 * 60;
        }

        private static readonly SystemLanguage[] RtlLanguages =
        {
            SystemLanguage.Arabic, SystemLanguage.Hebrew
        };

        public static float ConvertMillisToSeconds(long millis)
        {
            return millis / 1000;
        }

        public static string UnityVersion
        {
            get { return lazyUnityVersion.Value; }
        }

        private static readonly Lazy<string> lazyUnityVersion = new Lazy<string>(() =>
        {
            var formattedUnityVersion = "";
            var unityVersionParts = Application.unityVersion.Split('.');

            for (var i = 0; i < unityVersionParts.Length; i++)
            {
                if (int.TryParse(unityVersionParts[i], out _))
                {
                    if (i == 0)
                    {
                        formattedUnityVersion = unityVersionParts[i];
                    }
                    else
                    {
                        formattedUnityVersion += "." + unityVersionParts[i];
                    }
                }
                else
                {
                    var regexVersion = System.Text.RegularExpressions.Regex.Split(unityVersionParts[i], "[^\\d]+");

                    if (regexVersion.Length > 0 && int.TryParse(regexVersion[0], out _))
                    {
                        formattedUnityVersion += "." + regexVersion[0];
                    }
                }
            }

            return formattedUnityVersion;
        });

        public static Lazy<string> LazyPlatformDisplayName = new Lazy<string>(() => IsAndroidTarget() ? "Android" : IsIosTarget() ? "iOS" : "Unknown");

        public static string PlatformDisplayName
        {
            get { return LazyPlatformDisplayName.Value; }
        }

        public static long GetTotalSeconds(DateTime dateTime)
        {
            return (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        public static long GetTotalMilliSeconds(DateTime dateTime)
        {
            return (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        public static long GetMilliseconds(DateTime dateTime)
        {
            return (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        /// <summary>
        ///     Use this method instead of dateTime.ToString() to avoid exceptions that occur in Unity 2020 and above.
        /// </summary>
        /// <param name="dateTime">The date instance you want to convert</param>
        /// <returns></returns>
        public static string ToStringDate(DateTime dateTime)
        {
            return dateTime.SwToString();
        }

        /// </summary>
        /// <param name="dateTime">The date instance you want to convert</param>
        /// <param name="format">A date string format you wish to apply</param>
        /// <returns></returns>
        public static string ToStringDate(DateTime dateTime, string format)
        {
            return dateTime.SwToString(format);
        }

#if UNITY_EDITOR
        public static string WhereIs(string file)
        {
            string[] assets = { Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar };
            var myFile = new DirectoryInfo("Assets").GetFiles(file, SearchOption.AllDirectories);
            
            if (myFile.Length == 0) return string.Empty;
            
            var temp = myFile[0].ToString().Split(assets, 2, StringSplitOptions.None);

            return "Assets" + Path.DirectorySeparatorChar + temp[1];
        }
#endif
#if UNITY_IOS && !UNITY_EDITOR
		[DllImport("__Internal")]
		private static extern string _swGetCountry();
#endif
        internal static string GetCountry ()
        {
#if UNITY_EDITOR
            return "";
#elif UNITY_IOS
            return _swGetCountry();
#elif UNITY_ANDROID
            var locale = new AndroidJavaClass("java.util.Locale");
            var defautLocale = locale.CallStatic<AndroidJavaObject>("getDefault");
            var country = defautLocale.Call<string>("getCountry");
            return country;
#else
            return "";
#endif
        }

        public static Resolution GetNativeResolution ()
        {
            var res = new Resolution
            {
                width = Screen.currentResolution.width,
                height = Screen.currentResolution.height,
                refreshRate = Screen.currentResolution.refreshRate
            };
#if UNITY_IOS && !UNITY_EDITOR
            res.width = _swGetNativeWidth();
            res.height = _swGetNativeHeight();
#elif UNITY_ANDROID && !UNITY_EDITOR
            var (androidNativeWidth, androidNativeHeight) = GetAndroidNativeResolution();
            if (androidNativeWidth != 0 && androidNativeHeight != 0)
            {
                res.width = androidNativeWidth;
                res.height = androidNativeHeight;
            }
#endif
            return res;
        }

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        extern private static int _swGetNativeHeight();

        [DllImport("__Internal")]
        extern private static int _swGetNativeWidth();
#endif

        public static Tuple<int, int> GetAndroidNativeResolution ()
        {
            //TODO Add 2 fallbacks: (1) https://developer.android.com/reference/android/view/WindowMetrics#getBounds() or (2) https://developer.android.com/reference/android/view/Window#getDecorView()
            var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activityInstance = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

            var width = 0;
            var height = 0;

            var windowManagerInstance = activityInstance.Call<AndroidJavaObject>("getWindowManager");

            try
            {
                var pointInstance = new AndroidJavaObject("android.graphics.Point");
                var displayInstance = windowManagerInstance.Call<AndroidJavaObject>("getDefaultDisplay");
                displayInstance.Call("getRealSize", pointInstance);
                width = pointInstance.Get<int>("x");
                height = pointInstance.Get<int>("y");

                SwInfra.Logger.Log($"Measured native resolution of {width}x{height}");
            }
            catch (Exception e)
            {
                SwInfra.Logger.LogError("Cannot measure native resolution. Exception: " + e);
            }

            return new Tuple<int, int>(width, height);
        }

        public static bool IsRtlLanguage(SystemLanguage lang)
        {
            return RtlLanguages.ToList().Contains(lang);
        }

        internal static string SerializeToQueryString(Dictionary<string, object> dict)
        {
            if (dict == null) return "";
            if (dict.Count == 0) return "";

            var serializedKeyValue = dict.Where(keyValue => keyValue.Value != null).Select(keyValue => $"{keyValue.Key}={Uri.EscapeDataString(keyValue.Value.ToString())}");

            return string.Join("&", serializedKeyValue);
        }

        public static string GetSystemLanguageIso6391 ()
        {
            var lang = Application.systemLanguage;
            var code = "en";

            switch (lang)
            {
                case SystemLanguage.Afrikaans:
                    code = "af";

                    break;
                case SystemLanguage.Arabic:
                    code = "ar";

                    break;
                case SystemLanguage.Basque:
                    code = "eu";

                    break;
                case SystemLanguage.Belarusian:
                    code = "by";

                    break;
                case SystemLanguage.Bulgarian:
                    code = "bg";

                    break;
                case SystemLanguage.Catalan:
                    code = "ca";

                    break;
                case SystemLanguage.Chinese:
                    code = "zh";

                    break;
                case SystemLanguage.ChineseSimplified:
                    code = "zh";

                    break;
                case SystemLanguage.ChineseTraditional:
                    code = "zh";

                    break;
                case SystemLanguage.Czech:
                    code = "cs";

                    break;
                case SystemLanguage.Danish:
                    code = "da";

                    break;
                case SystemLanguage.Dutch:
                    code = "nl";

                    break;
                case SystemLanguage.English:
                    code = "en";

                    break;
                case SystemLanguage.Estonian:
                    code = "et";

                    break;
                case SystemLanguage.Faroese:
                    code = "fo";

                    break;
                case SystemLanguage.Finnish:
                    code = "fi";

                    break;
                case SystemLanguage.French:
                    code = "fr";

                    break;
                case SystemLanguage.German:
                    code = "de";

                    break;
                case SystemLanguage.Greek:
                    code = "el";

                    break;
                case SystemLanguage.Hebrew:
                    code = "iw";

                    break;
                case SystemLanguage.Hungarian:
                    code = "hu";

                    break;
                case SystemLanguage.Icelandic:
                    code = "is";

                    break;
                case SystemLanguage.Indonesian:
                    code = "in";

                    break;
                case SystemLanguage.Italian:
                    code = "it";

                    break;
                case SystemLanguage.Japanese:
                    code = "ja";

                    break;
                case SystemLanguage.Korean:
                    code = "ko";

                    break;
                case SystemLanguage.Latvian:
                    code = "lv";

                    break;
                case SystemLanguage.Lithuanian:
                    code = "lt";

                    break;
                case SystemLanguage.Norwegian:
                    code = "no";

                    break;
                case SystemLanguage.Polish:
                    code = "pl";

                    break;
                case SystemLanguage.Portuguese:
                    code = "pt";

                    break;
                case SystemLanguage.Romanian:
                    code = "ro";

                    break;
                case SystemLanguage.Russian:
                    code = "ru";

                    break;
                case SystemLanguage.SerboCroatian:
                    code = "sh";

                    break;
                case SystemLanguage.Slovak:
                    code = "sk";

                    break;
                case SystemLanguage.Slovenian:
                    code = "sl";

                    break;
                case SystemLanguage.Spanish:
                    code = "es";

                    break;
                case SystemLanguage.Swedish:
                    code = "sv";

                    break;
                case SystemLanguage.Thai:
                    code = "th";

                    break;
                case SystemLanguage.Turkish:
                    code = "tr";

                    break;
                case SystemLanguage.Ukrainian:
                    code = "uk";

                    break;
                case SystemLanguage.Unknown:
                    code = "en";

                    break;
                case SystemLanguage.Vietnamese:
                    code = "vi";

                    break;
            }

            return code;
        }

        public static bool IsRunningOnEditor ()
        {
#if UNITY_EDITOR
            return true;
#endif
            return false;
        }

        public static bool IsRunningOnDevice ()
        {
            return !IsRunningOnEditor();
        }

        public static bool IsRunningOnAndroid ()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return true;
#endif
            return false;
        }

        public static bool IsRunningOnIos ()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return true;
#endif
            return false;
        }

        public static bool IsAndroidTarget ()
        {
#if UNITY_ANDROID
            return true;
#endif
            return false;
        }

        public static bool IsIosTarget ()
        {
#if UNITY_IOS
            return true;
#endif
            return false;
        }

        public static bool IsIosSandbox
        {
            get { return LazyIsIosSandbox.Value; }
        }

        private static readonly Lazy<bool> LazyIsIosSandbox = new Lazy<bool>(() =>
        {
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            var isSandbox = false;
#if UNITY_IOS && !UNITY_EDITOR
            isSandbox = _swIsSandbox();
#endif
            SwInfra.Logger.Log($"SwSandboxDetector | IsIosSandbox | {isSandbox}");

            return isSandbox;
        });

        public static Sprite AppIconSprite
        {
            get { return LazyAppIconSprite.Value; }
        }

        private static readonly Lazy<Sprite> LazyAppIconSprite = new Lazy<Sprite>(() =>
        {
            if (AppIconBytes == null) return null;

            // Texture width & height are dummy since they will be overwritten!
            var texture2D = new Texture2D(1, 1);
            texture2D.LoadImage(AppIconBytes);
            var appIconSprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(.5f, .5f));

            return appIconSprite;
        });

        public static byte[] AppIconBytes
        {
            get { return LazyAppIconBytes.Value; }
        }

        private static readonly Lazy<byte[]> LazyAppIconBytes = new Lazy<byte[]>(() => Resources.Load<TextAsset>(SwConstants.AppIconResourcesPath)?.bytes);

        public static void LockUI ()
        {
            Object.FindObjectOfType<EventSystem>().enabled = false;
            Time.timeScale = 0;
            LockScreenRotation();
        }

        private static void LockScreenRotation ()
        {
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
        }

        public static bool IsLandscapeLayout ()
        {
#if UNITY_EDITOR
            return Screen.height <= Screen.width;
#elif (UNITY_IOS || UNITY_ANDROID) && UNITY_2019_4_OR_NEWER
            return Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight;
#elif UNITY_IOS || UNITY_ANDROID
            return Screen.orientation == ScreenOrientation.Landscape;
#else
            return true;
#endif
        }

        public static T Load<T>(string prefabPath) where T : Object
        {
            var suffix = IsLandscapeLayout() ? "l" : "p";

            return Resources.Load<T>($"{prefabPath}_{suffix}");
        }

        public static bool IsRunningOnMainThread
        {
            get { return SwInfra.MainThread == Thread.CurrentThread; }
        }

        public static string Format(this string format, params object[] list)
        {
            return string.Format(format, list);
        }

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        extern private static bool _swIsSandbox();
#endif
        public static string GenerateFileSizeString(ulong bytes)
        {
            if (bytes < 1024f)
            {
                return $"{bytes} bytes";
            }

            if ((double)bytes < 1024f * 1024f)
            {
                return $"{(double)bytes / 1024f:0.0}KB";
            }

            return $"{(double)bytes / 1024f / 1024f:0.0}MB";
        }
        
        /// <summary>
        /// This method modifies the stack and sorts it, the first element popped will be the smallest by default (controlled by ascending)
        /// </summary>
        /// <param name="stack">any stack to sort</param>
        /// <param name="ascending">if true the first element popped will be the smallest</param>
        /// <typeparam name="T">could be anything as long as it's comparable</typeparam>
        public static void SortStack<T>(this Stack<T> stack, bool ascending = true) where T : IComparable<T>
        {
            List<T> list = stack.ToList();
            list.Sort();

            if (ascending)
            {
                list.Reverse();
            }
            
            stack.Clear();
            foreach (T element in list)
            {
                stack.Push(element);
            }
        }
    }
}