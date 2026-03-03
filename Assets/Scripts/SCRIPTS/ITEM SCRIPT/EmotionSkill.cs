using UnityEngine;
using System.Collections;

public class EmotionSkill : MonoBehaviour
{
    private Player player;
    private Animator animator;
    private Rigidbody2D rb;

    [Header("Joy Skill Setup")]
    public int extraJumpValue = 1;
    public int extraJump = 0;

    [Header("Sadness Skill Setup")] 
    public float downwardSlashForce;
    public GameObject shieldObject;
    public float shieldDuration = 5f;
    private bool isShieldActive = false;
    

    private void Start()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        player.isSkillActive = false; // ensure off at start
        
        //Skill animations are off at start
        if (animator != null)
            animator.SetBool("isJoyActive", false);

        
        if (shieldObject != null)
        {
            shieldObject.SetActive(false); //shield off when game starts
        }
    }

    private void Update()
    {
        // Toggle Joy skill (Z)
        if (Input.GetKeyDown(KeyCode.Z)) 
        {
            player.isSkillActive = !player.isSkillActive;

            if (player.isSkillActive)
            {
                player.isSkillUsed = false;
                Debug.Log("Joy Activated - Double Jump Enabled");
            }
            else
            {
                player.isSkillUsed = true;
                Debug.Log("Joy Deactivated - Double Jump Disabled");
                
            }
            
            //Joy Double Jump Logic
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (player.isMidAir && player.isSkillActive && !player.isSkillUsed)
                {
                    player.Jump(); //calls the public jump function from Player.cs
                    player.isSkillUsed = true; //So that double jump won't be used forever
                    Debug.Log("Double jump Activated");
                }
            }
        }
        
        //Triggers sadness skill (x)
        if (Input.GetKeyDown(KeyCode.X))
        {
            //Activates sadness skill
            ActivateSadnessSkill();
        }
    }

    private void ActivateSadnessSkill()
    {
        Debug.Log("Activating Sadness Skill");
        //Downward Attack
        if (rb != null)
        {
            //current vertical velocity is set to zero
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, downwardSlashForce);
            // applies downward force
            rb.AddForce(Vector2.down * downwardSlashForce, ForceMode2D.Impulse);
        }
        //Trigger animation for sadness skill
        if (animator != null) 
        {
            animator.SetTrigger("SadnessAttack");
         }

        if (!isShieldActive)
        {
            StartCoroutine(Shield());
        }
    }

    private IEnumerator Shield()
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