using UnityEngine;

namespace SCRIPTS.ITEM_SCRIPT
{
    public class JournalManager : MonoBehaviour
    {
        public static JournalManager Instance;
       
       [Header("UI References")]
       public GameObject journalUIPanel;
       public Transform scrollViewContent; //where entries are spawned
       public GameObject journalEntryPrefab; //visual template for entry

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
       }

       private void Update()
       {
           //calls when J is pressed
           if (Input.GetKeyDown(KeyCode.J))
           {
               ToggleJournal();
           }
       }
       
       //calls when Journal item is picked up
       public void AddNewLore(string title, string content)
       {
           //spawns the entry prefab
           GameObject newEntry = Instantiate(journalEntryPrefab, scrollViewContent);
           
           //grabs script on the prefab and fills the text
           
           //shows panel
           journalUIPanel.SetActive(true);
           
           //pauses game when journal panel is active
           Time.timeScale = 0f;
       }

       //Toggles the panel on and off 
       public void ToggleJournal()
       {
           journalUIPanel.SetActive(!journalUIPanel.activeSelf);
           Time.timeScale = journalUIPanel.activeSelf ? 0f : 1f;
       }
    }
}