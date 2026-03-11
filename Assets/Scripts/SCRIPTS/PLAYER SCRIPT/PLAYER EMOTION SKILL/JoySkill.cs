using System;
using UnityEngine;

public class JoySkill : MonoBehaviour
{
    private Player player;
    private Animator animator;

    [Header("Joy Skill Setup")] 
    public int extraJumpValue = 1; 
    public int extraJump = 0;

    private void Start()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.SetBool("isJoyActive", false);
        } 
    }
   
    // Called by NewMonoBehaviourScript to explicitly turn Joy on or off
    public void SetJoy(bool isActive)
    {
        player.isSkillActive = isActive;

        if (isActive)
        {
            player.isSkillUsed = false;
            Debug.Log("Joy skill activated - Double Jump enabled");
        }
        else
        {
            player.isSkillUsed = true;
            Debug.Log("Joy Deactivated - Double Jump Disabled");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (player.isMidAir && player.isSkillActive && !player.isSkillUsed)
            {
                player.Jump(); 
                player.isSkillUsed = true; 
                Debug.Log("Double Jump Activated");
            }
        }
    }
}