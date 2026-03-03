using UnityEngine;

//Attach to items
public class QuestItemPickUpScript : MonoBehaviour
{
   [Header("Item Details")]
   public string itemName = "Rice Stalk";
   public int amount = 1;
   public string itemDescription = "";

   [Header("Quest Settings")] 
   public bool updateUI = true; //update the UI item counter

   private void OnTriggerEnter2D(Collider2D other)
   {
      //Check if the object collides with player
      if (other.CompareTag("Player"))
      {
         //Find the inventory on the player
         PlayerQuestItemInventory inventory = other.GetComponent<PlayerQuestItemInventory>();
         
         if (inventory != null)
         {
            // Add item using the updated inventory system
            inventory.AddItem(itemName, amount, itemDescription);
            // update in UI counter
            if (updateUI && ItemCounterUI.Instance != null)
            {
               ItemCounterUI.Instance.AddToCounter(itemName, amount);
            }
            //destroy item after picked up
           Destroy(gameObject);
         }
      }
   }
}
