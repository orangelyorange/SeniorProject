using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class JournalSystem : MonoBehaviour
{
    public GameObject journalUIPanel; //Panel for journal Panel
    public Text journalText; // text component for display log
    
    public List<string> journalEntries = new List<string>();

    private void Start()
    {
        if (journalUIPanel != null)
        {
            journalUIPanel.SetActive(false); //Hide Journal on start
        }
    }

    void Update()
    {
        //Toggle journal with J Key
        if (Input.GetKeyDown(KeyCode.J))
        {
            ToggleJournal();
        }
    }

    public void AddJournalEntry(string journalEntry)
    {
        journalEntries.Add(journalEntry);
        UpdateJournalText();
    }

    private void UpdateJournalText()
    {
        journalText.text = "";
        for (int i = 0; i < journalEntries.Count; i++)
        {
            journalText.text += journalEntries[i];
        }
    }

    private void ToggleJournal()
    {
        bool isActive = journalUIPanel.activeSelf;
        journalUIPanel.SetActive(!isActive);
    }
}
