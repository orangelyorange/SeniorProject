using UnityEngine;
using System.Collections.Generic;

public class ItemDrop : MonoBehaviour
{
    public HealthSystem playerHealth; //para magamit si health system 
	public List<GameObject> inventory = new List<GameObject>(); //for list ng objects
	public bool hasDropped = false; //ensure 1 item is dropped

    void Update()
    {
        // to check if player is dead and item has dropped
		//if (playerHealth != null && playerHealth.IsDead() && !hasDropped)
		{
			DropItem();
			hasDropped = true;		
    	}
	}

	void DropItem()
	{
		if (inventory.Count > 0) 
		{
			GameObject itemToDrop = inventory[0];
			itemToDrop.transform.SetParent(null);
			itemToDrop.transform.position = transform.position;
			itemToDrop.SetActive(true);
		}
	}		


}