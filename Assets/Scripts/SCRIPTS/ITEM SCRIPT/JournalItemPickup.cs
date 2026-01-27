using UnityEngine;

public class JournalItemPickup : MonoBehaviour
{
    public string collectibleItemName = "Journal Item";
    public int amount = 1;
    public string loreText = "Default lore text yeaur";
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Add to player journal inventory
            PlayerJournalItemInventory inventory = other.GetComponent<PlayerJournalItemInventory>();

            if (inventory != null)
            {
                inventory.AddItem(collectibleItemName);
            }
            
            // Add to journal system
           /* JournalSystem journalSystem = FindObjectOfType<JournalSystem>();
            if (journalSystem != null)
            {
                journalSystem.AddJournalEntry(loreText);
                Debug.Log($"Journal entry added: {collectibleItemName}");
            }
            else
            {
                Debug.LogWarning("No JournalSystem found in scene!");
            }*/
            //Heals the player upon pickup
            HealthSystem playerHealthSystem = other.GetComponent<HealthSystem>();
            if (playerHealthSystem != null)
            {
                playerHealthSystem.TakeHealing(1);
                Debug.Log("Player is healing!");
            }
            //ItemCounterUI.Instance.AddToCounter(collectibleItemName, amount);
            Destroy(gameObject);
        }
        
    }
}
