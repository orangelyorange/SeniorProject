using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RageSkill : MonoBehaviour
{
    private Player player;
    private Animator animator;
    private Rigidbody2D rb;

    [Header("Rage Skill State")] 
    public bool isRageEquipped = false;
    public bool isUnlocked = false; // Added to track if the skill is unlocked yet

    [Header("Rage Skill Setup")] 
    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    public float skillCooldown = 10f;
    
    [Header("Unlock requirements")]
    public int unlockSceneIndex = 3;
    
    private float lastSkillUsedTime = -Mathf.Infinity;
    private bool isDashing = false;
    
    private void Start()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (animator != null)
        {
            animator.SetBool("isRageActive", false);
        } 
        
        // Check if the skill should be unlocked when the scene loads
        CheckUnlockStatus();
    }

    private void Update()
    {
        // Check if unlocked, equipped, NOT currently dashing, and cooldown has passed
        if (isUnlocked && isRageEquipped && !isDashing && Time.time >= lastSkillUsedTime + skillCooldown)
        {
            // Assuming "C" is the key you want to press to activate the skill
            if (Input.GetKeyDown(KeyCode.C)) 
            {
                StartCoroutine(PerformDash());
            }
        }
    }

    // New method to handle the scene checking logic
    private void CheckUnlockStatus()
    {
        // Gets the build index of the currently active scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        
        // Unlocks if we are on or past the required scene
        if (currentSceneIndex >= unlockSceneIndex)
        {
            isUnlocked = true;
        }
    }

    private IEnumerator PerformDash()
    {
        isDashing = true;
        player.isDashing = true; // Tells the player script that we are dashing
        
        lastSkillUsedTime = Time.time; // Resets cooldown timer
        
        // Triggers animation
        if (animator != null)
        {
            animator.SetBool("isRageActive", true);
        }
        
        // Turns off gravity while dashing
        float originalGravity = 0f;
        if (rb != null)
        {
            originalGravity = rb.gravityScale;
            rb.gravityScale = 0f;
        }
        
        // NOTE: Mathf.Sign(transform.position.x) gets the sign of your world coordinate, 
        // not where the player is looking. See my notes below!
        float facingX = Mathf.Sign(transform.localScale.x); // Fixed this to use localScale
        Vector2 dashDirection = new Vector2(facingX, 0f);
        
        float startTime = Time.time;
        
        // Apply velocity for the duration of the dash
        while (Time.time < startTime + dashDuration)
        {
            if (rb != null)
            {
                rb.linearVelocity = dashDirection * dashSpeed;
            }
            yield return null; // Wait for next frame
        }
        
        // Clean up when the dash ends
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Stops momentum
            rb.gravityScale = originalGravity; // Restores gravity
        }
        
        if (animator != null)
        {
            animator.SetBool("isRageActive", false);
        }
        
        player.isDashing = false; // Gives control back to the player script
        isDashing = false;
    }
    
    public void SetRage(bool isRage)
    {
        isRageEquipped = isRage;
    }
}