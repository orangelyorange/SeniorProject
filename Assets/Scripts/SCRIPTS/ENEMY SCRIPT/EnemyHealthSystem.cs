using UnityEngine;
using System.Collections;

// 1. Define the enemy types so they appear in a dropdown list
public enum EnemyTypes 
{ 
    RiceMouse, 
    WaterGhost, 
    FireSentinel 
}

public class EnemyHealthSystem : MonoBehaviour
{
    [Header("Enemy Identity")]
    // 2. This variable lets the Player script know who it is hitting
    public EnemyType enemyType; 

    [Header("Health Settings")]
    public int maxHealth = 3; 
    private int currentHealth;
    
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Set current health to the max health at the start
        currentHealth = maxHealth; 
    }

    public void TakeDamage(int damage)
    {
        // Subtract from currentHealth so we don't lose our maxHealth value
        currentHealth -= damage; 
        
        // Optional: A helpful log to watch the math happen in the Unity Console
        Debug.Log(gameObject.name + " (" + enemyType + ") took " + damage + " damage! Health: " + currentHealth);

        StartCoroutine(BlinkRed());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log(gameObject.name + " defeated!");
        Destroy(gameObject);
    }

    private IEnumerator BlinkRed()
    {
        // It's always good practice to check if the component exists first!
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            spriteRenderer.color = Color.white;
        }
    }
}