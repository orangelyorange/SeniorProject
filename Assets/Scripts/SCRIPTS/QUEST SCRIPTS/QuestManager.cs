using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    
    //Quest Status
    [Header("Quest Status")]
    public bool questActive = false; //variable for activating quest items
    public bool questCompleted = false; //variable for completing quest items
    
    //Quest Requirements
    [Header("Quest Requirements")]
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
        
        // count the number of items the player currently has
        int currentAmount = playerInventory.GetItemCount(questItemName);
        
        Debug.Log($"Quest Progress: {currentAmount} {questItemName}s");

        if (currentAmount >= targetAmount)
        {
            CompleteQuest();
            return true; //Tells QuestSystem that deliver is successful
        }
        return false; //Tells QuestSystem they don't have enough items
    }

    public void CompleteQuest()
    {
        //remove items from player inventory
        playerInventory.RemoveItem(questItemName, targetAmount);
        
        // update flags
        questCompleted = true;
        questActive = false;
        Debug.Log("Quest completed. All items delivered!");

    }
}
