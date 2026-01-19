using UnityEngine;
using TMPro;

public class NPCDialogueTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    public string[] dialogueLines;

    [Header("UI Hover Settings (Screen Space Overlay)")]
    public RectTransform interactionUI;          // UI to hover (the label)
    public Vector3 worldOffset = new Vector3(0f, 2f, 0f);

    [Header("Prompt Object")]
    public GameObject interactionLabel;          // The "Press F to interact" object

    private bool isPlayerInRange = false;
    private bool isDialogueActive = false;

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
        if (interactionLabel != null && interactionLabel.activeSelf)
        {
            Vector3 screenPos = mainCam.WorldToScreenPoint(transform.position + worldOffset);
            interactionUI.position = screenPos;
        }

        // Handle F key for dialogue
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (!isDialogueActive)
            {
                NPCDialogueManager.Instance.StartDialogue(dialogueLines);
                isDialogueActive = true;
            }
            else
            {
                NPCDialogueManager.Instance.DisplayNextLine();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            interactionLabel.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;

            NPCDialogueManager.Instance.EndDialogue();
            isDialogueActive = false;

            interactionLabel.SetActive(false);
        }
    }
}