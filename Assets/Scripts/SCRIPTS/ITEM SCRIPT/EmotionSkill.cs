using UnityEngine;
using System.Collections;

public class EmotionSkill : MonoBehaviour
{
    private Player player;

    
    public int extraJumpValue = 1; 
    public int extraJump = 0;

    private void Start()
    {
        
        player = GetComponent<Player>();

        if (player == null)
        {
            Debug.LogError("Player script not found! Make sure both scripts are on the same object.");
        }

        // Initialize state
        extraJump = 0;
        player.isSkillActive = false; // Start with skills disabled
    }

    private void Update()
    {
        // Toggle Joy skill with "Z" key
        if (Input.GetKeyDown(KeyCode.Z)) 
        {
            
            player.isSkillActive = !player.isSkillActive;

            if (player.isSkillActive)
            {
                // Enable skill ability
                player.isSkillUsed = false; // Reset usage so they can jump immediately
                Debug.Log("Joy Activated - Double Jump Enabled");
            }
            else
            {
                // Disable ability
                Debug.Log("Joy Deactivated - Double Jump Disabled");
            }
        }
    }
}