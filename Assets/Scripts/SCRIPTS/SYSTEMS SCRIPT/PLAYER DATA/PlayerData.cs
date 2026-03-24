using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public float[] playerPosition;
    public List<QuestItem> questInventory; //List to store quest items, can be expanded to include more item types in the future
    public string levelName; //stores the scene name for the current level, useful for loading the correct scene when loading player data
    
    //constructor to initialize player data
    public PlayerData(List<QuestItem> currentInventory,Vector2 playerCurrentPosition, string currentLevelName)
    {
        
        //creates a new list to store the player's quest inventory
        questInventory = new List<QuestItem>(currentInventory);
        
        
        //stores the x and y coordinates in a simple floar array
        playerPosition = new float[2];
        playerPosition[0] = playerCurrentPosition.x;
        playerPosition[1] = playerCurrentPosition.y;
        
        levelName = currentLevelName; //stores the current level name
    }
}
