using UnityEngine;

[RequireComponent(typeof(EmotionSkillManager))]
public class PlayerAttack : MonoBehaviour
{
    [Header("Standard Attack")]
    public float attackRange = 1.5f;
    public float attackRate = 1f;
    public int baseDamage = 1;
    public int emotionDamage = 2; 
    public LayerMask enemyLayer;
    public Transform attackPoint;
    
    [Header("Downward Slash (Plunge)")]
    public float downwardSlashForce = 15f;
    public float plungeAoERadius = 2.5f;
    private bool isPerformingDownwardSlash = false;

    private float nextAttackTime = 0f;
    
    // Component References
    private EmotionSkillManager skillManager;
    private Player player;
    private Rigidbody2D rb;
	private Animator animator;

    void Start()
    {
        skillManager = GetComponent<EmotionSkillManager>();
        player = GetComponent<Player>();      // Needed to check if the player is in mid-air
        rb = GetComponent<Rigidbody2D>();     // Needed to apply the slam force

		animator = GetComponent<Animator>();     // Needed to trigger attack animations
    }

    void Update()
    {
        // 1. Handle Input for Attacking
        if (Input.GetMouseButtonDown(0))
        {
            // If in mid-air, trigger the downward slash!
            if (player.isMidAir && !isPerformingDownwardSlash)
            {
                ExecuteDownwardSlash();
            }
            // If on the ground and cooldown is ready, trigger normal attack!
            else if (!player.isMidAir && Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }

        // 2. Check if we just hit the ground while plunging
        if (isPerformingDownwardSlash && !player.isMidAir)
        {
            LandAndDealAoEDamage();
        }
    }

    public void Attack()
    {
        // Get enemies in front of the player
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        
        // Route them to our damage calculator
        DealDamageToEnemies(hitEnemies);
    }

    private void ExecuteDownwardSlash()
    {
        if (rb != null)
        {
            // Halt horizontal momentum and force the player straight down
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); 
            rb.AddForce(Vector2.down * downwardSlashForce, ForceMode2D.Impulse);
        }
        
        isPerformingDownwardSlash = true;
    }

    private void LandAndDealAoEDamage()
    {
        isPerformingDownwardSlash = false; // Reset the state
        Debug.Log("Slammed into the ground! Triggering AoE.");

        // Detect all enemies within the blast radius (centered on the player's body)
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, plungeAoERadius, enemyLayer);
        
        // Route them to our damage calculator
        DealDamageToEnemies(hitEnemies);
    }

    // Applies Damage & Emotion Weaknesses 
    private void DealDamageToEnemies(Collider2D[] enemiesToHit)
    {
        foreach (Collider2D enemy in enemiesToHit)
        {
            EnemyHealthSystem enemyHealth = enemy.GetComponent<EnemyHealthSystem>();
            if (enemyHealth != null)
            {
                int damageToDeal = baseDamage; 
                EmotionSkill activeSkill = skillManager.currentSkill;

                // Joy vs Water Ghost
                if (activeSkill == EmotionSkill.Joy && enemyHealth.enemyType == EnemyType.WaterGhost)
                {
                    damageToDeal = emotionDamage;
                    Debug.Log("Joy deals heavy damage to Water Ghost!");
                }
                // Sadness vs Fire Sentinel
                else if (activeSkill == EmotionSkill.Sadness && enemyHealth.enemyType == EnemyType.FireSentinel)
                {
                    damageToDeal = emotionDamage;
                    Debug.Log("Sadness deals heavy damage to Fire Sentinel!");
                }

                // Apply damage
                enemyHealth.TakeDamage(damageToDeal);
            }
        }
    }

    // Visualize the attack ranges in the Unity Scene view
    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            // Red circle for normal attack
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }

        // Cyan circle for Downward Slash AoE
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, plungeAoERadius);
    }
}