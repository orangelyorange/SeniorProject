using SCRIPTS.ITEM_SCRIPT;
using UnityEngine;

public class JournalItemPickup : MonoBehaviour
{
    [Header("Journal Item Pickup Data")]
    public string collectibleItemName = "Journal Item";
    public int amount = 1;

    [Header("Lore Data")]
    public string pageTitle = "Level 1";
    
    [TextArea(3,10)] // Gives a larger text box in the inspector
    public string pageContent = "Lore content";
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Check if the colliding object is the Player
        if (other.CompareTag("Player"))
        {
            // 2. Handle Player Inventory
            PlayerJournalItemInventory inventory = other.GetComponent<PlayerJournalItemInventory>();
            if (inventory != null)
            {
                inventory.AddItem(collectibleItemName);
            }
            else
            {
                Debug.LogWarning("JournalItemPickup: The Player is missing the PlayerJournalItemInventory component!");
            }
      
            // 3. Handle Player Healing
            HealthSystem playerHealthSystem = other.GetComponent<HealthSystem>();
            if (playerHealthSystem != null)
            {
                playerHealthSystem.TakeHealing(1);
                Debug.Log("Player is healing!");
            }

            // 4. Handle Journal UI (Safe check for Singleton)
            if (JournalManager.Instance != null)
            {
                JournalManager.Instance.AddNewLore(pageTitle, pageContent);
            }
            else
            {
                Debug.LogError("JournalItemPickup: JournalManager.Instance was not found in the scene!");
            }

            // 5. Handle Item Counter UI (Safe check for Singleton)
            if (ItemCounterUI.Instance != null)
            {
                ItemCounterUI.Instance.AddToCounter(collectibleItemName, amount);
            }
            else
            {
                Debug.LogError("JournalItemPickup: ItemCounterUI.Instance was not found in the scene!");
            }

            // 6. Destroy the item after everything is processed
            Destroy(gameObject);
        }
    }
}