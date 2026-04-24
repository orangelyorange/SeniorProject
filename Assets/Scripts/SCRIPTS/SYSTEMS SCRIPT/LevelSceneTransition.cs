using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelSceneTransition : MonoBehaviour
{
   public string playerTag = "Player"; // Tag to identify the player
   public float musicFadeDuration = 0.5f;
   private bool levelComplated = false; // Flag to prevent multiple triggers

   private void OnTriggerEnter2D(Collider2D levelCollision)
   {
      //checks if player triggered the exit and has not completed the level yet
      if (levelCollision.CompareTag(playerTag) && !levelComplated)
      {
         levelComplated = true; //prevents loading the scene multiple times
         StartCoroutine(LoadNextSceneRoutine());
      }
   }

   private IEnumerator LoadNextSceneRoutine()
   {
      if (AudioManager.Instance != null)
      {
         yield return AudioManager.Instance.FadeOutMusic(musicFadeDuration);
      }

      int currentScene = SceneManager.GetActiveScene().buildIndex;
      int nextScene = currentScene + 1; //calculates the next scene index
      
      //checks if the next scene index is within the valid range of scenes
      if (nextScene < SceneManager.sceneCountInBuildSettings)
      {
         Debug.Log("Loading next scene...");
         SceneManager.LoadScene(nextScene);
      }
      else
      {
         SceneManager.LoadScene(0);
      }
   }
}
