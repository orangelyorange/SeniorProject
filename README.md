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
- `followPlayerY` (turn off to keep camera Y fixed and avoid diagonal drift on uneven terrain)
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

# Journal System Setup (Unity)

## 1) Open the project
1. Open the project in Unity.
2. Open the scene you want to configure (e.g., `Level0`, `LEVEL1`, `LEVEL2- DRAFT`, or test scenes).

## 2) Add/verify the managers
1. Add **Game Managers 1** prefab to the scene if it is not already present.
2. Select the object with **JournalManager** (usually inside Game Managers).
3. Assign these required fields in **JournalManager**:
   - **journalUIPanel**: the `Journal Panel` root GameObject.
   - **scrollViewContent**: `Scroll View/Viewport/Content`.
   - **journalEntryPrefab**: `Assets/Prefabs/LoreEntryTemplate.prefab`.
   - **PopUpPanel**, **popUpTitle**, **popUpContent**: the popup panel and its title/body text objects.
4. (Optional) Assign:
   - **settingsPanel** / **inventoryPanel** if you have dedicated panels.
   - **act1BookmarkSprite**, **act2BookmarkSprite**, **settingsBookmarkSprite**, **inventoryBookmarkSprite** for tab visuals.
5. The **JournalProgressManager** is auto-created at runtime. No scene setup needed.

## 3) Build the Journal Panel UI
1. Ensure the `Journal Panel` has:
   - A **Scroll View** with `Viewport/Content` for the list.
   - A **text-only page view** with two TMP texts for title/body.
2. If you use the default object names, set them to:
   - Title text object name: **Lore**
   - Body text object name: **Prayer**
   - Otherwise, assign **pageTitleText** and **pageBodyText** in `JournalManager` (or update `pageTitleObjectName` / `pageBodyObjectName`).

## 4) Set up the side tabs
1. Ensure the `Journal Panel` contains a **Bookmark** GameObject (used as a template).
2. The system clones this for **Act 2**, **Settings**, and **Inventory** at runtime.
3. If you need a custom look for Inventory, assign **inventoryBookmarkSprite** in `JournalManager`.

## 5) Create Journal Page assets
1. In the Project window, create assets via:
   - **Create → Journal → Page Data**
2. Fill in:
   - **Id** (unique)
   - **Title**
   - **Body**
   - **Act** (Act1/Act2)
   - **Level Name**
   - **Display Order**

## 6) Configure Journal pickups
1. Add **JournalItemPickup** to your pickup GameObject.
2. Assign the **JournalPageData** asset.
3. (Optional) Set fallback fields if you don’t have a data asset.
4. Ensure the pickup has a trigger collider and the player is tagged **Player**.

## 7) Save/Load persistence
1. Ensure **SaveLoadManager** and **SaveLoadDataController** exist in the scene.
2. Use the Save/Load buttons or call `SaveLoadDataController.SaveGameButton()` / `LoadGameButton()`.
3. Collected pages persist across scenes and loads automatically.

## 8) Verify in play mode
1. Collect a journal page → popup appears and pauses.
2. Press **J** → journal opens.
3. Switch tabs → list updates.
4. Click an entry → title/body show on the right.
