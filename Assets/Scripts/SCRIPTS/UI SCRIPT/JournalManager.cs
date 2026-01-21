
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JournalManager : MonoBehaviour
{
   public GameObject mainJournal;
   public Button mainJournalButton;
   
   public bool isOpen = false;

   void Start()
   {
      if (mainJournal != null)
      {
         mainJournal.SetActive(false);
      }

      /*if (act1Journal != null)
      {
         act1Journal.SetActive(false);
      }*/
      
   }

   void Update()
   {
      if (Input.GetKeyDown(KeyCode.J))
      {
         //ToggleMainJournalPanel();
         ToggleMainJournal();
      }
   }

   public void ToggleMainJournal()
   {
      if (mainJournal != null)
      {
         mainJournal.SetActive(true);
         isOpen = true;
      }
   }

   /*public void ToggleMainJournalPanel()
   {
      if (mainJournal != null) return;
      
      if (mainJournal.activeSelf)
      {
         mainJournal.SetActive(false);
         isOpen = false;
         if (act1Journal != null)
         {
            act1Journal.SetActive(false);
         }
      }

      else
      {
         if (act1Journal != null)
         {
            act1Journal.SetActive(false);
         }
         
         mainJournal.SetActive(true);
         mainJournal.transform.SetAsLastSibling();
         isOpen = true;
      }
      
      if (act1Journal != null)
      {
         act1Journal.SetActive(!act1Journal.activeSelf);
      }
   }*/
}