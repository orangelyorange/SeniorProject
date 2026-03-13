using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SadnessSkill : MonoBehaviour
{
    private Player player;
    private Animator animator;
    private Rigidbody2D rb;

    [Header("Sadness Skill Setup")] 
    public float downwardSlashForce = 15f;
    public GameObject shieldObject;
    public float shieldDuration = 5f;

    [Header("AoE Combat Setup")]
    public float aoeRadius = 2.5f;
    public int baseAoEDamage = 1;
    public int emotionAoEDamage = 2; // Damage vs Fire Sentinel
    public LayerMask enemyLayer;

    [Header("Unlock Requirements")] 
    public int unlockSceneIndex = 2; 

    public bool isSadnessSkillActive = false;
    private bool isShieldActive = false;
    private bool isPerformingDownwardSlash = false; // Tracks if we are currently slamming down

    private void Start()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (shieldObject != null)
        {
            shieldObject.SetActive(false); 
        }

        if (animator != null)
        {
            animator.SetBool("isSadnessActive", false);
        }
    }

    private void Update()
    {
        // 1. Initiate the downward slash in mid-air
        if (Input.GetMouseButtonDown(0))
        {
            if (isSadnessSkillActive && player.isMidAir && !isShieldActive && !isPerformingDownwardSlash)
            {
                ExecuteDownwardSlash();
            }
        }

        // 2. Check if we just hit the ground while slashing
        if (isPerformingDownwardSlash && !player.isMidAir)
        {
            LandAndDealAoEDamage();
        }
    }

    public bool SetSadness(bool isActive)
    {
        if (isActive)
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (currentSceneIndex < unlockSceneIndex)
            {
                Debug.Log("Sadness skill locked. Reach level " + unlockSceneIndex + " to unlock.");
                return false; 
            }
        }

        isSadnessSkillActive = isActive;
        if (animator != null)
        {
            animator.SetBool("isSadnessActive", true);
        }
        Debug.Log(isActive ? "Sadness skill activated." : "Sadness skill Unequipped");
        return true;
    }

    private void ExecuteDownwardSlash()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); 
            rb.AddForce(Vector2.down * downwardSlashForce, ForceMode2D.Impulse);
        }
        
        isPerformingDownwardSlash = true; // Mark that we are in the middle of a slam
        StartCoroutine(ShieldRoutine());
    }

    private void LandAndDealAoEDamage()
    {
        isPerformingDownwardSlash = false; // Reset the state
        Debug.Log("Slammed into the ground! Triggering AoE.");

        // Detect all enemies within the blast radius (centered on the player)
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, aoeRadius, enemyLayer);
        
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealthSystem enemyHealth = enemy.GetComponent<EnemyHealthSystem>();
            if (enemyHealth != null)
            {
                int damageToDeal = baseAoEDamage;

                // Apply Sadness weakness logic
                if (enemyHealth.enemyType == EnemyType.FireSentinel)
                {
                    damageToDeal = emotionAoEDamage;
                    Debug.Log("Sadness AoE deals heavy damage to Fire Sentinel!");
                }

                enemyHealth.TakeDamage(damageToDeal);
            }
        }
    }

    private IEnumerator ShieldRoutine()
    {
        isShieldActive = true;
        if (shieldObject != null) shieldObject.SetActive(true);
        player.isInvulnerable = true;
        
        yield return new WaitForSeconds(shieldDuration);

        if (shieldObject != null) shieldObject.SetActive(false);
        player.isInvulnerable = false;
        isShieldActive = false;
        
        Debug.Log("Shield Deactivated");
    }

    // visualize the AoE radius in the Unity Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}