using System.Collections.Generic;
using UnityEngine;

// ============================================================
// HordeAI Design Notes (see also Docs/HordeAI.md)
// ------------------------------------------------------------
// Current approach: each enemy independently patrols A<->B
// and uses separation + cohesion + alignment + anti-jam reversal.
//
// Smarter future approach outline:
//   1. Leader/follower slots: one enemy leads, others trail in
//      formation slots behind it.
//   2. Head-on detection (implemented below): enemies that spot
//      an ally coming toward them reverse to their origin patrol
//      point instead of jamming.
//   3. Alignment (implemented below): enemies blend their velocity
//      with neighbors so the horde moves more cohesively.
//   4. NonAlloc queries (implemented below): reuse a pre-allocated
//      buffer to avoid per-frame GC pressure.
// ============================================================

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

    [Header("Horde Pathing / Anti-Jam")]
    [Tooltip("How far ahead (in the current patrol direction) to look for a blocking ally. Set 0 to disable.")]
    public float forwardBlockCheckDistance = 0.6f;
    [Tooltip("Radius of the forward-check circle cast.")]
    public float forwardBlockRadius = 0.25f;
    [Tooltip("Minimum seconds between patrol reversals to prevent rapid flip-flopping.")]
    public float blockRepathCooldown = 0.5f;
    [Tooltip("Layer(s) that count as ally enemies for the anti-jam check. Assign the Enemy layer here for best results; leave empty to fall back to tag-only filtering.")]
    public LayerMask allyLayerMask;

    [Header("Alignment")]
    [Tooltip("How strongly this enemy steers to match the average X velocity of nearby allies (0 = off, 1 = full match each frame).")]
    public float alignmentStrength = 0.5f;

    [Header("Player Flee / Retreat")]
    public bool enablePlayerFlee = false;
    public float fleeTriggerRadius = 1.25f;
    public float fleeSpeed = 6f;
    public float fleeDuration = 0.35f;
    public float fleeCooldown = 0.6f;

    private bool isSteppingBack;

    // Pre-allocated buffer reused every FixedUpdate — avoids per-frame heap allocations from OverlapCircleAll.
    // Unity FixedUpdate runs on the main thread sequentially so sharing this buffer between calls is safe.
    private readonly Collider2D[] _neighborBuffer = new Collider2D[32];

    private float _lastRepathTime = -999f;
    private Transform _player;
    private float _fleeEndTime = -999f;
    private float _lastFleeTime = -999f;
    private float _stepBackEndTime = -999f;
    private float _stepBackDirectionX = 1f;
    private float _stepBackSpeed = 0f;

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
            _player = player.transform;
        }
    }

    void FixedUpdate()
    {
        // Physics-driven movement belongs in FixedUpdate so velocity changes align with Rigidbody2D simulation steps.
        if (currentPoint == null || rb == null) return;

        if (HandleFleeMovement())
        {
            return;
        }

        if (HandleStepBackMovement())
        {
            return;
        }

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

        float patrolDirectionX = (currentPoint == EnemyPointB.transform) ? 1f : -1f;

        // Anti-jam: if an ally is directly blocking our patrol direction, reverse back to the other patrol point.
        if (Time.time - _lastRepathTime >= blockRepathCooldown)
        {
            if (IsAllyBlockingAhead(patrolDirectionX))
            {
                _lastRepathTime = Time.time;
                ReversePatrolTarget();
                patrolDirectionX = (currentPoint == EnemyPointB.transform) ? 1f : -1f;
            }
        }

        float separationVelocityX = CalculateSeparationVelocityX();
        float groupVelocityX = CalculateGroupCohesionVelocityX();
        float alignmentVelocityX = CalculateAlignmentVelocityX();
        float targetVelocityX = (patrolDirectionX * speed) + separationVelocityX + groupVelocityX + alignmentVelocityX;

        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);
    }

    public bool BeginStepBack(Transform target, float distance, float duration)
    {
        if (target == null || distance <= 0f || duration <= 0f) return false;
        if (isSteppingBack || IsFleeTimerActive()) return false;

        _stepBackDirectionX = GetDirectionAwayFrom(target.position.x);
        _stepBackSpeed = distance / duration;
        _stepBackEndTime = Time.time + duration;
        isSteppingBack = true;
        return true;
    }

    public bool IsFleeing()
    {
        return enablePlayerFlee && IsFleeTimerActive();
    }

    /// <summary>
    /// Returns true when an ally enemy is detected directly ahead in the patrol direction,
    /// indicating a head-on jam. Uses a CircleCast so it catches partially-overlapping bodies.
    /// </summary>
    private bool IsAllyBlockingAhead(float patrolDirectionX)
    {
        if (forwardBlockCheckDistance <= 0f || forwardBlockRadius <= 0f) return false;

        Vector2 origin = rb.position;
        Vector2 direction = new Vector2(Mathf.Sign(patrolDirectionX), 0f);

        RaycastHit2D hit = (allyLayerMask.value != 0)
            ? Physics2D.CircleCast(origin, forwardBlockRadius, direction, forwardBlockCheckDistance, allyLayerMask)
            : Physics2D.CircleCast(origin, forwardBlockRadius, direction, forwardBlockCheckDistance);

        if (hit.collider == null) return false;

        // Exclude self
        if (hit.rigidbody != null && hit.rigidbody == rb) return false;

        // Must be an ally enemy (tag-based confirmation works even without a layer mask)
        return hit.collider.CompareTag(enemyTag);
    }

    /// <summary>
    /// Swaps the current patrol target between Point A and Point B, then flips the sprite
    /// so the enemy faces the new direction. Called when an ally jam is detected.
    /// </summary>
    private void ReversePatrolTarget()
    {
        currentPoint = (currentPoint == EnemyPointB.transform)
            ? EnemyPointA.transform
            : EnemyPointB.transform;
        Flip();
    }

    private float CalculateSeparationVelocityX()
    {
        if (awarenessRadius <= 0f || separationDistance <= 0f || separationStrength <= 0f)
        {
            return 0f;
        }

        int count = Physics2D.OverlapCircleNonAlloc(transform.position, awarenessRadius, _neighborBuffer);
        float separationForceX = 0f;

        for (int i = 0; i < count; i++)
        {
            Collider2D neighbor = _neighborBuffer[i];
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
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, awarenessRadius, _neighborBuffer);
        HashSet<int> selectedIds = new HashSet<int>();
        float groupCenterXSum = 0f;
        int usedNeighbors = 0;

        for (int i = 0; i < desiredNeighbors; i++)
        {
            Collider2D bestNeighbor = null;
            float bestDistance = float.MaxValue;

            for (int j = 0; j < count; j++)
            {
                Collider2D neighbor = _neighborBuffer[j];
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

    /// <summary>
    /// Alignment: nudges this enemy's X velocity toward the average X velocity of nearby allies,
    /// so the horde moves more cohesively as one unit.
    /// Returns zero when alignmentStrength is 0 or no neighbors are in range.
    /// </summary>
    private float CalculateAlignmentVelocityX()
    {
        if (alignmentStrength <= 0f || awarenessRadius <= 0f) return 0f;

        int count = Physics2D.OverlapCircleNonAlloc(transform.position, awarenessRadius, _neighborBuffer);
        float avgVelX = 0f;
        int neighborCount = 0;

        for (int i = 0; i < count; i++)
        {
            Collider2D neighbor = _neighborBuffer[i];
            if (neighbor.gameObject == gameObject) continue;
            if (!neighbor.CompareTag(enemyTag)) continue;

            Rigidbody2D neighborRb = neighbor.attachedRigidbody;
            if (neighborRb == null || neighborRb == rb) continue;

            avgVelX += neighborRb.linearVelocity.x;
            neighborCount++;
        }

        if (neighborCount == 0) return 0f;

        avgVelX /= neighborCount;

        // Apply a fraction of the difference between neighbors' average velocity and our own.
        float diff = avgVelX - rb.linearVelocity.x;
        return diff * Mathf.Clamp01(alignmentStrength);
    }

    private bool HandleFleeMovement()
    {
        if (!enablePlayerFlee || _player == null) return false;

        if (!IsFleeTimerActive())
        {
            if (Time.time - _lastFleeTime < fleeCooldown) return false;
            if (fleeTriggerRadius <= 0f || fleeDuration <= 0f) return false;

            float distanceToPlayer = Vector2.Distance(transform.position, _player.position);
            if (distanceToPlayer > fleeTriggerRadius) return false;

            StartFlee(false);
        }

        if (!IsFleeTimerActive()) return false;

        float directionX = GetDirectionAwayFrom(_player.position.x);

        float speedToUse = fleeSpeed > 0f ? fleeSpeed : speed;
        rb.linearVelocity = new Vector2(directionX * speedToUse, rb.linearVelocity.y);
        return true;
    }

    private bool HandleStepBackMovement()
    {
        if (!isSteppingBack) return false;

        if (Time.time >= _stepBackEndTime)
        {
            isSteppingBack = false;
            return false;
        }

        rb.linearVelocity = new Vector2(_stepBackDirectionX * _stepBackSpeed, rb.linearVelocity.y);
        return true;
    }

    private bool IsFleeTimerActive()
    {
        return Time.time < _fleeEndTime;
    }

    private void StartFlee(bool ignoreCooldown)
    {
        if (!ignoreCooldown && Time.time - _lastFleeTime < fleeCooldown) return;

        _lastFleeTime = Time.time;
        _fleeEndTime = Time.time + fleeDuration;
    }

    private void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!enablePlayerFlee) return;
        if (_player == null) return;

        if (collision.collider != null && collision.collider.CompareTag("Player"))
        {
            StartFlee(true);
        }
    }

    private float GetDirectionAwayFrom(float targetX)
    {
        float directionX = Mathf.Sign(transform.position.x - targetX);
        if (Mathf.Approximately(directionX, 0f))
        {
            directionX = transform.localScale.x >= 0f ? 1f : -1f;
        }

        return directionX;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, awarenessRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, separationDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, preferredGroupSpacing);

        // Visualise the forward-block check in the direction the enemy is currently facing
        if (forwardBlockCheckDistance > 0f && forwardBlockRadius > 0f)
        {
            float facingDir = transform.localScale.x >= 0f ? 1f : -1f;
            Vector3 checkCenter = transform.position + new Vector3(facingDir * forwardBlockCheckDistance, 0f, 0f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(checkCenter, forwardBlockRadius);
        }

        if (enablePlayerFlee && fleeTriggerRadius > 0f)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, fleeTriggerRadius);
        }
    }
}
