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
    private const float SelfHitTolerance = 0.05f;

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
    public float skyYPosition = 12f; // how high the falling rock spawns

    [Header("Ally Awareness Settings")]
    public float allyAwarenessRadius = 2f;
    public float allySpacingRadius = 1f;
    public float allyLineOfFireBlockRadius = 0.25f;
    public float blockedAttackRetryDelay = 0.25f;
    public LayerMask allyLayerMask;
    public string allyTag = "Enemy";

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
                if (PerformAttack())
                {
                    timer = 0f; // Reset timer after attacking
                }
                else
                {
                    // Keep allies coordinated by scheduling a short delayed retry when blocked by ally-awareness rules.
                    timer = Mathf.Max(0f, attackCooldown - blockedAttackRetryDelay);
                }
            }
        }
        else
        {
            // Optional: If you want the enemy to be ready to attack the exact moment the 
            // player steps back into range, keep the timer primed.
            timer = attackCooldown; 
        }
    }

    private bool PerformAttack()
    {
        // Check our dropdown to see which attack to use!
        if (attackStyle == EnemyAttackStyle.ShootProjectile)
        {
            return ShootStandardProjectile();
        }

        if (attackStyle == EnemyAttackStyle.SpawnGeyserUnderground)
        {
            return SpawnUndergroundGeyser();
        }

        if (attackStyle == EnemyAttackStyle.SpawnFallingRock)
        {
            return SpawnFallingRock();
        }

        return false;
    }

    private bool ShootStandardProjectile()
    {
        if (attackPrefab == null || attackSpawnPoint == null) return false;

        Vector2 toPlayer = player.transform.position - attackSpawnPoint.position;
        float distanceToPlayer = toPlayer.magnitude;
        if (distanceToPlayer <= 0f) return false;
        Vector2 direction = toPlayer / distanceToPlayer;

        if (IsAllyInLineOfFire(attackSpawnPoint.position, direction, distanceToPlayer))
        {
            return false;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Instance.enemyShoot);
        }

        // Spawn bullet
        GameObject newBullet = Instantiate(attackPrefab, attackSpawnPoint.position, Quaternion.identity);
        
        // Calculate angle to player and rotate bullet
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        newBullet.transform.rotation = Quaternion.Euler(0, 0, angle);
        return true;
    }

    private bool SpawnUndergroundGeyser()
    {
        if (attackPrefab == null) return false;
        if (IsAllyTooCloseToTargetArea(player.transform.position)) return false;

        // Create a new position: Player's exact X, but our deep underground Y
        Vector2 spawnPos = new Vector2(player.transform.position.x, undergroundYPosition);

        // Spawn the geyser pointing straight up
        Instantiate(attackPrefab, spawnPos, Quaternion.identity);
        return true;
    }

    private bool SpawnFallingRock()
    {
        if (attackPrefab == null) return false;
        if (IsAllyTooCloseToTargetArea(player.transform.position)) return false;

        Vector2 spawnPos = new Vector2(player.transform.position.x, skyYPosition);
        Instantiate(attackPrefab, spawnPos, Quaternion.identity);
        return true;
    }

    private bool IsAllyInLineOfFire(Vector2 start, Vector2 direction, float distance)
    {
        if (allyLineOfFireBlockRadius <= 0f || distance <= 0f) return false;

        RaycastHit2D[] hits;
        if (allyLayerMask.value == 0)
        {
            hits = Physics2D.CircleCastAll(start, allyLineOfFireBlockRadius, direction, distance);
        }
        else
        {
            hits = Physics2D.CircleCastAll(start, allyLineOfFireBlockRadius, direction, distance, allyLayerMask);
        }

        foreach (RaycastHit2D hit in hits)
        {
            Collider2D hitCollider = hit.collider;
            if (hitCollider == null) continue;
            if (hitCollider.attachedRigidbody != null && hitCollider.attachedRigidbody.gameObject == gameObject) continue;
            if (!hitCollider.CompareTag(allyTag)) continue;
            if (Vector2.Distance(start, hitCollider.ClosestPoint(start)) <= SelfHitTolerance) continue;

            return true;
        }

        return false;
    }

    private bool IsAllyTooCloseToTargetArea(Vector2 targetPosition)
    {
        if (allyAwarenessRadius <= 0f || allySpacingRadius <= 0f) return false;

        // Two-step filtering:
        // 1) Ally must already be in the target zone (allySpacingRadius around targetPosition),
        // 2) and also be part of this enemy's local awareness neighborhood (allyAwarenessRadius).
        Collider2D[] nearbyAllies;
        if (allyLayerMask.value == 0)
        {
            nearbyAllies = Physics2D.OverlapCircleAll(targetPosition, allySpacingRadius);
        }
        else
        {
            nearbyAllies = Physics2D.OverlapCircleAll(targetPosition, allySpacingRadius, allyLayerMask);
        }

        foreach (Collider2D ally in nearbyAllies)
        {
            if (ally == null) continue;
            if (!ally.CompareTag(allyTag)) continue;
            if (ally.attachedRigidbody != null && ally.attachedRigidbody.gameObject == gameObject) continue;

            float allyDistance = Vector2.Distance(transform.position, ally.transform.position);
            if (allyDistance <= allyAwarenessRadius)
            {
                return true;
            }
        }

        return false;
    }

    // Draws a helpful yellow circle in the Unity editor to show the detection range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, allyAwarenessRadius);
    }
}
