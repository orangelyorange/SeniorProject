using System;
using UnityEngine;

public class JoySkill : MonoBehaviour
{
    private Player player;
    private Animator animator;

    [Header("Joy Skill Setup")] 
    public int extraJumpValue = 1; 
    public int extraJump = 0;

    [Header("Joy Cooldown")]
    public float skillCooldown = 5f;
    private float nextSkillTime = -Mathf.Infinity;
    private bool hasSkillBeenUsed = false;

    public bool HasSkillBeenUsed => hasSkillBeenUsed;
    public float CooldownRemaining => Mathf.Max(0f, nextSkillTime - Time.time);
    public float CooldownDuration => skillCooldown;

    private void Start()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.SetBool("isJoyActive", false);
        } 
    }
   
    // Called by Emotion Skill Manager to explicitly turn Joy on or off
    public void SetJoy(bool isActive)
    {
        if (animator != null)
        {
            animator.SetBool("isJoyActive", isActive);
        }

        if (isActive)
        {
            player.isSkillUsed = false;
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySfx(AudioManager.Instance.joyActivate);
            }
            Debug.Log("Joy skill activated - Double Jump enabled");
        }
        else
        {
            player.isSkillUsed = true; // Prevents jumping if deactivated mid-air
            Debug.Log("Joy Deactivated - Double Jump Disabled");
        }
    }

    private void Update()
    {
        // 1. Check if the player is safely on the ground
        if (!player.isMidAir)
        {
            // 2. If the skill is currently active AND they used the double jump
            if (player.isSkillActive && player.isSkillUsed)
            {
                // Auto-turn off the skill locally
                SetJoy(false);
                
                // Force the player's skill state to false (simulating toggling 'Z' off)
                player.isSkillActive = false; 
                
                Debug.Log("Player landed. Joy skill auto-toggled off.");
            }
            
            // 3. Always reset the usage tracker when on the ground so they can use it again later
            player.isSkillUsed = false;
        }
        
        // Double Jump Logic
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (player.isMidAir && player.isSkillActive && !player.isSkillUsed && Time.time >= nextSkillTime)
            {
                player.Jump(); 
                player.isSkillUsed = true; // Marks that the double jump was consumed
                hasSkillBeenUsed = true;
                nextSkillTime = Time.time + skillCooldown;
                Debug.Log("Double Jump Activated");
            }
        }
    }
}
