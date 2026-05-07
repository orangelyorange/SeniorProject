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
    private Vector3 baseScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        baseScale = transform.localScale;
        baseScale.x = Mathf.Abs(baseScale.x);

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
        {
            Vector3 scale = baseScale;
            transform.localScale = scale;
        }
        else if (moveInput < -MoveThreshold)
        {
            Vector3 scale = baseScale;
            scale.x = -baseScale.x;
            transform.localScale = scale;
        }

        animator.SetBool("isRunning", Mathf.Abs(moveInput) > MoveThreshold);
        animator.SetBool("isJumping", !isGrounded);

      
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

        if (spawn == Vector3.zero)
            spawn = transform.position;

        transform.position = spawn;
        rb.position = spawn;
        rb.linearVelocity = Vector2.zero;

        isDashing = false;
        isSkillUsed = false;
        isInvulnerable = false;

        // ⭐ IMPORTANT: reset animation state immediately
        if (animator != null)
        {
            animator.SetBool("isDead", false);
            animator.Rebind();
            animator.Update(0f);
        }

        Debug.Log("Respawned at checkpoint: " + spawn);
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
