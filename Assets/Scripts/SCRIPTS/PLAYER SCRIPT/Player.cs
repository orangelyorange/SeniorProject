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
        // ⭐ FALL DEATH CHECK (THIS WAS MISSING)
        if (transform.position.y < -15f)
        {
            Respawn();
            return;
        }

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

    // ⭐⭐⭐ RUNTIME RESPAWN FUNCTION ⭐⭐⭐
    public void Respawn()
    {
        Vector3 spawn = Checkpoint.GetSpawnPoint();

        // If no checkpoint yet, just respawn where player started
        if (spawn == Vector3.zero)
            return;

        transform.position = spawn;
        rb.position = spawn;
        rb.linearVelocity = Vector2.zero;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySfx(AudioManager.Instance.playerRespawn);

        Debug.Log("Respawned at checkpoint: " + spawn);
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

    // APPLY CHECKPOINT ON SCENE LOAD
    private void ApplyCheckpointOnce()
    {
        if (hasAppliedSpawn) return;

        Vector3 spawn = Checkpoint.GetSpawnPoint();

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