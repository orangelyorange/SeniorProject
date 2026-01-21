using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public bool questActive = false; //variable for activating quest items
    public bool questCompleted = false; //variable for completing quest items
    public List<string> questItems = new List<string> { "ItemA", "ItemB", "ItemC", "ItemD" }; //quest items list

    private PlayerQuestItemInventory playerInventory; //reference player quest item inventory

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        playerInventory = FindObjectOfType<PlayerQuestItemInventory>();
    }

    //Start Quest upon conversing with NPC
    public void StartQuest()
    {
        if (questCompleted) return;
        questCompleted = true;
        Debug.Log("Quest started: Deliver 4 Rice stalks");
    }

    //To check quest progress
    public void QuestProgress()
    {
        if (!questActive || questCompleted) return;
        int collectedCount = 0;
        foreach (string item in questItems)
        {
            if (playerInventory.inventory.Contains(item))
            {
                collectedCount++;
            }
            Debug.Log($"Quest Progress: {collectedCount}/{questItems.Count} items delivered.");
        }

        if (collectedCount == questItems.Count)
        {
            CompleteQuest();
        }
    }

    private void CompleteQuest()
    {
        questCompleted = true;
        questActive = false;
        Debug.Log("Quest completed. All items delivered!");

        string[] completionDialogue = new string[]
        {
            "Thank you, kind soul.",
            "I can finally harvest all the good rice! Praise Gugurang!"
        };
        
        NPCDialogueManager.Instance.StartDialogue(completionDialogue);
    }
}
