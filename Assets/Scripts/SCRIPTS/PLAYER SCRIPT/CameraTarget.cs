using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    [SerializeField] private Transform player;
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
        if (player == null || !player.CompareTag("Player"))
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
            float facing = Mathf.Sign(player.localScale.x);
            if (!Mathf.Approximately(facing, 0f))
            {
                lastFacing = facing;
            }
        }
    }

    private void LateUpdate()
    {
        if (player == null)
        {
            return;
        }

        float facing = player.localScale.x;
        if (!Mathf.Approximately(facing, 0f))
        {
            lastFacing = Mathf.Sign(facing);
        }

        float targetXOffset = horizontalLookAhead * lastFacing;
        float targetYOffset = 0f;

        bool lookUp = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool lookDown = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        if (lookUp ^ lookDown)
        {
            targetYOffset = lookUp ? verticalLookAmount : -verticalLookAmount;
        }

        currentXOffset = Mathf.Lerp(currentXOffset, targetXOffset, horizontalFollowSpeed * Time.deltaTime);
        currentYOffset = Mathf.Lerp(currentYOffset, targetYOffset, verticalFollowSpeed * Time.deltaTime);

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
