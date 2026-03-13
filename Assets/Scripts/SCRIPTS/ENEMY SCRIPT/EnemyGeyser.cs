using UnityEngine;

public class EnemyGeyser : MonoBehaviour
{
    [Header("Geyser Settings")]
    public int damage = 1;
    public float speed = 2f;        // Make this faster for a quick eruption!
    public float lifeTime = 3f;      // How long it lasts

    [Header("Targeting")]
    public string targetTag = "Player"; 

    void Start()
    {
        // Set the timer to destroy the wave
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Move the wave straight UP every frame
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            HealthSystem playerHealth = other.GetComponent<HealthSystem>();
            
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Player is hit!");
            }
        }
    }
}