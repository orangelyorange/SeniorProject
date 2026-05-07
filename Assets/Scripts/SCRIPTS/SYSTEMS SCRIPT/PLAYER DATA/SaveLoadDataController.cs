using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadDataController : MonoBehaviour
{
   [Header("Player References")] 
   public Player playerMovement; //player movement script reference
   public PlayerQuestItemInventory playerInventory; //quest inventory script reference

   public void SaveGameButton()
   {
      if (playerMovement != null && playerInventory != null)
      {
         //grabs the name of the level player is currently in
         string currentSceneName = SceneManager.GetActiveScene().name;

         if (SaveLoadManager.Instance != null)
         {
            Vector2 playerPosition = playerMovement.transform.position;
            List<JournalCollectedPage> journalPages = JournalProgressManager.GetOrCreate().GetCollectedPages();
            SaveLoadManager.Instance.SaveGame(playerInventory.inventory, playerPosition, currentSceneName, journalPages);
         }
         else
         {
            Debug.LogError("SaveLoadManager instance not found.");
         }
      }
      else
      {
         Debug.LogError("Player references not set in SaveLoadDataController.");
      }
   }

   public void LoadGameButton()
   {
      //tells the singleton to start load sequence
      SaveLoadManager.Instance.LoadGame();
   }
}
