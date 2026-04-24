using System.Collections.Generic;
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

    [Header("Group Patrol")]
    public int preferredGroupSize = 3;
    public float preferredGroupSpacing = 1.1f;
    public float groupCohesionStrength = 1.25f;

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
        // Physics-driven movement belongs in FixedUpdate so velocity changes align with Rigidbody2D simulation steps.
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
        float groupVelocityX = CalculateGroupCohesionVelocityX();
        float targetVelocityX = (patrolDirectionX * speed) + separationVelocityX + groupVelocityX;

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

    private float CalculateGroupCohesionVelocityX()
    {
        if (awarenessRadius <= 0f || preferredGroupSize <= 1 || groupCohesionStrength <= 0f)
        {
            return 0f;
        }

        int desiredNeighbors = Mathf.Max(preferredGroupSize - 1, 1);
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, awarenessRadius);
        HashSet<int> selectedIds = new HashSet<int>();
        float groupCenterXSum = 0f;
        int usedNeighbors = 0;

        for (int i = 0; i < desiredNeighbors; i++)
        {
            Collider2D bestNeighbor = null;
            float bestDistance = float.MaxValue;

            foreach (Collider2D neighbor in neighbors)
            {
                if (neighbor.gameObject == gameObject) continue;
                if (neighbor.attachedRigidbody == rb) continue;
                if (!neighbor.CompareTag(enemyTag)) continue;

                int neighborId = neighbor.GetInstanceID();
                if (selectedIds.Contains(neighborId)) continue;

                Vector2 offset = neighbor.transform.position - transform.position;
                float distance = offset.magnitude;
                if (distance <= 0f || distance > awarenessRadius) continue;

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestNeighbor = neighbor;
                }
            }

            if (bestNeighbor == null) break;

            selectedIds.Add(bestNeighbor.GetInstanceID());
            groupCenterXSum += bestNeighbor.transform.position.x;
            usedNeighbors++;
        }

        if (usedNeighbors == 0) return 0f;
        float centerX = groupCenterXSum / usedNeighbors;

        float xToGroupCenter = centerX - transform.position.x;
        float desiredBand = preferredGroupSpacing * 0.5f;
        if (Mathf.Abs(xToGroupCenter) <= desiredBand)
        {
            return 0f;
        }

        float adjustedDistance = Mathf.Abs(xToGroupCenter) - desiredBand;
        float normalizedPull = Mathf.Clamp01(adjustedDistance / Mathf.Max(0.01f, awarenessRadius));
        return Mathf.Sign(xToGroupCenter) * normalizedPull * groupCohesionStrength;
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

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, preferredGroupSpacing);
    }
}
