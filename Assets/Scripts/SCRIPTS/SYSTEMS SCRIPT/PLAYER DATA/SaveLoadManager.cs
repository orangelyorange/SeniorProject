using UnityEngine;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
    private string saveFilePath;

    void Awake()
    {
        //save file path is set to a file named "saveData.json" in the persistent data path of the application
        saveFilePath = Application.persistentDataPath + "/saveData.json";
    }
    
    //call this so save the game
    public void SaveGame(int playerHealth, int playerInventory, Vector2 playerPosition)
    {
        //construct a SaveData object with the current game state
        PlayerData playerData = new PlayerData(playerHealth, playerInventory, playerPosition);
        
        string json = JsonUtility.ToJson(playerData);
        File.WriteAllText(saveFilePath, json);
        
        Debug.Log("Game Saved: " + json);
    }

    public PlayerData LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
            
            Debug.Log("Game Loaded: " + json);
            return playerData;
        }
        else
        {
            Debug.LogWarning("Game Not Found: " + saveFilePath);
            return null;
        }
    }
}
