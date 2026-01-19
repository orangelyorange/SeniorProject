using UnityEngine;


public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 1.5f;
    public float attackRate = 1f;
    public int attackDamage = 1;
    public LayerMask enemyLayer;

    public Transform attackPoint;
    private float nextAttackTime = 0f;

    void Update()
    {
        // To check if Left Mouse button is pressed
        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    
    public void Attack()
    {
        // To detect enemies in range of attack

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        
        // To deal damage

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyHealthSystem>().TakeDamage(attackDamage);
        }


    }
}
   
