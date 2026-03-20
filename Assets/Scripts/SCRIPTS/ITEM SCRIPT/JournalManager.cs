using TMPro;
using UnityEngine;

namespace SCRIPTS.ITEM_SCRIPT
{
    public class JournalManager : MonoBehaviour
    {
        public static JournalManager Instance;
       
       [Header("Main Journal UI")]
       public GameObject journalUIPanel;
       public Transform scrollViewContent; //where entries are spawned
       public GameObject journalEntryPrefab; //visual template for entry
       
       [Header("Pop-up Journal UI")]
       public GameObject PopUpPanel;
       public TextMeshProUGUI popUpTitle;
       public TextMeshProUGUI popUpContent;

       private void Awake()
       {
           if (Instance == null)
           {
               Instance = this;
           }
           else
           {
               Destroy(gameObject);
           }
           if (journalUIPanel != null) journalUIPanel.SetActive(false);
            
           //panel is hidden when game starts
           if (journalUIPanel != null)
           {
               journalUIPanel.SetActive(false);
           }
           
           if (PopUpPanel != null) PopUpPanel.SetActive(false);
       }

       private void Update()
       {
           //calls when J is pressed
           if (Input.GetKeyDown(KeyCode.J) && (PopUpPanel == null || !PopUpPanel.activeSelf))
           {
               ToggleJournal();
           }
           
           if (PopUpPanel != null && PopUpPanel.activeSelf)
           {
               if (Input.GetMouseButtonDown(0))
               {
                   ClosePopUp();
               }
           }
       }
       
       //calls when Journal item is picked up
       public void AddNewLore(string title, string content)
       {
           //spawns the entry prefab
           GameObject newEntry = Instantiate(journalEntryPrefab, scrollViewContent);
           
           //grabs script on the prefab and fills the text
           JournalEntry entryScript = newEntry.GetComponent<JournalEntry>();
           if (entryScript != null)
           {
               entryScript.Setup(title, content);
           }
           
           //shows pop up to player
           ShowPopUp(title, content);
       }
       
       //handles the turning on of the pop up panel and filling in the text
       private void ShowPopUp(string title, string content)
       {
              if (PopUpPanel != null)
              {
                popUpTitle.text = title;
                popUpContent.text = content;
                PopUpPanel.SetActive(true);
                
                //freezes game so player can read it
                Time.timeScale = 0;
              }
       }

       private void ClosePopUp()
       {
           if (PopUpPanel != null)
           {
                PopUpPanel.SetActive(false);
                
                //resumes game
                if (!journalUIPanel.activeSelf) //only resumes if journal isn't open
                {
                    Time.timeScale = 1;
                }
           }
       }

       //Toggles the panel on and off 
       public void ToggleJournal()
       {
           //flips active state of the panel
           journalUIPanel.SetActive(!journalUIPanel.activeSelf);
           
           //if the panel is now active, pause the game; if it's closed, resume the game
           Time.timeScale = journalUIPanel.activeSelf ? 0f : 1f;
       }
    }
}