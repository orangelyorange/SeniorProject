using UnityEngine;

[RequireComponent(typeof(EmotionSkillManager))]
public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 1.5f;
    public float attackRate = 1f;
    public int baseDamage = 1;
    public int emotionDamage = 2; 
    public LayerMask enemyLayer;
    public Transform attackPoint;
    
    private float nextAttackTime = 0f;
    private EmotionSkillManager skillManager;

    void Start()
    {
        // Grab the Manager so we can check which skill is currently equipped
        skillManager = GetComponent<EmotionSkillManager>();
    }

    void Update()
    {
        // Attack when Left Click is pressed on the ground 
        // (Your Sadness skill already handles Left Click in mid-air!)
        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    public void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealthSystem enemyHealth = enemy.GetComponent<EnemyHealthSystem>();
            if (enemyHealth != null)
            {
                int damageToDeal = baseDamage; 
                EmotionSkill activeSkill = skillManager.currentSkill;

                // MATCHUP 1: Joy vs Water Ghost
                if (activeSkill == EmotionSkill.Joy && enemyHealth.enemyType == EnemyType.WaterGhost)
                {
                    damageToDeal = emotionDamage;
                    Debug.Log("Joy deals heavy damage to Water Ghost!");
                }
                // MATCHUP 2: Sadness vs Fire Sentinel
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
}