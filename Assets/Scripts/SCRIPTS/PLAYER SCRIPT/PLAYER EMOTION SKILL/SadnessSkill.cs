using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SadnessSkill : MonoBehaviour
{
   private Player player;
   private Animator animator;
   private Rigidbody2D rb;

   [Header("Sadness Skill Setup")] 
   public float downwardSlashForce;
   public GameObject shieldObject;
   public float shieldDuration = 5f;

   [Header("Unlock Requirements")] 
   public int unlockSceneIndex = 2; //defaults to 2 based on current scene setup

   private bool isSadnessActive = false;
   private bool isShieldActive = false;

   private void Start()
   {
      player = GetComponent<Player>();
      animator = GetComponent<Animator>();
      rb = GetComponent<Rigidbody2D>();

      if (shieldObject != null)
      {
         shieldObject.SetActive(false); //shield off when game starts`
      }
   }

   private void Update()
   {
      if (Input.GetMouseButtonDown(0))
      {
         if (isSadnessActive && player.isMidAir && !isShieldActive)
         {
            ExecuteDownwardSlash();
         }
      }
   }

   //Called by Skill Manager to activate Sadness Skill
   public void ToggleSadness()
   {
      //checks if player is in correct level scene (level 2)
      int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
      if (currentSceneIndex < unlockSceneIndex)
      {
         Debug.Log("Sadness skill locked. Reach level " + unlockSceneIndex + " to unlock.");
         return; //to stop the code from running if the player is in the wrong scene
      }
      //Toggles skin on/off
      isSadnessActive = !isSadnessActive;

      if (isSadnessActive)
      {
         Debug.Log("Sadness skill activated.");
      }
      else
      {
         Debug.Log("Sadness skill Unequipped");
      }
   }
   private void ExecuteDownwardSlash()
   {
      if (rb != null)
      {
         rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); //reset vertical velocity
         rb.AddForce(Vector2.down * downwardSlashForce, ForceMode2D.Impulse);
      }
      
      /*
       if (animator != null)
       {
       animator.SetTrigger("SadnessAttack");
       }
       */
      
      StartCoroutine(ShieldRoutine());
   }

   private IEnumerator ShieldRoutine()
   {
      isShieldActive = true;

      if (shieldObject != null)
      {
         shieldObject.SetActive(true);
      }
      player.isInvulnerable = true;
        
      yield return new WaitForSeconds(shieldDuration);

      if (shieldObject != null)
      {
         shieldObject.SetActive(false);
      }
        
      player.isInvulnerable = false;
      isShieldActive = false;
        
      Debug.Log("Shield Deactivated");
   }
}
