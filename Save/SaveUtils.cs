namespace AF
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public static class SaveUtils
    {
        public static string SAVE_FILES_FOLDER = "QuickSave";

        static readonly string[] excludedFileNames = { "GamePreferences.json", "steam_autocloud.vdf" };

        static List<FileInfo> GetSaveFiles(string saveFilesLocation, string fileExtension)
        {
            string saveFolderPath = Path.Combine(Application.persistentDataPath, saveFilesLocation);

            if (!Directory.Exists(saveFolderPath))
            {
                return new List<FileInfo>();
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(saveFolderPath);
            var files = directoryInfo.GetFiles();

            return files
                .Where(file => file.Name.EndsWith("." + fileExtension) && !excludedFileNames.Contains(file.Name))
                .OrderByDescending(file => file.CreationTime)
                .ToList();
        }

        public static bool HasSaveFiles(string saveFilesLocation)
        {
            try
            {
                return GetSaveFiles(saveFilesLocation, "json").Any();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while checking for save files: {e.Message}");
                return false;
            }
        }

        public static string GetLastSaveFile(string saveFilesLocation)
        {
            var fileInfos = GetSaveFiles(saveFilesLocation, "json");

            return fileInfos.FirstOrDefault()?.Name.Replace(".json", "") ?? string.Empty;
        }

        public static string[] GetSaveFileNames(string saveFilesLocation)
        {
            return GetSaveFiles(saveFilesLocation, "json")
                .Select(file => file.Name.Replace(".json", ""))
                .ToArray();
        }

        public static Texture2D GetScreenshotFilePath(string saveFilesLocation, string fileName)
        {
            var saveFiles = GetSaveFiles(saveFilesLocation, "jpg");

            string targetFilePath = saveFiles
                .FirstOrDefault(file => file.Name.Replace(".jpg", "") == fileName)?.FullName;

            if (string.IsNullOrEmpty(targetFilePath))
            {
                return null;
            }

            var texture = new Texture2D(2, 2);
            texture.LoadImage(File.ReadAllBytes(targetFilePath));
            return texture;
        }

        public static string CreateNameForSaveFile()
        {
            return $"Save - {SceneManager.GetActiveScene().name} - {DateTime.Now:yyyy-MM-dd HH-mm-ss}";
        }

        public static bool DeleteSaveFile(string saveFilesLocation, string fileName)
        {
            string jsonFilePath = Path.Combine(Application.persistentDataPath, saveFilesLocation, fileName + ".json");
            string jpegFilePath = Path.Combine(Application.persistentDataPath, saveFilesLocation, fileName + ".jpg");

            bool jsonFileDeleted = false;

            if (File.Exists(jsonFilePath))
            {
                try
                {
                    File.Delete(jsonFilePath);
                    jsonFileDeleted = true;
                    Debug.Log($"Deleted save file: {jsonFilePath}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error deleting save file: {e.Message}");
                }
            }
            else
            {
                Debug.LogWarning($"Save file does not exist: {jsonFilePath}");
            }

            if (File.Exists(jpegFilePath))
            {
                try
                {
                    File.Delete(jpegFilePath);
                    Debug.Log($"Deleted screenshot file: {jpegFilePath}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error deleting screenshot file: {e.Message}");
                }
            }
            else
            {
                Debug.LogWarning($"Screenshot file does not exist: {jpegFilePath}");
            }

            return jsonFileDeleted;
        }
    }
}
