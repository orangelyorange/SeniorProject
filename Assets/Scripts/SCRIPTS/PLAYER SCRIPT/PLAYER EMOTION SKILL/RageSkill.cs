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
    }

    private void Update()
    {
        //automatic rage skill dash when c (player skill) is equipped
        if (isRageEquipped)
        {
            StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash()
    {
        isDashing = true;
        player.isDashing = true; //tells the player script that we are dashing, so it can disable other movement inputs
        
        lastSkillUsedTime = Time.time; //resets cooldown timer
        
        //triggers animation
        if (animator != null)
        {
            animator.SetBool("isRageActive", true);
        }
        
        //turns off gravity while dashing
        float originalGravity = 0f;
        if (rb != null)
        {
            originalGravity = rb.gravityScale;
            rb.gravityScale = 0f;
        }
        
        //Determine the dash direction based on player facing
        float facingX = Mathf.Sign(transform.position.x);
        Vector2 dashDirection = new Vector2(facingX, 0f);
        
        float StartTime = Time.time;
        
        //Apply velocity for the duration of the dash
        while (Time.time < StartTime + dashDuration)
        {
            if (rb != null)
            {
                rb.linearVelocity = dashDirection * dashSpeed;
            }
            yield return null; //wait for next frame
        }
        //Clean up when the dash ends
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; //stops momentum
            rb.gravityScale = originalGravity; //restores gravity
        }
        
        if (animator != null)
        {
            animator.SetBool("isRageActive", false);
        }
        
        player.isDashing = false; //gives control back to the player script
        isDashing = false;
    }
    
    public void SetRage(bool isRage)
    {
	if (isActive) 
	{
		int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
		if (currentSceneIndex < unlockSceneIndex)
            {
                Debug.Log("Rage skill locked. Reach level " + unlockSceneIndex + " to unlock.");
                return false; 
			}	
    }
     isRageEquipped = isRage;
}
