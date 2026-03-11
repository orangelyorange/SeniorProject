using System;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class JoySkill : MonoBehaviour
{
   private Player player;
   private Animator animator;

   [Header("Joy Skill Setup")] 
   public int extraJumpValue = 1; // how many extra jumps the player gets
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
   
   //called by SkillManager when the player activates the joy skill
   public void ToggleJoy()
   {
      player.isSkillActive = !player.isSkillActive;

      if (player.isSkillActive)
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
      //Joy Double Jump Logic
      if (Input.GetKeyDown(KeyCode.Space))
      {
         if (player.isMidAir && player.isSkillActive && !player.isSkillUsed)
         {
            player.Jump(); //calls the public jump function from player script
            player.isSkillUsed = true; //prevents the player from using the skill more than once per activation
             Debug.Log("Double Jump Activated");
         }
      }
   }
}
