using UnityEngine;

public class EnemyPatrolAi : MonoBehaviour
{
    public GameObject EnemyPointA;
    public GameObject EnemyPointB;
    private Rigidbody2D rb;
    private Animator animator;
    private Transform currentPoint;
    public float speed;
    
    [Header("Awareness Spacing")]
    public float awarenessRadius = 1.5f;
    public float separationDistance = 0.8f;
    public float separationStrength = 2f;
    public string enemyTag = "Enemy";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentPoint = EnemyPointB.transform;
        animator.SetBool("isRunning", true);
        
        //Finds the player at the start
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            
        }
    }

    void FixedUpdate()
    {
        if (currentPoint == null || rb == null) return;

        float distanceToPoint = Vector2.Distance(transform.position, currentPoint.position);
        if (distanceToPoint < 0.5f && currentPoint == EnemyPointB.transform)
        {
            Flip();
            currentPoint = EnemyPointA.transform;
        }
        else if (distanceToPoint < 0.5f && currentPoint == EnemyPointA.transform)
        {
            Flip();
            currentPoint = EnemyPointB.transform;
        }

        float patrolDirectionX;
        if (currentPoint == EnemyPointB.transform)
        {
            patrolDirectionX = 1f;
        }
        else
        {
            patrolDirectionX = -1f;
        }

        float separationVelocityX = CalculateSeparationVelocityX();
        float targetVelocityX = (patrolDirectionX * speed) + separationVelocityX;

        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);
    }

    private float CalculateSeparationVelocityX()
    {
        if (awarenessRadius <= 0f || separationDistance <= 0f || separationStrength <= 0f)
        {
            return 0f;
        }

        Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, awarenessRadius);
        float separationForceX = 0f;

        foreach (Collider2D neighbor in neighbors)
        {
            if (neighbor.gameObject == gameObject) continue;
            if (neighbor.attachedRigidbody == rb) continue;
            if (!neighbor.CompareTag(enemyTag)) continue;

            Vector2 offset = (Vector2)transform.position - neighbor.ClosestPoint(transform.position);
            float distance = offset.magnitude;
            if (distance <= 0f || distance > separationDistance) continue;

            float strength = 1f - (distance / separationDistance);
            separationForceX += Mathf.Sign(offset.x) * strength;
        }

        return separationForceX * separationStrength;
    }

    private void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, awarenessRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, separationDistance);
    }
}
