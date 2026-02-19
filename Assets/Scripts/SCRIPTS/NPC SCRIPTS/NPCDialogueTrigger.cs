using UnityEngine;
using TMPro;

public class NPCDialogueTrigger : MonoBehaviour
{
    //For Hover ng Label
    public RectTransform interactionUI;// UI to hover (the label)
    public Vector3 worldOffset = new Vector3(0f, 2f, 0f);

    [Header("Prompt Object")]
    public GameObject interactionLabel; // The "Press F to interact" object

    public bool isPlayerInRange = false; //detect if player is in range of npc

    private Camera mainCam; 

    void Start()
    {
        mainCam = Camera.main;

        if (interactionLabel != null)
            interactionLabel.SetActive(false);
    }

    void Update()
    {
        // Make UI hover above NPC when active
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
            if (interactionLabel != null) interactionLabel.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;

            NPCDialogueManager.Instance.EndDialogue(); //Ensure dialogue box closes if player runs away mid-convo

           if (interactionLabel != null) interactionLabel.SetActive(false);
        }
    }
}