using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Facebook.Unity.Settings;

namespace SupersonicWisdomSDK.Editor
{
    public class SwEditorUtils
    {
        #region --- Constants ---

        public struct Keys
        {
            public const string CHECKSUM = "checksum";
            public const string ELIGIBLE_WISDOM_STAGE = "eligibleWisdomStage";
        }
        
        private const string IAP_ASSEMBLY_FULL_NAME = "UnityEngine.Purchasing";
        private const string SUPERSONIC_WISDOM_RESOURCE_DIR_NAME = "SupersonicWisdom";
        private const string SUPERSONIC_WISDOM_SETTINGS_ASSET_RESOURCE_FILE_NAME = "Settings";
        private const string SUPERSONIC_WISDOM_ACCOUNT_DATA_ASSET_RESOURCE_FILE_NAME = "SwAccountData";
        internal const string SW_UPDATE_METADATA_FOLDER_NAME = "sw-update-metadata";
        internal const string SW_UPDATE_METADATA_HIDDEN_FOLDER = "./" + SwEditorConstants.ASSETS + "/SupersonicWisdom/." + SW_UPDATE_METADATA_FOLDER_NAME;
        
        public static string SwUpdateMetadataExposedFolder
        {
            get { return SW_UPDATE_METADATA_HIDDEN_FOLDER.Replace("SupersonicWisdom/.", "SupersonicWisdom/"); }
        }

        #endregion

        #region --- Members ---

        private static readonly string SettingsAssetFileRelativePath = $"Resources/{SUPERSONIC_WISDOM_RESOURCE_DIR_NAME}/{SUPERSONIC_WISDOM_SETTINGS_ASSET_RESOURCE_FILE_NAME}.asset";
        private static readonly string AccountDataAssetFileRelativePath = $"Resources/{SUPERSONIC_WISDOM_RESOURCE_DIR_NAME}/{SUPERSONIC_WISDOM_ACCOUNT_DATA_ASSET_RESOURCE_FILE_NAME}.asset";
        private static SwSettingsManager<SwSettings> _settingsManager;
        private static readonly Lazy<SwMainThreadActionsQueue> LazyMainThreadActions = new Lazy<SwMainThreadActionsQueue>(() =>
        {
            // Register to main thread updates, the "Lazy"'s factory mechanism will ensure that it will occur only once.
            EditorApplication.update += OnEditorApplicationUpdate;

            return new SwMainThreadActionsQueue();
        });

        #endregion


        #region --- Properties ---

        public static string AppName
        {
            get { return string.IsNullOrEmpty(SwSettings.appName) ? Application.productName : SwSettings.appName; }
        }

        public static SwSettings SwSettings
        {
            get
            {
                if (_settingsManager == null)
                {
                    _settingsManager = new SwSettingsManager<SwSettings>(null, $"{SUPERSONIC_WISDOM_RESOURCE_DIR_NAME}/{SUPERSONIC_WISDOM_SETTINGS_ASSET_RESOURCE_FILE_NAME}");
                }

                var doesSettingsAssetFileExist = File.Exists($"{Application.dataPath}/{SettingsAssetFileRelativePath}");
                // Fix unity issue in post build script where Resource.Load doesn't always work on first run
                if (_settingsManager.Settings == null && doesSettingsAssetFileExist)
                {
                    var settings = AssetDatabase.LoadAssetAtPath($"Assets/{SettingsAssetFileRelativePath}", typeof(SwSettings)) as SwSettings;
                    _settingsManager.Load(settings);
                }

                return _settingsManager.Settings;
            }
        }

        public static bool AllowEditingSettings { get; set; }

        public static string FacebookAppId
        {
            set
            {
                if (FacebookSettings.AppIds.Count == 0)
                {
                    FacebookSettings.AppIds = new List<string> { value };
                    FacebookSettings.AppLabels = new List<string> { AppName };
                }
                else
                {
                    FacebookSettings.AppIds[0] = value;
                    FacebookSettings.AppLabels[0] = AppName;
                }

                EditorUtility.SetDirty(FacebookSettings.Instance);
            }
            get
            {
                if (FacebookSettings.AppIds.Count == 0) return "";

                return FacebookSettings.AppIds[0];
            }
        }

        internal static SwAccountData SwAccountData
        {
            get
            {
                var assetPath = $"Assets/{AccountDataAssetFileRelativePath}";

                var swAccountData = AssetDatabase.LoadAssetAtPath(assetPath, typeof(SwAccountData)) as SwAccountData;

                if (swAccountData == null)
                {
                    var swAccountDataAsset = ScriptableObject.CreateInstance<SwAccountData>();
                    AssetDatabase.CreateAsset(swAccountDataAsset, assetPath);
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();

                    swAccountData = swAccountDataAsset;
                }

                return swAccountData;
            }
        }
        
        private static SwMainThreadActionsQueue MainThreadActions
        {
            get { return LazyMainThreadActions.Value; }
        }

        #endregion


        #region --- Public Methods ---

        public static void CallOnNextUpdate(Action action)
        {
            new CallOnInspectorUpdate(action);
        }

        public static async Task<bool> ImportPackage(string packagePath, bool isSilent)
        {
            var importListener = new SwImportPackageCallback(errorMessage =>
            {
                var didImport = errorMessage == null;
                
                if (!didImport)
                {
                    SwEditorLogger.LogError(errorMessage);
                }
            });
            
            AssetDatabase.ImportPackage(packagePath, !isSilent);

            while (importListener.IsCompletedSuccessfully == null)
            {
                await Task.Delay(300);
            }
            
            return importListener.IsCompletedSuccessfully.Value;
        }

        public static void OpenSettings()
        {
            if (SwSettings == null)
            {
                CreateSettings();
            }

            Selection.activeObject = SwSettings;
        }

        public static float Percentage(float part, float total)
        {
            if (part == 0 || total == 0) return 0;

            return part / total * 100f;
        }

        /// <summary>
        ///     Runs a given action on the main thread (UI thread).
        ///     In general, when we want to present dialogs, progress bars or make operations like import packages, etc. we must
        ///     call it on the main thread. Otherwise the Unity Editor will ignore our request (it won't crash or throw an error,
        ///     but it will ignore).
        ///     Whenever the process returns from a <see cref="T:System.Threading.Tasks.Task" />, it runs on a secondary thread (
        ///     by a pool or custom). Therefore we should use this
        /// </summary>
        /// <param name="actionToRun">The action that will actually run on the main thread.</param>
        /// <param name="afterDelayMilliseconds">
        ///     The delay to wait before executing the action. Default value is zero (run
        ///     immediately)
        /// </param>
        public static void RunOnMainThread(Action actionToRun, int afterDelayMilliseconds = 0)
        {
            if (afterDelayMilliseconds == 0)
            {
                void SafeRun()
                {
                    try
                    {
                        actionToRun();
                    }
                    catch (Exception mainThreadActionException)
                    {
                        SwEditorLogger.LogError(mainThreadActionException.ToString());
                    }
                }

                var isRunningOnMainThread = !Thread.CurrentThread.IsBackground;

                if (isRunningOnMainThread)
                {
                    SafeRun();
                }
                else
                {
                    MainThreadActions.Add(SafeRun);
                }
            }
            else
            {
                new Task(() =>
                {
                    Thread.Sleep(afterDelayMilliseconds);
                    RunOnMainThread(actionToRun);
                }).Start();
            }
        }

        #endregion


        #region --- Private Methods ---

        internal static bool IsUnityIapAssetInstalled ()
        {
            return DoesAssemblyExists(IAP_ASSEMBLY_FULL_NAME);
        }

        internal static void OpenIronSourceIntegrationManager ()
        {
            var ironSourceMenuType = Type.GetType("IronSourceMenu, Assembly-CSharp-Editor");

            try
            {
                ironSourceMenuType.GetMethod("SdkManagerProd", BindingFlags.Public | BindingFlags.Static).Invoke(null, null);
            }
            catch (Exception)
            {
                Debug.LogError("Could not open IronSource Integration Manager");
            }
        }

        internal static IEnumerator WaitUntilEndOfCompilation ()
        {
            yield return new WaitUntil(() => !EditorApplication.isCompiling);
        }

        private static void CreateSettings ()
        {
            try
            {
                if (_settingsManager.Settings == null)
                {
                    // If the settings asset doesn't exist, then create it. We require a resources folder
                    if (!Directory.Exists($"{Application.dataPath}/Resources"))
                    {
                        Directory.CreateDirectory($"{Application.dataPath}/Resources");
                    }

                    if (!Directory.Exists($"{Application.dataPath}/Resources/{SUPERSONIC_WISDOM_RESOURCE_DIR_NAME}"))
                    {
                        Directory.CreateDirectory($"{Application.dataPath}/Resources/{SUPERSONIC_WISDOM_RESOURCE_DIR_NAME}");
                        Debug.Log($"SupersonicWisdom: Resources/{SUPERSONIC_WISDOM_RESOURCE_DIR_NAME} folder is required to store settings. it was created ");
                    }

                    var assetPath = $"Assets/{SettingsAssetFileRelativePath}";

                    if (File.Exists(assetPath))
                    {
                        AssetDatabase.DeleteAsset(assetPath);
                        AssetDatabase.Refresh();
                    }

                    var asset = ScriptableObject.CreateInstance<SwSettings>();
                    AssetDatabase.CreateAsset(asset, assetPath);
                    AssetDatabase.Refresh();

                    AssetDatabase.SaveAssets();
                    Debug.Log("SupersonicWisdom: Settings file didn't exist and was created");
                    Selection.activeObject = asset;

                    _settingsManager.Load();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("SupersonicWisdom: Error getting Settings in InitAPI: " + e.Message);
            }
        }

        private static bool DoesAssemblyExists(string assemblyFullName)
        {
            return AppDomain.CurrentDomain.GetAssemblies().Any(assembly => assembly.GetName().Name.Equals(assemblyFullName));
        }

        private static void OnEditorApplicationUpdate ()
        {
            MainThreadActions.Run();
        }
        
        public static string Md5(string filePath)
        {
            var md5 = MD5.Create();
            var stream = File.OpenRead(filePath);
            
            var md5Hash = md5.ComputeHash(stream);
            var md5Checksum = BitConverter.ToString(md5Hash).Replace("-", "").ToLowerInvariant();

            return md5Checksum;
        }
        
        #endregion

        public static string Plural(Int64 amount, string single)
        {
            return amount == 1 ? single : $"{single}s";
        }

        public static void DrawGuiLayoutSwLogoLabel(float dimensions)
        {
            GUILayout.Label(SwGameObjectLogo.Logo, GUILayout.Width(dimensions), GUILayout.Height(dimensions));
        }

        public static void DrawClickableButtonLink(string linkTitle, string linkUrl, Action onClick)
        {
            var previousLabelFontStyle = GUI.skin.label.fontStyle;
            GUI.skin.label.fontStyle = FontStyle.Bold;
            var linkButtonWidth = GUI.skin.button.CalcSize(new GUIContent(linkTitle)).x;

            if (GUILayout.Button(new GUIContent(linkTitle, linkUrl), EditorStyles.linkLabel, GUILayout.MaxWidth(linkButtonWidth)))
            {
                onClick();
            }

            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
            GUI.skin.label.fontStyle = previousLabelFontStyle;
        }
    }

    internal class SwImportPackageCallback
    {
        #region --- Members ---

        private readonly AssetDatabase.ImportPackageCallback _onImportCompleted;
        private readonly AssetDatabase.ImportPackageFailedCallback _onImportFailed;
        public bool? IsCompletedSuccessfully { get; private set; }

        #endregion


        #region --- Construction ---

        /// <summary>
        /// This class registers itself as a callback, no need to pass it to any "import" method.
        /// As long as the object of this class lives, it will observe Editor import callbacks.
        /// Once this object is destroyed it will stop observing.
        /// </summary>
        /// <param name="onComplete"></param>
        public SwImportPackageCallback(Action<string> onComplete)
        {
            _onImportCompleted = packageName =>
            {
                UnregisterListeners();
                IsCompletedSuccessfully = true;
                onComplete?.Invoke(null);
            };

            _onImportFailed = (packageName, errorMessage) =>
            {
                UnregisterListeners();
                IsCompletedSuccessfully = false;
                onComplete?.Invoke(errorMessage);
            };

            RegisterListeners();
        }

        private void RegisterListeners()
        {
            AssetDatabase.importPackageCompleted += _onImportCompleted;
            AssetDatabase.importPackageFailed += _onImportFailed;
        }

        private void UnregisterListeners()
        {
            AssetDatabase.importPackageCompleted -= _onImportCompleted;
            AssetDatabase.importPackageFailed -= _onImportFailed;
        }

        ~SwImportPackageCallback()
        {
            UnregisterListeners();
        }
        
        #endregion
    }
}