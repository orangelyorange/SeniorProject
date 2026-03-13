using UnityEngine;

public class Player : MonoBehaviour
{
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
    
    public bool isDashing = false;
    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        //ignores normal inputs so player can't move or jump while dashing
        if (isDashing) return;
        
        moveInput = Input.GetAxis("Horizontal");

        // Player jump logic
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                Jump();
            }
        }

        // Flip sprite when pressed A or D
        if (moveInput > 0.01f)
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        else if (moveInput < -0.01f)
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);

        // Animation parameters
        animator.SetBool("isRunning", moveInput != 0);
        animator.SetBool("isJumping", !isGrounded);
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

        if (isDashing) return;
        
        // Movement
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Better falling
        rb.gravityScale = rb.linearVelocity.y < 0 ? 3f : 1f;
    }

    public void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }
}
