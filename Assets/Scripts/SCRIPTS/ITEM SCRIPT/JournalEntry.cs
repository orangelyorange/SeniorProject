using UnityEngine;
using TMPro;

    public class JournalEntry : MonoBehaviour
    {
        public TextMeshProUGUI journalTitle;
        public TextMeshProUGUI journalContent;
        
        // called by the manager when prefab is spawned
        public void Setup(string newTitle, string newContent)
        {
            journalTitle.text = newTitle;
            journalContent.text = newContent;
        }
    }
