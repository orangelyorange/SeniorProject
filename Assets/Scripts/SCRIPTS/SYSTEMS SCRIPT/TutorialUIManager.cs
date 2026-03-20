using UnityEngine;
using TMPro;

public class TutorialUIManager : MonoBehaviour
{
   //allows us to set the text of the tutorial UI from other scripts
   public static TutorialUIManager instance;
   
   [Header("Tutorial UI References")]
   public GameObject tutorialPanel;
   public TextMeshProUGUI tutorialText;

   private void Awake()
   {
      //sets up the singleton pattern for this manager
      if (instance == null)
      {
         instance = this;
      }
      else
      {
         Destroy(gameObject);
      }
      
      //hides the panel at the beginning of the game
      tutorialPanel.SetActive(false);
   }
   
   //will be called by trigger script to show a message on the tutorial UI
   public void ShowMessage(string message)
   {
      tutorialText.text = message;
      tutorialPanel.SetActive(true);
   }
   
   //will be called by trigger script to hide the tutorial UI
   public void HideMessage()
   {
      tutorialPanel.SetActive(false);
   }
}
