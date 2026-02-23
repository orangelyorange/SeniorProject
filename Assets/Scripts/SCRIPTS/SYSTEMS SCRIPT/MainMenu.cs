using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
   [Header("UI Panels")]
   public GameObject mainMenuPanel;
   public GameObject optionsPanel;
   
   [Header("Audio Settings")]
   public Slider mainVolumeSlider; //reference UI slider main object
   public float fadeDuration = 1f;

   void Start()
   {
      if (mainVolumeSlider!= null)
      {
         mainVolumeSlider.value = AudioListener.volume;
      }
   }

   public void StartGame()
   {
      StartCoroutine(FadeOutAndLoad());
   }
   
   //fade out sequence 
   private IEnumerator FadeOutAndLoad(){
      //finds audio source
      AudioSource musicSource = FindObjectOfType<AudioSource>();

      if (musicSource != null)
      {
         float startVolume = musicSource.volume;

         while (musicSource.volume > 0)
         {
            musicSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
         }
         
         musicSource.Stop();
         musicSource.volume = startVolume; //reset in case player returns
      }
   //Loads next scene only while the loop above has finished
   SceneManager.LoadScene("Level 1 Test Mechanics");
      /* add na lang ung next scenes sa build settings*/
   }

   public void OpenOptions()
   {
      mainMenuPanel.SetActive(false);
      optionsPanel.SetActive(true);
      Debug.Log("Options menu opened");
   }

   public void CloseOptions()
   {
      mainMenuPanel.SetActive(true);
      optionsPanel.SetActive(false);
      Debug.Log("Options menu closed");
   }

   public void QuitGame()
   {
      Debug.Log("Quit game");
      Application.Quit();
   }
   
   public void SetVolume(float volume)
   {
      // AudioListener.volume controls the master volume (0.0 to 1.0)
      AudioListener.volume = volume; 
   }
}
