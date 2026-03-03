using UnityEngine;

// This ensures you don't accidentally forget to attach the trigger script!
[RequireComponent(typeof(NPCDialogueTrigger))] 
public class QuestSystem : MonoBehaviour
{
    [Header("Quest Connection")]
    [Tooltip("This MUST match the ID in the QuestManager list!")]
    public string questID = "FarmerQuest"; 

    [Header("Dialogues")]
    public string[] questStartDialogue;
    public string[] questReminderDialogue;
    public string[] questSuccessDialogue;
    public string[] afterCompletionDialogue;

    public bool isTalking = false;
    
    private NPCDialogueTrigger dialogueTrigger; 
    
    private void Start()
    {
        dialogueTrigger = GetComponent<NPCDialogueTrigger>();
    }

    void Update()
    {
        // Only allow pressing 'F' if the dialogueTrigger says the player is close enough
        if (dialogueTrigger != null && dialogueTrigger.isPlayerInRange)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!isTalking)
                {
                    HandleInteraction();
                    isTalking = true;
                }
                else
                {
                    // If already talking, just show the next line
                    NPCDialogueManager.Instance.DisplayNextLine();
                }
            }
        }
    }

    void HandleInteraction()
    {
        // 1. Find THIS specific quest from the central Manager
        QuestData currentQuest = QuestManager.Instance.allQuests.Find(q => q.questID == questID);

        if (currentQuest == null)
        {
            Debug.LogError($"[QuestSystem] Could not find quest '{questID}' in QuestManager!");
            return;
        }

        // 2. Grab the player's inventory from the trigger script
        PlayerQuestItemInventory inventory = dialogueTrigger.playerInventory;

        // Condition A: Quest has not started
        if (!currentQuest.questActive && !currentQuest.questCompleted)
        {
            NPCDialogueManager.Instance.StartDialogue(questStartDialogue);
            currentQuest.questActive = true; // Mark it as started!
        }
        
        // Condition B: Quest is in progress
        else if (currentQuest.questActive && !currentQuest.questCompleted)
        {
            int currentAmount = inventory.GetItemCount(currentQuest.requiredItemName);

            // Check if player has sufficient items
            if (currentAmount >= currentQuest.requiredAmount)
            {
                // Take items
                inventory.RemoveItem(currentQuest.requiredItemName, currentQuest.requiredAmount);

                // Update UI Counter
                if (ItemCounterUI.Instance != null)
                {
                    ItemCounterUI.Instance.RemoveFromCounter(currentQuest.requiredItemName, currentQuest.requiredAmount);
                }

                // Complete quest
                currentQuest.questCompleted = true;
                currentQuest.questActive = false;

                NPCDialogueManager.Instance.StartDialogue(questSuccessDialogue);
            }
            else
            {
                // Reminder Sequence if Player does not have enough items
                NPCDialogueManager.Instance.StartDialogue(questReminderDialogue);
            }
        }
        
        // Condition C: Quest is completely finished
        else if (currentQuest.questCompleted)
        {
            // NEW: Using the array you created for after completion!
            NPCDialogueManager.Instance.StartDialogue(afterCompletionDialogue); 
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Reset talking state when they walk away
            isTalking = false; 
        }
    }
}