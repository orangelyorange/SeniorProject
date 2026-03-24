using UnityEngine;
using TMPro;

public class NPCDialogueTrigger : MonoBehaviour
{
    [Header("UI Hover Settings (Screen Space Overlay)")]
    public RectTransform interactionUI;          // UI to hover (the label)
    public Vector3 worldOffset = new Vector3(0f, 2f, 0f);

    [Header("Prompt Object")]
    public GameObject interactionLabel;          // The "Press F to interact" object

    [HideInInspector] public bool isPlayerInRange = false;
    
    //Made into public so that Quest System can read it
    [HideInInspector] public PlayerQuestItemInventory playerInventory;

    private Camera mainCam;
    private Animator animator;

    void Start()
    {
        mainCam = Camera.main;

        if (interactionLabel != null)
            interactionLabel.SetActive(false);
        
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Only handles hovering of the UI
        if (interactionLabel != null && interactionLabel.activeSelf)
        {
            Vector3 screenPos = mainCam.WorldToScreenPoint(transform.position + worldOffset);
            interactionUI.position = screenPos;
        }
        
        animator.SetBool("isTalking", false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerInventory = other.GetComponent<PlayerQuestItemInventory>();
            if (interactionLabel != null) interactionLabel.SetActive(true);
            animator.SetBool("isTalking", true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerInventory = null; //to clear inventory
            if (NPCDialogueManager.Instance != null)
            {
                NPCDialogueManager.Instance.EndDialogue();
            }
            
            if (interactionLabel != null) interactionLabel.SetActive(false);
            animator.SetBool("isTalking", false);
        }
    }
}