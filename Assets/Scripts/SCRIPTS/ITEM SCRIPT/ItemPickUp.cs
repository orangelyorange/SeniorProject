using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public string collectibleItemName = "Rice Stalk"; //For item
    public int amount = 1;  
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerQuestItemInventory inventory = other.GetComponent<PlayerQuestItemInventory>();

            if (inventory != null)
            {
                inventory.AddItem(collectibleItemName);
                //QuestManager.Instance.QuestProgress(); para sa quest chuchu
                ItemCounterUI.Instance.AddToCounter(collectibleItemName, amount);
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySfx(AudioManager.Instance.itemPickup);
                }
                Destroy(gameObject);
            }
            
        }
    }
}
