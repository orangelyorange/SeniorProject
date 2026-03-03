using UnityEngine;
using TMPro;

public class NPCDialogueTrigger : MonoBehaviour
{
    [Header("UI Hover Settings")]
    public RectTransform interactionUI; 
    public Vector3 worldOffset = new Vector3(0f, 2f, 0f);
    public GameObject interactionLabel; 

    [HideInInspector] public bool isPlayerInRange = false; 
    
    // Made this PUBLIC so QuestSystem can read it!
    [HideInInspector] public PlayerQuestItemInventory playerInventory; 

    private Camera mainCam; 

    void Start()
    {
        mainCam = Camera.main;
        if (interactionLabel != null) interactionLabel.SetActive(false);
    }

    void Update()
    {
        // Only handles hovering the UI now. No input checking!
        if (interactionLabel != null && interactionLabel.activeSelf && interactionUI != null)
        {
            Vector3 screenPos = mainCam.WorldToScreenPoint(transform.position + worldOffset);
            interactionUI.position = screenPos;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerInventory = other.GetComponent<PlayerQuestItemInventory>(); // Store inventory
            if (interactionLabel != null) interactionLabel.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerInventory = null; // Clear inventory

            if (NPCDialogueManager.Instance != null) 
            {
                NPCDialogueManager.Instance.EndDialogue(); 
            }

            if (interactionLabel != null) interactionLabel.SetActive(false);
        }
    }
}