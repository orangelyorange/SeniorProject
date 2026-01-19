using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;


public class HealthSystem : MonoBehaviour
{
    public int PlayerHealth; //tracks player's health
    public int PlayerMaxHealth = 5; // full health
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        PlayerHealth = PlayerMaxHealth; //sets current health to full
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage) // tracker of damage player takes
    {
         PlayerHealth -= damage;
         StartCoroutine(BlinkRed());
        
            //if Player's HP reaches zero, the game object is destroyed
         if (PlayerHealth <= 0)
         {
             Die();
             SceneManager.LoadScene(SceneManager.GetActiveScene().name);
         }
    }

    public void TakeHealing(int healing) //tracker for healing
    {
        if (PlayerHealth >= PlayerMaxHealth)
        {
            PlayerHealth =  PlayerMaxHealth;
            Debug.Log("Player max health is " + PlayerMaxHealth);
            return;
        }
        
        PlayerHealth += healing;
        StartCoroutine(BlinkGreen());

        if (PlayerHealth >= PlayerMaxHealth)
        {
            PlayerHealth = PlayerMaxHealth;
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
    
    public IEnumerator BlinkRed() //player blinks red when taking damage
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.color = Color.white;
    }

    public IEnumerator BlinkGreen() //player blinks green when taking damage
    {
        spriteRenderer.color = Color.green;
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.color = Color.white;
    }
}
    
