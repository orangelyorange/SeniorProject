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
   public Slider musicVolumeSlider;
   public Slider sfxVolumeSlider;
   public float fadeDuration = 1f;

   void Start()
   {
      if (mainVolumeSlider!= null)
      {
         mainVolumeSlider.value = AudioManager.Instance != null ? AudioManager.Instance.MasterVolume : 1f;
      }

      if (musicVolumeSlider != null)
      {
         musicVolumeSlider.value = AudioManager.Instance != null ? AudioManager.Instance.MusicVolume : 1f;
      }

      if (sfxVolumeSlider != null)
      {
         sfxVolumeSlider.value = AudioManager.Instance != null ? AudioManager.Instance.SfxVolume : 1f;
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
      if (AudioManager.Instance != null)
      {
         yield return AudioManager.Instance.FadeOutMusic(fadeDuration);
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
      if (AudioManager.Instance != null)
      {
         AudioManager.Instance.SetMasterVolume(volume);
      }
   }

   public void SetMusicVolume(float volume)
   {
      if (AudioManager.Instance != null)
      {
         AudioManager.Instance.SetMusicVolume(volume);
      }
   }

   public void SetSfxVolume(float volume)
   {
      if (AudioManager.Instance != null)
      {
         AudioManager.Instance.SetSfxVolume(volume);
      }
   }
}
