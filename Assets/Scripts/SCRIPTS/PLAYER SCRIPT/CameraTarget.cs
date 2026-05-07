using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    private static readonly string[] CameraBoundsObjectNames = { "CameraBounds", "CameraBoundaries" };

    [SerializeField] private Transform player;

    [Header("Horizontal Look Ahead")]
    [SerializeField] private bool enableHorizontalLookAhead = true;
    [SerializeField] private float horizontalLookAhead = 1.5f;
    [SerializeField] private float horizontalFollowSpeed = 6f;

    [Header("Vertical Look")]
    [SerializeField] private bool followPlayerY = true;
    [SerializeField] private float verticalLookAmount = 3.5f;
    [SerializeField] private float verticalFollowSpeed = 3f;

    [Header("Bounds")]
    [SerializeField] private bool enableTargetBoundsClamp = true;
    [SerializeField] private Collider2D cameraBounds;

    private float currentXOffset;
    private float currentYOffset;
    private float lockedBaseY;
    private bool hasLockedBaseY;
    private float lastFacing = 1f;
    private Camera cam;

    void Awake()
    {
        cam = Camera.main;

        if (player == null)
        {
            player = PlayerLocator.GetPlayerTransform();
        }

        ResolveCameraBoundsReference();

        if (player != null)
        {
            transform.position = player.position;
            lockedBaseY = player.position.y;
            hasLockedBaseY = true;

            float facing = Mathf.Sign(player.localScale.x);
            if (!Mathf.Approximately(facing, 0f))
                lastFacing = facing;
        }
        else
        {
            lockedBaseY = transform.position.y;
            hasLockedBaseY = true;
        }
    }

    void LateUpdate()
    {
        if (player == null)
        {
            player = PlayerLocator.GetPlayerTransform();
            if (player == null) return;
        }

        if (cameraBounds == null)
            ResolveCameraBoundsReference();

        if (!hasLockedBaseY)
        {
            lockedBaseY = player.position.y;
            hasLockedBaseY = true;
        }

        // ---------- HORIZONTAL LOOK AHEAD ----------
        float targetXOffset = 0f;

        if (enableHorizontalLookAhead)
        {
            float facing = player.localScale.x;
            if (!Mathf.Approximately(facing, 0f))
                lastFacing = Mathf.Sign(facing);

            targetXOffset = horizontalLookAhead * lastFacing;
        }

        // ---------- VERTICAL LOOK ----------
        float targetYOffset = 0f;

        bool lookUp = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool lookDown = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

        if ((lookUp || lookDown) && lookUp != lookDown)
            targetYOffset = lookUp ? verticalLookAmount : -verticalLookAmount;

        // Smooth follow (exponential smoothing)
        float horizontalT = 1f - Mathf.Exp(-horizontalFollowSpeed * Time.deltaTime);
        float verticalT = 1f - Mathf.Exp(-verticalFollowSpeed * Time.deltaTime);

        currentXOffset = enableHorizontalLookAhead
            ? Mathf.Lerp(currentXOffset, targetXOffset, horizontalT)
            : 0f;

        currentYOffset = Mathf.Lerp(currentYOffset, targetYOffset, verticalT);

        float baseY = followPlayerY ? player.position.y : lockedBaseY;
        Vector3 targetPosition = new Vector3(player.position.x + currentXOffset, baseY + currentYOffset, player.position.z);

        // ---------- CAMERA BOUNDS FIX ----------
        if (enableTargetBoundsClamp && cameraBounds != null && cam != null)
        {
            Bounds bounds = cameraBounds.bounds;

            float camHeight = cam.orthographicSize;
            float camWidth = camHeight * cam.aspect;

            targetPosition.x = Mathf.Clamp(
                targetPosition.x,
                bounds.min.x + camWidth,
                bounds.max.x - camWidth);

            targetPosition.y = Mathf.Clamp(
                targetPosition.y,
                bounds.min.y + camHeight,
                bounds.max.y - camHeight);
        }

        // ---------- KEEP CAMERA Z FIXED ----------
        targetPosition.z = transform.position.z;

        transform.position = targetPosition;
    }

    private void ResolveCameraBoundsReference()
    {
        if (cameraBounds != null)
            return;

        for (int i = 0; i < CameraBoundsObjectNames.Length; i++)
        {
            GameObject boundsObject = GameObject.Find(CameraBoundsObjectNames[i]);
            if (boundsObject == null)
                continue;

            cameraBounds = boundsObject.GetComponent<Collider2D>();
            if (cameraBounds != null)
                return;
        }
    }
}
