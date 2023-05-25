using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SupersonicWisdomSDK.Editor;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class SwFileUtils
{
    #region --- Constants ---

    public const string UNITY_IGNORED_FILE_SUFFIX = "~";
    public const string HIDDEN_FILE_PREFIX = ".";
    public const string META_FILE_EXTENSION = ".meta";
    public const string UNITY_PACKAGE_FILE_EXTENSION = ".unitypackage";

    #endregion


    #region --- Public Methods ---

    public static bool DeleteDirectory(string path)
    {
        bool didDelete = false;
        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            
            didDelete = true;
        }
        catch (Exception e)
        {
            SwEditorLogger.LogError(e);
        }

        return didDelete;
    }

    public static bool CopyFile(string sourcePath, string destinationPath, bool shouldOverwrite = false, bool ignoreMissing = false)
    {
        var didCopy = false;

        try
        {
            File.Copy(sourcePath, destinationPath, shouldOverwrite);
            didCopy = true;
        }
        catch (FileNotFoundException e)
        {
            // Gracefully ignore
            didCopy = ignoreMissing;
        }
        catch (Exception e)
        {
            // Possible exceptions can be found here: https://learn.microsoft.com/en-us/dotnet/api/system.io.file.copy
            SwEditorLogger.LogWarning("Failed to copy '" + sourcePath + "' to '" + destinationPath + "'\nError:" + e);
        }

        return didCopy;
    }

    public static bool CreateDirectory(string path)
    {
        bool didCreate = false;
        try
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            didCreate = true;
        }
        catch (Exception e)
        {
            SwEditorLogger.LogError(e);
        }

        return didCreate;
    }

    public static bool TryMoveOrRenameFile(string sourcePath, string destinationPath)
    {
        var didRename = false;

        try
        {
            if (!File.Exists(destinationPath) && File.Exists(sourcePath))
            {
                File.Move(sourcePath, destinationPath);
                didRename = true;
            }
        }
        catch (Exception e)
        {
            SwEditorLogger.LogError(e);
        }

        return didRename;
    }

    public static bool TryMoveOrRenameFolder(string sourcePath, string destinationPath)
    {
        var didRename = false;

        try
        {
            if (!Directory.Exists(destinationPath) && Directory.Exists(sourcePath))
            {
                Directory.Move(sourcePath, destinationPath);
                didRename = true;
            }
        }
        catch (Exception e)
        {
            SwEditorLogger.LogError(e);
        }

        return didRename;
    }

    public static string[] GetFolderContent(string folderPath, bool recursive = false, params string[] excludedFileExtension)
    {
        var allFiles = new List<string>();

        try
        {
            if (!Directory.Exists(folderPath)) return allFiles.ToArray();

            allFiles = Directory.GetFiles(folderPath).ToList();

            allFiles = allFiles.Where((item, index) =>
            {
                var included = true;

                foreach (var excludedExtension in excludedFileExtension)
                {
                    var extensionToExclude = (excludedExtension.StartsWith(".") ? "" : ".") + excludedExtension;
                    included &= !item.EndsWith(extensionToExclude);
                }

                return included;
            }).ToList();

            if (!recursive) return allFiles.ToArray();

            var subFolders = new DirectoryInfo(folderPath).GetDirectories();

            foreach (var subDirectoryInfo in subFolders)
            {
                var subFiles = GetFolderContent(subDirectoryInfo.FullName, true, excludedFileExtension);
                allFiles.AddRange(subFiles);
            }
        }
        catch (Exception e)
        {
            SwEditorLogger.LogError(e);
        }

        return allFiles.ToArray();
    }

    #endregion


    #region --- Private Methods ---

    internal static string ReadJsonAttributeFromFile(string fileSearchPattern, params string[] jsonSearchPattern)
    {
        var jsonObject = ReadJsonFile(fileSearchPattern);

        return (string) jsonObject?.SelectToken(string.Join(".", jsonSearchPattern));
    }

    internal static bool DeleteFilesAtPaths(string[] paths, bool shouldAlsoDeleteMeta = true, bool shouldAlsoDeleteEmptyContainingFolder = true)
    {
        var didDeleteAll = true;
        
        foreach (var fileOrFolderPath in paths)
        {
            var path = "./" + fileOrFolderPath;

            var deleteCandidateFileInfo = new FileInfo(path);
            didDeleteAll &= deleteCandidateFileInfo.SwTryDelete();

            if (shouldAlsoDeleteMeta)
            {
                var metaOfTheFile = new FileInfo(path + META_FILE_EXTENSION);
                didDeleteAll &= metaOfTheFile.SwTryDelete();
            }

            if (shouldAlsoDeleteEmptyContainingFolder)
            {
                var folderPath = deleteCandidateFileInfo.Directory?.FullName ?? "";

                if (string.IsNullOrEmpty(folderPath)) continue;
                if (GetFolderContent(folderPath, true).Length > 0) continue;

                try
                {
                    // Delete empty folder
                    Directory.Delete(folderPath, true); // Still using `recursive == true` - sometimes the folder may contain empty folders so this boolean is necessary
                }
                catch (Exception e)
                {
                    if (!(e is DirectoryNotFoundException))
                    {
                        didDeleteAll = false;
                        SwEditorLogger.LogError(e);
                    }
                }

                if (shouldAlsoDeleteMeta)
                {
                    var metaOfFolderOfDeletedFile = new FileInfo(folderPath + META_FILE_EXTENSION);
                    didDeleteAll &= metaOfFolderOfDeletedFile.SwTryDelete();
                }
            }
        }

        return didDeleteAll;
    }

    internal static double FileSizeInMb(string filePath)
    {
        if (!File.Exists(filePath)) return -1;

        var resultZipFileInfo = new FileInfo(filePath);

        return (double) resultZipFileInfo.Length / 1024 / 1024;
    }

    internal static bool SwTryDelete(this FileInfo self)
    {
        if (!self.Exists) return true;

        var deleted = false;

        try
        {
            self.Delete();
            deleted = true;
        }
        catch (Exception e)
        {
            SwEditorLogger.LogError(e);
        }

        return deleted;
    }

    private static string ReadFile(string fileSearchPattern)
    {
        var dir = new DirectoryInfo(Application.dataPath);
        var files = dir.GetFiles(fileSearchPattern);

        if (files.Length == 0) return string.Empty;

        var source = new StreamReader(files.First().FullName);
        var fileContents = source.ReadToEnd();
        source.Close();

        return fileContents;
    }

    private static JObject ReadJsonFile(string fileSearchPattern)
    {
        var fileContent = ReadFile(fileSearchPattern);
        var googleServices = JObject.Parse(fileContent);

        return googleServices;
    }

    #endregion
}