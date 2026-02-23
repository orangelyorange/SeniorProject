using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class QuestData
{
    public string questID; //unique name for specific quest
    public string npcName;
    public string requiredItemName;
    public int requiredAmount;
    public bool questActive;
    public bool questCompleted;
}
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    
    [Header("Quest Database")]
    
    // A list of all quests in the game
    public List<QuestData> allQuests = new List<QuestData>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // The Trigger will call this method and pass the ID and the player's inventory
    public void ProcessQuestInteraction(string questID, PlayerQuestItemInventory inventory)
    {
        // Find the specific quest in our list
        QuestData currentQuest = allQuests.Find(q => q.questID == questID);

        if (currentQuest == null)
        {
            Debug.LogWarning($"QuestManager: Could not find quest with ID {questID}");
            return;
        }

        // Condition A: Quest hasn't started
        if (!currentQuest.questActive && !currentQuest.questCompleted)
        {
            currentQuest.questActive = true;
            Debug.Log($"{currentQuest.npcName}: Hello! Please bring me {currentQuest.requiredAmount} {currentQuest.requiredItemName}(s).");
        }
        // Condition B: Quest is active, check inventory
        else if (currentQuest.questActive && !currentQuest.questCompleted)
        {
            int currentAmount = inventory.GetItemCount(currentQuest.requiredItemName);

            if (currentAmount >= currentQuest.requiredAmount)
            {
                // Take items
                inventory.RemoveItem(currentQuest.requiredItemName, currentQuest.requiredAmount);

                // Update UI Counter
                if (ItemCounterUI.Instance != null)
                {
                    ItemCounterUI.Instance.RemoveFromCounter(currentQuest.requiredItemName, currentQuest.requiredAmount);
                }

                // Complete Quest
                currentQuest.questCompleted = true;
                currentQuest.questActive = false;
                Debug.Log($"{currentQuest.npcName}: Thank you for the {currentQuest.requiredItemName}(s)! Quest Complete.");
            }
            else
            {
                Debug.Log($"{currentQuest.npcName}: You only have {currentAmount} out of {currentQuest.requiredAmount}. Keep looking!");
            }
        }
        // Condition C: Quest already done
        else if (currentQuest.questCompleted)
        {
            Debug.Log($"Quest completed: {currentQuest.questID}");
        }
    }
}
