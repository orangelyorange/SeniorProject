# Hollow Knight-Style Camera Setup

This repository uses a Cinemachine virtual camera (`PlayerCamera`) with a `CameraTarget` helper to achieve Hollow Knight-style camera movement.

## Scene Setup (per scene)
1. **Add/verify CameraTarget**
   - Place `Assets/Prefabs/CameraTarget.prefab` in the scene.
   - Ensure its `CameraTarget` component references the scene’s Player transform (tagged **Player**).
   - If you leave `player` empty, it auto-finds the Player by tag on play.

2. **Wire PlayerCamera to CameraTarget**
   - Select the `PlayerCamera` virtual camera in the scene.
   - Set **Tracking Target** to the `CameraTarget` transform.
   - The composer settings (dead zone, damping, lookahead) should already be set via the prefab. If the scene overrides them, apply prefab values.

3. **Add room bounds (recommended)**
   - Create a `CameraBounds` GameObject with a **PolygonCollider2D** or **CompositeCollider2D**.
   - Set the collider to **Is Trigger** and shape it to the playable room.
   - Add a **CinemachineConfiner2D** extension to `PlayerCamera` and assign the collider to **Bounding Shape 2D**.
   - (Optional) Assign the same `CameraBounds` collider to the `CameraTarget` component to clamp its target position.

## Controls
- **Look up:** `W` or `Up Arrow`
- **Look down:** `S` or `Down Arrow`

## Tuning
Adjust on the `CameraTarget` component:
- `horizontalLookAhead` (default ~1.5)
- `verticalLookAmount` (default ~3.5)
- `horizontalFollowSpeed` / `verticalFollowSpeed` for responsiveness

Adjust on the `PlayerCamera` Positional Composer:
- **Dead Zone** size (default `0.1 x 0.1`)
- **Damping** (default `x: 0.5`, `y: 0.3`)
- **Lookahead** (default `Time: 0.2`, `Smoothing: 5`)

## Troubleshooting
- Camera not moving: confirm `PlayerCamera` Tracking Target points at `CameraTarget`.
- Look controls not working: ensure input is not blocked by UI and the Player is active in the scene.
- Camera shows outside the room: verify `CameraBounds` collider is assigned to the Confiner and encloses the playable area.
