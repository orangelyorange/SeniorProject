using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// SaveData represents a single save file with metadata.
/// </summary>
[System.Serializable]
public class SaveData
{
    public string saveFileName;
    public string lastSceneName;
    public DateTime saveDateTime;
    public int playTime; // in seconds
    public string playerLevel; // or any other game progress data

    public SaveData(string sceneName)
    {
        lastSceneName = sceneName;
        saveDateTime = DateTime.Now;
        playTime = 0;
        playerLevel = "Level 1";
        saveFileName = $"save_{saveDateTime:yyyy_MM_dd_HH_mm_ss}.json";
    }
}

/// <summary>
/// SaveSystem handles all save/load operations for the game.
/// Uses JSON serialization to store game data.
/// </summary>
public class SaveSystem
{
    private string saveFolderPath;
    private const string SAVE_FOLDER_NAME = "GameSaves";

    public SaveSystem()
    {
        // Initialize the save folder path
        saveFolderPath = Path.Combine(Application.persistentDataPath, SAVE_FOLDER_NAME);

        // Create the folder if it doesn't exist
        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
            Debug.Log($"Created save folder at: {saveFolderPath}");
        }
    }

    /// <summary>
    /// Saves game data to a JSON file.
    /// </summary>
    public void SaveGame(SaveData saveData)
    {
        try
        {
            string filePath = Path.Combine(saveFolderPath, saveData.saveFileName);
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(filePath, json);
            Debug.Log($"Game saved to: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving game: {e.Message}");
        }
    }

    /// <summary>
    /// Loads a specific save file by name.
    /// </summary>
    public SaveData LoadSave(string fileName)
    {
        try
        {
            string filePath = Path.Combine(saveFolderPath, fileName);

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                SaveData loadedData = JsonUtility.FromJson<SaveData>(json);
                Debug.Log($"Game loaded from: {filePath}");
                return loadedData;
            }
            else
            {
                Debug.LogWarning($"Save file not found: {filePath}");
                return null;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading game: {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// Loads the most recent save file.
    /// </summary>
    public SaveData LoadMostRecentSave()
    {
        try
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(saveFolderPath);
            FileInfo[] files = directoryInfo.GetFiles("*.json");

            if (files.Length == 0)
            {
                Debug.LogWarning("No save files found.");
                return null;
            }

            // Sort by last write time and get the most recent
            FileInfo mostRecentFile = files.OrderByDescending(f => f.LastWriteTime).First();
            return LoadSave(mostRecentFile.Name);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading most recent save: {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// Checks if any save files exist.
    /// </summary>
    public bool HasSaveFiles()
    {
        try
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(saveFolderPath);
            FileInfo[] files = directoryInfo.GetFiles("*.json");
            return files.Length > 0;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error checking save files: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// Gets all available save files.
    /// </summary>
    public List<SaveData> GetAllSaves()
    {
        List<SaveData> saves = new List<SaveData>();

        try
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(saveFolderPath);
            FileInfo[] files = directoryInfo.GetFiles("*.json");

            foreach (FileInfo file in files.OrderByDescending(f => f.LastWriteTime))
            {
                SaveData saveData = LoadSave(file.Name);
                if (saveData != null)
                {
                    saves.Add(saveData);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error getting all saves: {e.Message}");
        }

        return saves;
    }

    /// <summary>
    /// Deletes a specific save file.
    /// </summary>
    public bool DeleteSave(string fileName)
    {
        try
        {
            string filePath = Path.Combine(saveFolderPath, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"Save file deleted: {filePath}");
                return true;
            }
            else
            {
                Debug.LogWarning($"Save file not found: {filePath}");
                return false;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error deleting save: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// Gets the save folder path for debugging purposes.
    /// </summary>
    public string GetSaveFolderPath()
    {
        return saveFolderPath;
    }
}