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
