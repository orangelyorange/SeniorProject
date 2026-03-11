using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsPanel : MonoBehaviour
{
   public GameObject optionsPanel;
   public GameObject settingsPanel;
   
   public bool isOpen = false;

   void Start()
   {
      if (optionsPanel != null)
      {
         optionsPanel.SetActive(false);
         settingsPanel.SetActive(false);
         isOpen = false;
      }
   }

   void Update()
   {
      if (Input.GetKeyDown(KeyCode.Escape)) //when presses escape, the game pauses
      {
         ToggleOptionsPanel();
      }
   }

   public void ToggleOptionsPanel()
   {
      if (optionsPanel != null)
      {
         //Flips active state
         optionsPanel.SetActive(!optionsPanel.activeSelf);
         isOpen = optionsPanel.activeSelf;
         
         //to pause and unpause time of game
         if (isOpen)
         {
            Time.timeScale = 0; //freezes the game
         }

         else
         {
            Time.timeScale = 1; //unfreezes the game
         }
      }
      
      if (settingsPanel != null){
         settingsPanel.SetActive(false);
      }
   }

   public void SettingsPanel()
   {
      if (settingsPanel != null)
      {
         settingsPanel.SetActive(true);
         optionsPanel.SetActive(false);
      }
   }
   
   //to connect to main menu button
   public void LoadMainMenu()
   {
      Time.timeScale = 1;
      //Loads the scene at index 0 or main menu
      SceneManager.LoadScene(0);
   }
}
