using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
   [Header("UI Panels")]
   public GameObject mainMenuPanel;
   public GameObject optionsPanel;
   public GameObject creditsPanel;
   
   [Header("Audio Settings")]
   public Slider mainVolumeSlider; //reference UI slider main object
   public float fadeDuration = 1f;

   void Start()
   {
      if (mainVolumeSlider!= null)
      {
         mainVolumeSlider.value = AudioListener.volume;
      }
      
      if (optionsPanel != null)
      {
         optionsPanel.SetActive(false);
      }
      
      if (creditsPanel != null)
      {
         creditsPanel.SetActive(false);
      }
   }

   public void StartGame()
   {
      StartCoroutine(FadeOutAndLoad());
   }
   
   //fade out sequence 
   private IEnumerator FadeOutAndLoad(){
      //finds audio source. wala pa atm 
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
   SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
   
   }

   public void OpenOptions()
   {
      mainMenuPanel.SetActive(false);
      optionsPanel.SetActive(true);
      creditsPanel.SetActive(false);
      Debug.Log("Options menu opened");
   }

   public void CloseOptions()
   {
      mainMenuPanel.SetActive(true);
      optionsPanel.SetActive(false);
      creditsPanel.SetActive(false);
      Debug.Log("Options menu closed");
   }

   public void QuitGame()
   {
      Debug.Log("Quit game");
      Application.Quit();
   }

   public void CreditsPanel()
   {
      mainMenuPanel.SetActive(false);
      optionsPanel.SetActive(false);
      creditsPanel.SetActive(true);
      }
   
   public void SetVolume(float volume)
   {
      // AudioListener.volume controls the master volume (0.0 to 1.0)
      AudioListener.volume = volume; 
   }
}
