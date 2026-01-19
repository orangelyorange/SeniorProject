using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCDialogueManager : MonoBehaviour
{
    public static NPCDialogueManager Instance;

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    private string[] lines;
    private int currentLineIndex = 0;
    private bool isDialogueActive = false;

    private void Awake()
    {
        // Simple singleton pattern
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        HideDialogue();
    }

    public void StartDialogue(string[] dialogueLines)
    {
        if (dialogueLines.Length == 0) return;

        lines = dialogueLines;
        currentLineIndex = 0;
        isDialogueActive = true;

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
