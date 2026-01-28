using UnityEngine;
using System.Collections;

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
    public bool isSkillActive; // toggled by EmotionSkill
    public bool isSkillUsed;

    private EmotionSkill emotionSkill;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        emotionSkill = GetComponent<EmotionSkill>();
    }

    private void Update()
    {
        // Movement
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Jump
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
            transform.localScale = Vector3.one;
        else if (moveInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        // Gravity adjustment for smoother fall
        if (rb.linearVelocity.y < 0)
            rb.gravityScale = 3f;
        else
            rb.gravityScale = 1f;

        // Animation parameters
        animator.SetBool("isRunning", moveInput != 0);
        animator.SetBool("isJumping", !isGrounded);
        animator.SetBool("isJoyActive", isSkillActive);
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isMidAir = !isGrounded;

        if (isGrounded)
        {
            isSkillUsed = false; // reset double jump on landing
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }
}
