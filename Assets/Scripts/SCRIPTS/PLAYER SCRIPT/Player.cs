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

    public bool isGrounded;
    public bool isMidAir;
    public bool isSkillActive;
    public bool isSkillUsed;
    public bool isInvulnerable = false;
    public bool isDashing = false;

    private float moveInput;
    private bool isRunLoopPlaying;

    private bool hasAppliedSpawn = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        ApplyCheckpointOnce();
        PlayPendingRespawnSfx();
    }

    void Update()
    {
        if (isDashing)
        {
            StopRunningSfx();
            return;
        }

        moveInput = Input.GetAxis("Horizontal");

        if (!isDashing && Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
                Jump();
        }

        if (moveInput > MoveThreshold)
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        else if (moveInput < -MoveThreshold)
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);

        animator.SetBool("isRunning", moveInput != 0);
        animator.SetBool("isJumping", !isGrounded);

        HandleRunningSfx();
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        isMidAir = !isGrounded;

        if (isGrounded)
            isSkillUsed = false;

        if (isDashing)
            return;

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        rb.gravityScale = rb.linearVelocity.y < 0 ? 3f : 1f;
    }

    public void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySfx(AudioManager.Instance.playerJump);
    }

    private void HandleRunningSfx()
    {
        bool shouldPlayRunSfx =
            Mathf.Abs(moveInput) > MoveThreshold &&
            isGrounded &&
            !isDashing;

        if (shouldPlayRunSfx && !isRunLoopPlaying)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayLoopingSfx(AudioManager.Instance.playerRun);

            isRunLoopPlaying = true;
        }
        else if (!shouldPlayRunSfx && isRunLoopPlaying)
        {
            StopRunningSfx();
        }
    }

    private void StopRunningSfx()
    {
        if (!isRunLoopPlaying) return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.StopLoopingSfx(AudioManager.Instance.playerRun);

        isRunLoopPlaying = false;
    }

    private void OnDisable()
    {
        StopRunningSfx();
    }

    // ✅ FIXED: SAFE SPAWN (NO BAD ZERO SPAWN, NO DOUBLE APPLY)
    private void ApplyCheckpointOnce()
    {
        if (hasAppliedSpawn) return;

        Vector3 spawn = Checkpoint.GetSpawnPoint();

        // 🔥 safety check (prevents spawning at weird origin)
        if (spawn == Vector3.zero)
        {
            hasAppliedSpawn = true;
            return;
        }

        transform.position = spawn;

        if (rb != null)
        {
            rb.position = spawn;
            rb.linearVelocity = Vector2.zero;
        }

        hasAppliedSpawn = true;
    }

    private void PlayPendingRespawnSfx()
    {
        if (!HealthSystem.ConsumeRespawnSfxRequest())
            return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySfx(AudioManager.Instance.playerRespawn);
    }
}