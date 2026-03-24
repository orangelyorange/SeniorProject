using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestItem
{
    public string itemName;
    public int quantity;
    public string description;
    
    //Constructor to create new item
    public QuestItem(string name, int amount, string desc = "")
    {
        itemName = name;
        quantity = amount;
        description = desc;
    }
}

public class PlayerQuestItemInventory : MonoBehaviour
{
	public SaveLoadManager saveLoadManager; // Reference to the SaveLoadManager

    //Inventory List
    public List<QuestItem> inventory =  new List<QuestItem>();

    //Store Item PickUp script
    public void AddItem(string itemName, int amountToAdd = 1, string description = "")
    {
        QuestItem existingItem = inventory.Find(item => item.itemName == itemName);

        if (existingItem != null)
        {
            existingItem.quantity += amountToAdd;
        }

        else
        {
            inventory.Add(new QuestItem(itemName, amountToAdd, description));
        }
        
        Debug.Log("Added {amountToAdd} {itemName}(s) to inventory.");
    }

    public int GetItemCount(string itemName)
    {
       QuestItem existingItem = inventory.Find(item => item.itemName == itemName);

       if (existingItem != null)
       {
           return existingItem.quantity;
       }

       return 0;
    }

    public void RemoveItem(string itemName, int amountToRemove)
    {
        QuestItem existingItem = inventory.Find(item => item.itemName == itemName);

        if (existingItem != null)
        {
            existingItem.quantity -= amountToRemove;

            if (existingItem.quantity <= 0)
            {
                inventory.Remove(existingItem);
            }
            Debug.Log("Removed {$amountToRemove} {$itemName}(s) from inventory.");
        }
        else
        {
            Debug.LogWarning($"Could not remove {itemName} because it is not in the inventory.");
        }
    } 
}
