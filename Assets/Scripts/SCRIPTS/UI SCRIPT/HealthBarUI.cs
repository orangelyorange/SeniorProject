using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Sprite[] healthSprites; //assign heart sprites
    public HealthSystem playerHealth;  // Reference to your player's health system
    public Image targetImage;          // The single UI Image component to change

    private void Start()
    {
        if (playerHealth == null)
        {
            Debug.LogWarning("HealthBarUI: PlayerHealth not assigned!");
        }

        if (targetImage == null)
        {
            // If not assigned manually, try to find it on this object
            targetImage = GetComponent<Image>();
        }
    }

    private void Update()
    {
        UpdateHealthSprite();
    }

    private void UpdateHealthSprite()
    {
        // Get current health
        int currentHealth = playerHealth.PlayerHealth;
        
        //clamp the index between 0 and the last slot in the array.
        int spriteIndex = Mathf.Clamp(currentHealth, 0, healthSprites.Length - 1);

        // Change the sprite on the single image
        if (targetImage.sprite != healthSprites[spriteIndex])
        {
            targetImage.sprite = healthSprites[spriteIndex];
        }
    }
}