using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
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

    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        moveInput = Input.GetAxis("Horizontal");

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                Jump();
            }
            else if (isMidAir && isSkillActive && !isSkillUsed)
            {
                Jump();
                isSkillUsed = true;
                Debug.Log("Double Jump Activated!");
            }
        }

        // Flip sprite
        if (moveInput > 0.01f)
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        else if (moveInput < -0.01f)
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);

        // Animations
        animator.SetBool("isRunning", moveInput != 0);
        animator.SetBool("isJumping", !isGrounded);
        animator.SetBool("isJoyActive", isSkillActive);
    }

    void FixedUpdate()
    {
        // Movement
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Ground check
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        isMidAir = !isGrounded;

        if (isGrounded)
            isSkillUsed = false;

        // Better falling
        rb.gravityScale = rb.linearVelocity.y < 0 ? 3f : 1f;
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }
}
