using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private bool enableHorizontalLookAhead = false;
    [SerializeField] private float horizontalLookAhead = 1.5f;
    [SerializeField] private float horizontalFollowSpeed = 6f;
    [SerializeField] private float verticalLookAmount = 3.5f;
    [SerializeField] private float verticalFollowSpeed = 3f;
    [SerializeField] private Collider2D cameraBounds;

    private float currentXOffset;
    private float currentYOffset;
    private float lastFacing = 1f;

    private void Awake()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (cameraBounds == null)
        {
            GameObject boundsObject = GameObject.Find("CameraBounds");
            if (boundsObject != null)
            {
                cameraBounds = boundsObject.GetComponent<Collider2D>();
            }
        }

        if (player != null)
        {
            transform.position = player.position;
            if (enableHorizontalLookAhead)
            {
                float facing = Mathf.Sign(player.localScale.x);
                if (!Mathf.Approximately(facing, 0f))
                {
                    lastFacing = facing;
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (player == null)
        {
            return;
        }

        float targetXOffset = 0f;
        if (enableHorizontalLookAhead)
        {
            float facing = player.localScale.x;
            if (!Mathf.Approximately(facing, 0f))
            {
                lastFacing = Mathf.Sign(facing);
            }
            targetXOffset = horizontalLookAhead * lastFacing;
        }
        float targetYOffset = 0f;

        bool lookUp = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool lookDown = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        if (lookUp ^ lookDown)
        {
            targetYOffset = lookUp ? verticalLookAmount : -verticalLookAmount;
        }

        float horizontalT = 1f - Mathf.Exp(-horizontalFollowSpeed * Time.deltaTime);
        float verticalT = 1f - Mathf.Exp(-verticalFollowSpeed * Time.deltaTime);
        currentXOffset = enableHorizontalLookAhead
            ? Mathf.Lerp(currentXOffset, targetXOffset, horizontalT)
            : 0f;
        currentYOffset = Mathf.Lerp(currentYOffset, targetYOffset, verticalT);

        Vector3 targetPosition = player.position + new Vector3(currentXOffset, currentYOffset, 0f);
        if (cameraBounds != null)
        {
            Bounds bounds = cameraBounds.bounds;
            targetPosition.x = Mathf.Clamp(targetPosition.x, bounds.min.x, bounds.max.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, bounds.min.y, bounds.max.y);
        }

        targetPosition.z = player.position.z;
        transform.position = targetPosition;
    }
}
