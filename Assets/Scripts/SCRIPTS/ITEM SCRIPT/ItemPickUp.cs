using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public string itemId = "Rice Stalk"; //For item
    public int amount = 1;  
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerQuestItemInventory inventory = other.GetComponent<PlayerQuestItemInventory>();

            if (inventory != null)
            {
                inventory.AddItem(itemId);
                QuestManager.Instance.QuestProgress(); //para sa quest system
                ItemCounterUI.Instance.AddToCounter(itemId, amount); //para maadd sa counter
                
                Destroy(gameObject);
            }
            
        }
    }
}