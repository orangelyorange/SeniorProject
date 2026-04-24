using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SadnessSkill : MonoBehaviour
{
    private Player player;
    private Animator animator;

    [Header("Sadness Shield Setup")] 
    public GameObject shieldObject;
    public float shieldDuration = 5f;
    public float shieldCooldown = 8f; // How long until the shield can be used again

    [Header("Unlock Requirements")] 
    public int unlockSceneIndex = 4; 

    public bool isSadnessSkillActive = false;
    private bool isShieldActive = false;
    private float nextShieldTime = 0f; // Tracks the cooldown timer

    private void Start()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();

        if (shieldObject != null)
        {
            shieldObject.SetActive(false); 
        }

        if (animator != null)
        {
            animator.SetBool("isSadnessActive", false);
        }
    }

    private void Update()
    {
        // Activate the shield by pressing 'X'
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (isSadnessSkillActive && !isShieldActive && Time.time >= nextShieldTime)
            {
                ExecuteShieldSkill();
            }
            else if (isSadnessSkillActive && Time.time < nextShieldTime)
            {
                // Optional: Tells you exactly how many seconds are left on the cooldown
                Debug.Log($"Shield is on cooldown! {Mathf.Ceil(nextShieldTime - Time.time)}s remaining.");
            }
        }
    }

    public bool SetSadness(bool isActive)
    {
        if (isActive)
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (currentSceneIndex < unlockSceneIndex)
            {
                Debug.Log("Sadness skill locked. Reach level " + unlockSceneIndex + " to unlock.");
                return false; 
            }
        }

        isSadnessSkillActive = isActive;
        if (animator != null)
        {
            animator.SetBool("isSadnessActive", isActive);
        }
        Debug.Log(isActive ? "Sadness skill equipped." : "Sadness skill unequipped.");
        return true;
    }

    private void ExecuteShieldSkill()
    {
        Debug.Log("Activating Sadness Shield.");
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Instance.sadnessActivate);
        }
        
        // Set the timer for when the shield can be used next
        nextShieldTime = Time.time + shieldCooldown; 
        
        StartCoroutine(ShieldRoutine());
    }

    private IEnumerator ShieldRoutine()
    {
        isShieldActive = true;
        if (shieldObject != null) shieldObject.SetActive(true);
        if (player != null) player.isInvulnerable = true;
        
        yield return new WaitForSeconds(shieldDuration);

        if (shieldObject != null) shieldObject.SetActive(false);
        if (player != null) player.isInvulnerable = false;
        
        isShieldActive = false;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Instance.sadnessShieldExpire);
        }
        Debug.Log("Shield Deactivated");
    }
}
