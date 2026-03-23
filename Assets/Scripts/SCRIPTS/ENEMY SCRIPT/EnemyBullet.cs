using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float bulletSpeed = 10f; // Renamed from 'force' to 'speed' since it's kinematic
    public int damage = 1;
    public float lifeTime = 10f;    

    void Start()
    {
        // Start the timer to destroy the bullet so it doesn't fly forever
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Move the bullet forward every frame. 
        // Because the spawner rotated the bullet, Vector2.right will point directly at the player!
        transform.Translate(Vector2.right * bulletSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collide)
    {
        // Check if the bullet hit the Player
        if (collide.CompareTag("Player"))
        {
            HealthSystem healthScript = collide.GetComponent<HealthSystem>();
            
            if (healthScript != null)
            {
                healthScript.TakeDamage(damage);
                Debug.Log("Enemy bullet hit the player!");
            }

            // Destroy the bullet after it deals damage
            Destroy(gameObject);
        }

        // Check if the bullet hit a wall or the floor
        if (collide.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // Destroy the bullet so it doesn't pass through walls
            Destroy(gameObject);
        }
    }
}