using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Sprite inactiveSprite;
    [SerializeField] private Sprite activeSprite;

    [SerializeField] private string playerTag = "Player";

    private SpriteRenderer sr;

    private static Checkpoint activeCheckpoint;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        SetVisual(false);
    }

    private void Start()
    {
        // restore only inside this scene session
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
        // turn off previous checkpoint
        if (activeCheckpoint != null)
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

    // called by Player when respawning
    public static Vector3 GetSpawnPoint()
    {
        if (activeCheckpoint != null)
            return activeCheckpoint.transform.position;

        return Vector3.zero;
    }
}