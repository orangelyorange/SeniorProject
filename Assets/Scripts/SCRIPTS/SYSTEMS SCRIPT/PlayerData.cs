using UnityEngine;
[System.Serializable]
public class PlayerData
{
    public int playerHealth;
    public float[] playerPosition;
    public int playerInventory;
    
    //constructor to initialize player data
    public PlayerData(int playerCurrentHealth, int playerCurrentInventory,Vector2 playerCurrentPosition)
    {
        playerHealth = playerCurrentHealth;
        playerInventory = playerCurrentInventory;
        
        
        //stores the x and y coordinates in a simple floar array
        playerPosition = new float[2];
        playerPosition[0] = playerCurrentPosition.x;
        playerPosition[1] = playerCurrentPosition.y;
    }
}
