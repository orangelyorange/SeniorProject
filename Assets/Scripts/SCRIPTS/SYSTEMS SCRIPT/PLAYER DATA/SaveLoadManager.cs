    using System.Collections.Generic;
    using UnityEngine.SceneManagement;
    using UnityEngine;
    using System.IO;

    public class SaveLoadManager : MonoBehaviour
    {
        public static SaveLoadManager Instance; //singleton instance so that it can be accessed from other scripts
        private string saveFilePath;
        private PlayerData pendingLoadData; //holds data that is waiting to be loaded when the scene is ready

        void Awake()
        {
            //singleton pattern setup to ensure only one instance of this manager exists
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); //persist this manager across scene loads
                saveFilePath = Application.persistentDataPath + "/saveData.json"; //set the path for the save file
                
                //subscribe to the scene loaded event to handle loading data when a new scene is loaded
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else
            {
                Destroy(gameObject); //destroy any duplicate instances of this manager
            }
        }
        
        //call this so save the game
        public void SaveGame(List<QuestItem> playerInventory, Vector2 playerPosition, string levelName)
        {
            //construct a SaveData object with the current game state
            PlayerData playerData = new PlayerData(playerInventory, playerPosition, levelName);
            
            string json = JsonUtility.ToJson(playerData);
            File.WriteAllText(saveFilePath, json);
            
            Debug.Log($"Game Saved! Level: {levelName} | Path: {saveFilePath}");
        }

        public PlayerData LoadGame()
        {
            if (File.Exists(saveFilePath))
            {
                string json = File.ReadAllText(saveFilePath);
                pendingLoadData = JsonUtility.FromJson<PlayerData>(json);
                
                Debug.Log($"Save found. Loading scene: {pendingLoadData.levelName}...");
                
                //Loads the scene
                SceneManager.LoadScene(pendingLoadData.levelName);
            }
            else
            {
                Debug.LogWarning("Game Not Found: " + saveFilePath);
            }
            return null;
        } 
        
        //calls when new scene is loaded to apply the loaded data to the player
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            //checks if we have pending load data and if the loaded scene matches the saved level name
            if (pendingLoadData != null && scene.name == pendingLoadData.levelName)
            {
                //finds the player spawned in the scene
                Player player = FindObjectOfType<Player>();
                PlayerQuestItemInventory playerInventory = FindObjectOfType<PlayerQuestItemInventory>();

                if (player != null && playerInventory != null)
                {
                    //apply inventory
                    playerInventory.inventory = pendingLoadData.questInventory;
                    
                    //apply position data
                    Vector2 loadedPosition = new Vector2(pendingLoadData.playerPosition[0], pendingLoadData.playerPosition[1]);
                    
                    Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                    if (rb != null) rb.linearVelocity = Vector2.zero;
                    
                    player.transform.position = loadedPosition;
                    
                    Debug.Log($"Data applied to player in scene: {scene.name}");
                    
                    //clears data so it does not automatically apply again if we load another scene
                    pendingLoadData = null;
                    
                }
                else
                {
                    Debug.LogWarning("SaveLoadManager could not find the Player in the loaded scene!");
                }
            }
        }
        
        public bool HasSaveFile()
        {
            return File.Exists(saveFilePath);
        }

        private void OnDestroy()
        {
            // cleans up the scene loaded event subscription when this manager is destroyed to prevent memory leaks
            if (Instance == this)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }
        
    }
