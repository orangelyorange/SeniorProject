using UnityEngine;

public class JumpAttack : MonoBehaviour
{
    public Player player;
    public int jumpDamage = 1; // damage sa jump
    public string enemyTag = "Enemy"; // tag the enemy game object
    public EnemyHealthSystem enemyHealth;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(enemyTag))
        {
            if (rb.linearVelocity.y < 0)
            {
                enemyHealth = collision.gameObject.GetComponent<EnemyHealthSystem>(); // get the enemy health system
                if (enemyHealth != null)
                {
                 enemyHealth.TakeDamage(jumpDamage); //-1 damage
                 rb.linearVelocity = new Vector2(rb.linearVelocity.x, player.jumpForce);
                 enemyHealth.Die();
                }
            }
        }
    }
}