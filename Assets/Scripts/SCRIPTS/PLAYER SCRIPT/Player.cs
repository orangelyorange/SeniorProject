using UnityEngine;

public class Player : MonoBehaviour
{
    private const float MoveThreshold = 0.01f;

    [Header("Player Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator animator;

    [Header("Skill States")]
    public bool isGrounded;
    public bool isMidAir;
    public bool isSkillActive; // set by EmotionSkill
    public bool isSkillUsed;

    public bool isInvulnerable = false; //for the sadness shield
    
    public bool isDashing = false; //for rage skill
    private float moveInput;
    private bool isRunLoopPlaying;
    private bool isVerticalLookActive;
    private bool wasVerticalLookActive;
    private Vector2 verticalLookAnchor;
    private Collider2D verticalLookGroundCollider;
    private Vector3 verticalLookGroundLocalOffset;
    private RigidbodyConstraints2D baseConstraints;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        baseConstraints = rb.constraints;
        animator = GetComponent<Animator>();
        RestoreCheckpointPosition();
        PlayPendingRespawnSfx();
    }

    void Update()
    {
        //ignores normal inputs so player can't move or jump while dashing
        if (isDashing)
        {
            isVerticalLookActive = false;
            wasVerticalLookActive = false;
            StopRunningSfx();
            return;
        }

        isVerticalLookActive = IsVerticalLookActive();
        if (isVerticalLookActive && !wasVerticalLookActive)
        {
            verticalLookGroundCollider = null;
            if (groundCheck != null)
            {
                Collider2D groundCollider = Physics2D.OverlapCircle(
                    groundCheck.position,
                    groundCheckRadius,
                    groundLayer
                );
                if (groundCollider != null)
                {
                    verticalLookGroundCollider = groundCollider;
                    verticalLookGroundLocalOffset = groundCollider.transform.InverseTransformPoint(rb.position);
                }
            }
            if (verticalLookGroundCollider == null)
            {
                verticalLookAnchor = rb.position;
            }
        }
        else if (!isVerticalLookActive && wasVerticalLookActive)
        {
            verticalLookGroundCollider = null;
        }
        wasVerticalLookActive = isVerticalLookActive;
        moveInput = isVerticalLookActive ? 0f : Input.GetAxis("Horizontal");

        // Player jump logic
        if (!isVerticalLookActive && Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                Jump();
            }
        }

        // Flip sprite when pressed A or D
        if (moveInput > MoveThreshold)
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        else if (moveInput < -MoveThreshold)
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);

        // Animation parameters
        animator.SetBool("isRunning", moveInput != 0);
        animator.SetBool("isJumping", !isGrounded);

        HandleRunningSfx();
    }

    void FixedUpdate()
    { 
        // Ground check
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        isMidAir = !isGrounded;
        
        //resets double jump skill when touching the ground
        if (isGrounded)
            isSkillUsed = false;

        if (isDashing)
        {
            ReleaseVerticalLookFreeze();
            return;
        }

        if (isVerticalLookActive)
        {
            ApplyVerticalLookFreeze();
            return;
        }
        ReleaseVerticalLookFreeze();
        
        // Movement
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Better falling
        rb.gravityScale = rb.linearVelocity.y < 0 ? 3f : 1f;
    }

    private bool IsVerticalLookActive()
    {
        bool lookUp = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool lookDown = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        return (lookUp || lookDown) && lookUp != lookDown;
    }

    private void ApplyVerticalLookFreeze()
    {
        rb.constraints = baseConstraints | RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        if (verticalLookGroundCollider != null)
        {
            rb.position = verticalLookGroundCollider.transform.TransformPoint(verticalLookGroundLocalOffset);
        }
        else
        {
            rb.position = verticalLookAnchor;
        }
    }

    private void ReleaseVerticalLookFreeze()
    {
        if (rb.constraints != baseConstraints)
        {
            rb.constraints = baseConstraints;
        }
    }

    public void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Instance.playerJump);
        }
    }

    private void HandleRunningSfx()
    {
        bool shouldPlayRunSfx = Mathf.Abs(moveInput) > MoveThreshold && isGrounded && !isDashing;

        if (shouldPlayRunSfx && !isRunLoopPlaying)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayLoopingSfx(AudioManager.Instance.playerRun);
            }
            isRunLoopPlaying = true;
        }
        else if (!shouldPlayRunSfx && isRunLoopPlaying)
        {
            StopRunningSfx();
        }
    }

    private void StopRunningSfx()
    {
        if (!isRunLoopPlaying)
        {
            return;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopLoopingSfx(AudioManager.Instance.playerRun);
        }
        isRunLoopPlaying = false;
    }

    private void OnDisable()
    {
        StopRunningSfx();
    }

    private void RestoreCheckpointPosition()
    {
        if (!Checkpoint.TryGetCheckpointForCurrentScene(out Vector3 checkpointPosition))
        {
            return;
        }

        transform.position = checkpointPosition;
        if (rb != null)
        {
            rb.position = checkpointPosition;
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void PlayPendingRespawnSfx()
    {
        if (!HealthSystem.ConsumeRespawnSfxRequest())
        {
            return;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Instance.playerRespawn);
        }
    }
}
