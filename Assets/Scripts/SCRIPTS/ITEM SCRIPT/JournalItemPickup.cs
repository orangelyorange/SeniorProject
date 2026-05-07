using SCRIPTS.ITEM_SCRIPT;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JournalItemPickup : MonoBehaviour
{
    [Header("Journal Item Pickup Data")]
    public string collectibleItemName = "Journal Item";
    public int amount = 1;

    [Header("Lore Data")]
    public JournalPageData pageData;
    public string pageTitle = "Level 1";

    [TextArea(3, 10)] // Gives a larger text box in the inspector
    public string pageContent = "Lore content";

    [Header("Lore Organization (Fallback)")]
    public JournalTab fallbackAct = JournalTab.Act1;
    public string fallbackLevelName;
    public int fallbackDisplayOrder;
    public string fallbackPageId;
    
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
                JournalManager.Instance.AddNewLore(CreateCollectedPage());
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

    private JournalCollectedPage CreateCollectedPage()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (pageData != null)
        {
            return pageData.ToCollectedPage(sceneName);
        }

        string fallbackId = string.IsNullOrWhiteSpace(fallbackPageId)
            ? $"{sceneName}_{SanitizeIdSegment(gameObject.name)}_{SanitizeIdSegment(pageTitle)}"
            : fallbackPageId;

        return new JournalCollectedPage
        {
            pageId = fallbackId,
            title = pageTitle,
            body = pageContent,
            act = fallbackAct,
            levelName = string.IsNullOrWhiteSpace(fallbackLevelName) ? sceneName : fallbackLevelName,
            displayOrder = fallbackDisplayOrder
        };
    }

    private static string SanitizeIdSegment(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "unknown";
        }

        char[] chars = value.ToCharArray();
        for (int index = 0; index < chars.Length; index++)
        {
            char current = chars[index];
            if (!char.IsLetterOrDigit(current) && current != '-' && current != '_')
            {
                chars[index] = '_';
            }
        }

        return new string(chars);
    }
}
