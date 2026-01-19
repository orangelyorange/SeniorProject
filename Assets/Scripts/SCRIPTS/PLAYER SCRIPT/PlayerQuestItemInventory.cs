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
}
