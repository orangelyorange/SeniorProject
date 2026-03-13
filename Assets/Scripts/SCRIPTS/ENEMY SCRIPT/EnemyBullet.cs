using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] public float bulletForce;
    public int damage = 1;
    private float timer;

    [SerializeField] private LayerMask groundLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Velocity is set by the spawner (EnemyAttack). This avoids per-bullet FindWithTag calls
        // and prevents conflicting velocity assignments.
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 10)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collide)
    {
        if (collide.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }

        if (collide.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
