using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    
    //Quest Status
    public bool questActive = false; //variable for activating quest items
    public bool questCompleted = false; //variable for completing quest items
    
    //Quest Requirement
    public string questItemName = "Rice Stalk"; //quest items name na hahanapin
    public int targetAmount = 5; //item amount required
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

    //Called by Quest System when you first accept the quest
    public void StartQuest()
    {
        if (questCompleted) return;
        questActive = true;
        Debug.Log($"Quest started: Deliver {targetAmount} {questItemName}s");
    }

    //To check quest progress
    public bool QuestProgress()
    {
        if (!questActive || questCompleted) return false;
        
        // from method in player quest item inventory
        int currentAmount = playerInventory.GetItemCount(questItemName);
        
        Debug.Log($"Quest Progress: {currentAmount} {questItemName}s");

        if (currentAmount >= targetAmount)
        {
            CompleteQuest();
            return true;
        }
        return false;
    }

    public void CompleteQuest()
    {
        //remove items from player inventory
        playerInventory.RemoveItem(questItemName, targetAmount);
        
        // update flags
        questCompleted = true;
        questActive = false;
        Debug.Log("Quest completed. All items delivered!");

        string[] completionDialogue = new string[] //To trigger completion dialogue
        {
            "Thank you, kind soul.",
            "I can finally harvest all the good rice! Praise Gugurang!"
        };
        
        NPCDialogueManager.Instance.StartDialogue(completionDialogue);
    }
}
