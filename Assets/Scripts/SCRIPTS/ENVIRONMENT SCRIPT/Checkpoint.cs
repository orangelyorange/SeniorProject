using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Sprite inactiveSprite;
    [SerializeField] private Sprite activeSprite;

    [SerializeField] private string playerTag = "Player";

    private SpriteRenderer sr;

    private static Checkpoint activeCheckpoint;
    private static bool hasInitialized = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        SetVisual(false);

        // ⭐ ensure ONLY ONE checkpoint stays active visually on scene start
        if (!hasInitialized)
        {
            activeCheckpoint = null;
            hasInitialized = true;
        }

        if (activeCheckpoint == this)
        {
            SetVisual(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        Activate();
    }

    private void Activate()
    {
        if (activeCheckpoint != null && activeCheckpoint != this)
        {
            activeCheckpoint.SetVisual(false);
        }

        activeCheckpoint = this;
        SetVisual(true);
    }

    private void SetVisual(bool state)
    {
        if (sr == null) return;

        sr.sprite = state ? activeSprite : inactiveSprite;
    }

    // ⭐ GUARANTEED SPAWN ACCESS
    public static Vector3 GetSpawnPoint()
    {
        if (activeCheckpoint != null)
            return activeCheckpoint.transform.position;

        return Vector3.zero;
    }
}