using System.Collections.Generic;
using UnityEngine;

public class PlayerQuestItemInventory : MonoBehaviour
{
    public List<string> inventory =  new List<string>();

    public void AddItem(string itemName)
    {
        inventory.Add(itemName);
        Debug.Log("Added to inventory: " + itemName);
        Debug.Log("Added to inventory: " + string.Join(",", inventory));
    }

    public int GetItemCount(string itemName)
    {
        int count = 0;
        foreach (string item in inventory)
        {
            if (item == itemName)
            {
                count++;
            }
        }
        return count;
    }

    public void RemoveItem(string itemName, int amountToRemove)
    {
        for (int i = 0; i < amountToRemove; i++)
        {
            if (inventory.Contains(itemName))
            {
                inventory.Remove(itemName);
            }
        }
        Debug.Log("Removed from inventory: " + itemName);
    } 
}
