# Horde AI Design — Enemy Patrol System

This document describes the current enemy horde implementation in `EnemyPatrolAi.cs` and proposes a roadmap for a smarter, more robust horde coordination system.

---

## Current Implementation

Each enemy independently:
1. **Patrols** between two waypoints (`EnemyPointA` ↔ `EnemyPointB`) by setting `Rigidbody2D.linearVelocity` directly in `FixedUpdate`.
2. **Separates** from close neighbors (`CalculateSeparationVelocityX`) so they don't stack.
3. **Coheres** toward nearby group centers (`CalculateGroupCohesionVelocityX`) so they stay loosely packed.
4. **Aligns** velocity with neighbors (`CalculateAlignmentVelocityX`) so the group moves more uniformly.
5. **Anti-jams** when a head-on ally is detected ahead (`IsAllyBlockingAhead`) — the enemy reverses its patrol target back to the opposite point rather than pushing through.

### Inspector fields added (v2)
| Field | Default | Purpose |
|---|---|---|
| `forwardBlockCheckDistance` | 0.6 | How far ahead (units) to look for a blocking ally |
| `forwardBlockRadius` | 0.25 | Width of the forward circle-cast |
| `blockRepathCooldown` | 0.5 s | Minimum time between reversals (prevents flip-flopping) |
| `allyLayerMask` | *(empty)* | Restrict forward check to the Enemy physics layer for accuracy |
| `alignmentStrength` | 0.5 | How much to blend toward neighbor average velocity (0–1) |

---

## Known Limitations of the Current Design

- All enemies share the same two waypoints; there is no per-group or per-lane assignment.
- The anti-jam rule is local and reactive: if two groups meet at the midpoint simultaneously, multiple enemies may reverse at the same time, causing wave-like oscillation.
- Separation/cohesion only operate on the X axis; vertical stacking (stairs, slopes) is not handled.
- `OverlapCircleNonAlloc` with a fixed buffer size of 32 caps the detectable neighbor count; very dense hordes could exceed this.

---

## Smarter Horde AI — Proposed Roadmap

### 1. Leader / Follower Formation (Slot-Based)

**Idea:** Assign one enemy per patrol segment as the *leader*; remaining enemies follow in numbered slots behind it.

- On `Start`, query nearby enemies; the one closest to the patrol target becomes leader (`isLeader = true`).
- Followers target a world position offset from the leader along the patrol axis:  
  `slotTarget = leader.position + slotIndex * (-patrolDirection * slotSpacing)`
- When the leader reverses (reaches a waypoint or is anti-jammed), followers inherit the new direction automatically, preventing accordion bunching.

**Benefits:** Eliminates head-on collisions almost entirely because the whole group turns together. Natural single-file look.

**Cost:** Requires a lightweight leader-election step on spawn or when a leader is destroyed.

---

### 2. Lane / Two-Column Layout

**Idea:** Split enemies into two lanes (lane 0 = left edge, lane 1 = right edge of the patrol corridor).

- Each enemy remembers its `laneIndex` (0 or 1).
- Separation applies a lane offset: target X = `basePath.x + (laneIndex == 0 ? -laneOffset : laneOffset)`.
- Head-on encounters can be handled by passing: lane 0 enemies pass on the left, lane 1 on the right, so they physically can move past each other if collision is disabled.

**Benefits:** Two opposing groups can share a corridor without reversing. More realistic-looking patrol behaviour.

---

### 3. Better Head-On Handling — Yielding with Stagger

Instead of all enemies reversing simultaneously (which causes oscillation):

1. When `IsAllyBlockingAhead` fires, check if the blocking ally is also reversing (`blockingEnemy._lastRepathTime` is recent).
2. If both want to reverse, pick only the one with the lower `GetInstanceID()` to reverse; the other holds position for `blockRepathCooldown` then continues. This staggered yield removes the wave effect.

---

### 4. Steering Weights — Smooth Blending

Replace the current additive sum with a prioritised, clamped blend:

```csharp
float raw = (patrol * patrolWeight)
          + (separation * separationWeight)
          + (cohesion  * cohesionWeight)
          + (alignment * alignmentWeight);

float targetVX = Mathf.Clamp(raw, -maxSpeed, maxSpeed);
```

Expose `patrolWeight`, `separationWeight`, etc. in the Inspector so designers can tune the relative influence of each force without needing to change strength magnitudes.

---

### 5. Performance — Reduce Query Frequency

`OverlapCircleNonAlloc` is called 3–4 times per `FixedUpdate` per enemy. For a 20-enemy horde that is 60–80 physics queries per frame.

**Improvement:** Cache neighbors once per FixedUpdate and share the result:

```csharp
private int _cachedNeighborCount;

void FixedUpdate()
{
    _cachedNeighborCount = Physics2D.OverlapCircleNonAlloc(
        transform.position, awarenessRadius, _neighborBuffer);

    // Pass _cachedNeighborCount to all Calculate... methods
}
```

This reduces queries from O(N × methods) to O(N × 1) per frame — a 3–4× speedup at no cost.

For very large hordes, consider updating the cache every other `FixedUpdate` (`_frameCount % 2 == 0`) — imperceptible to players at 50 Hz physics.

---

### 6. Incremental Implementation Priority

| Priority | Task | Complexity |
|---|---|---|
| ✅ Done | Anti-jam reversal with cooldown | Low |
| ✅ Done | Alignment velocity contribution | Low |
| ✅ Done | NonAlloc buffer (no per-frame GC) | Low |
| Next | Shared neighbor cache (single query per FixedUpdate) | Low |
| Next | Staggered yield (avoid simultaneous reversal) | Medium |
| Future | Leader/follower slots | Medium |
| Future | Two-lane patrol layout | Medium-High |
| Future | Full prioritised steering weights | Medium |

---

## Quick Tuning Reference

| Situation | Remedy |
|---|---|
| Enemies still overlap | Increase `separationStrength` or `separationDistance` |
| Enemies spread too far apart | Increase `groupCohesionStrength` or reduce `preferredGroupSpacing` |
| Enemies oscillate/flip-flop | Increase `blockRepathCooldown` |
| Forward check hits walls/ground | Assign `allyLayerMask` to only the Enemy layer |
| Enemies don't avoid each other | Confirm all enemies share the same `enemyTag` and that tag is correct |
| Horde feels jittery | Lower `separationStrength`, raise `preferredGroupSpacing` dead-zone |
