using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;


public class HealthSystem : MonoBehaviour
{
    public int PlayerHealth; //tracks player's health
    public int PlayerMaxHealth = 4; // full health
    private SpriteRenderer spriteRenderer;

	private Animator animator;
	private bool isDead = false; //prevents multiple death triggers

    private void Start()
    {
        PlayerHealth = PlayerMaxHealth; //sets current health to full
        spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage) // tracker of damage player takes
    {
		if (isDead) return; // Prevents taking damage if already dead

         PlayerHealth -= damage;
         StartCoroutine(BlinkRed());
        
            //if Player's HP reaches zero, the game object is destroyed
         if (PlayerHealth <= 0)
         {
             isDead = true;
             StartCoroutine(Die());
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
    

    public IEnumerator Die()
		{
		animator.SetBool("isDead", true); // Trigger death animation

		GetComponent<Player>().enabled = false; // Disable player movement
		GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero; // Stop player movement

		yield return new WaitForSeconds(0.5f); // Wait for the death animation to finish

        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene to restart the game
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
    
