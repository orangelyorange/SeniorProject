using System.Collections.Generic;
using UnityEngine;

public class PlayerJournalItemInventory : MonoBehaviour
{
    [Header("Inventory")]
    public List<string> inventory = new List<string>();

    public System.Action OnInventoryChanged;

    public void AddItem(string itemName)
    {
        if (string.IsNullOrWhiteSpace(itemName))
            return;

        inventory.Add(itemName);

        Debug.Log($"Added to inventory: {itemName}");
        Debug.Log("Inventory: " + string.Join(", ", inventory));

        OnInventoryChanged?.Invoke();
    }

    public bool HasItem(string itemName)
    {
        return inventory.Contains(itemName);
    }

    public void RemoveItem(string itemName)
    {
        if (inventory.Remove(itemName))
        {
            OnInventoryChanged?.Invoke();
        }
    }
}