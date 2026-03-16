using UnityEngine;

// This creates a dropdown menu in the Inspector
public enum EnemyAttackStyle
{
    ShootProjectile,
    SpawnGeyserUnderground,
	SpawnFallingRock
}

public class EnemyAttack : MonoBehaviour
{
    [Header("General Attack Settings")]
    public EnemyAttackStyle attackStyle;     // Choose the attack type from the dropdown
    public GameObject attackPrefab;          // The Bullet OR the Tsunami prefab
    public float detectionRadius = 10f;      // How close the player needs to be
    public float attackCooldown = 2f;        // Time between attacks

    [Header("Level 1 Attack Settings")]
    public Transform attackSpawnPoint;       // Where bullets shoot from

    [Header("Level 2 Attack Settings")]
    public float undergroundYPosition = -6f; // How deep the geyser 

	[Header("Level 3 Attack Settings")]
public float skyYPosition = 12f; //how high the falling rock spawns

    private GameObject player;
    private float timer;

    void Start()
    {
        // Find the player once at the start
        player = GameObject.FindGameObjectWithTag("Player");
        
        // Start the timer at the cooldown max so the enemy attacks immediately when the player enters range
        timer = attackCooldown; 
    }

    void Update()
    {
        // If the player is missing or destroyed, do nothing
        if (player == null) return;

        // Check distance to the player
        float distance = Vector2.Distance(transform.position, player.transform.position);

        // If the player is inside the detection radius, start counting up
        if (distance <= detectionRadius)
        {
            timer += Time.deltaTime;

            if (timer >= attackCooldown)
            {
                PerformAttack();
                timer = 0f; // Reset timer after attacking
            }
        }
        else
        {
            // Optional: If you want the enemy to be ready to attack the exact moment the 
            // player steps back into range, keep the timer primed.
            timer = attackCooldown; 
        }
    }

    private void PerformAttack()
    {
        // Check our dropdown to see which attack to use!
        if (attackStyle == EnemyAttackStyle.ShootProjectile)
        {
            ShootStandardProjectile();
        }
        else if (attackStyle == EnemyAttackStyle.SpawnGeyserUnderground)
        {
            SpawnUndergroundGeyser();
        }
    }

    private void ShootStandardProjectile()
    {
        if (attackPrefab == null || attackSpawnPoint == null) return;

        // Spawn bullet
        GameObject newBullet = Instantiate(attackPrefab, attackSpawnPoint.position, Quaternion.identity);
        
        // Calculate angle to player and rotate bullet
        Vector2 direction = (player.transform.position - attackSpawnPoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        newBullet.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void SpawnUndergroundGeyser()
    {
        if (attackPrefab == null) return;

        // Create a new position: Player's exact X, but our deep underground Y
        Vector2 spawnPos = new Vector2(player.transform.position.x, undergroundYPosition);

        // Spawn the geyser pointing straight up
        Instantiate(attackPrefab, spawnPos, Quaternion.identity);
    }

    // Draws a helpful yellow circle in the Unity editor to show the detection range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}