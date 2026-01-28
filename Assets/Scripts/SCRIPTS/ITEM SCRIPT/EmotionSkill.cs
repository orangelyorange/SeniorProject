using UnityEngine;
using System.Collections;

public class EmotionSkill : MonoBehaviour
{
    private Player player;
    private Animator animator;

    // For Double Jump
    public int extraJumpValue = 1;
    public int extraJump = 0;

    private void Start()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();

        extraJump = 0;
        player.isSkillActive = false; // ensure off at start

        if (animator != null)
            animator.SetBool("isJoyActive", false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) // Toggle Joy skill
        {
            player.isSkillActive = !player.isSkillActive;

            if (player.isSkillActive)
            {
                player.isSkillUsed = false;
                Debug.Log("Joy Activated - Double Jump Enabled");

                if (animator != null)
                    animator.SetBool("isJoyActive", true);
            }
            else
            {
                player.isSkillUsed = true;
                Debug.Log("Joy Deactivated - Double Jump Disabled");

                if (animator != null)
                    animator.SetBool("isJoyActive", false);
            }
        }
    }
}