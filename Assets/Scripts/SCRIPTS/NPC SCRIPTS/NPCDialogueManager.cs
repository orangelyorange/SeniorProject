using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System; // <-- NEW: Required for Actions

public class NPCDialogueManager : MonoBehaviour
{
    public static NPCDialogueManager Instance;

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    private string[] lines;
    private int currentLineIndex = 0;
    private bool isDialogueActive = false;

    // <-- NEW: This stores our instructions for when the dialogue ends
    private Action onDialogueCompleteCallback; 

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        HideDialogue();
    }

    // <-- NEW: Added 'Action onComplete = null' parameter
    public void StartDialogue(string[] dialogueLines, Action onComplete = null) 
    {
        if (dialogueLines.Length == 0) return;

        lines = dialogueLines;
        currentLineIndex = 0;
        isDialogueActive = true;
        
        // <-- NEW: Save the instructions (if any were passed)
        onDialogueCompleteCallback = onComplete; 

        ShowDialogue();
        DisplayCurrentLine();
    }

    public void DisplayNextLine()
    {
        if (!isDialogueActive) return;

        currentLineIndex++;

        if (currentLineIndex < lines.Length)
        {
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    private void DisplayCurrentLine()
    {
        if (dialogueText != null)
        {
            dialogueText.text = lines[currentLineIndex];
        }
        else
        {
            Debug.Log(lines[currentLineIndex]);
        }
    }

    public void EndDialogue()
    {
        HideDialogue();
        isDialogueActive = false;

        // <-- NEW: If we have instructions saved, execute them now, then clear them!
        onDialogueCompleteCallback?.Invoke();
        onDialogueCompleteCallback = null;
    }

    private void ShowDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);
    }

    private void HideDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }
}