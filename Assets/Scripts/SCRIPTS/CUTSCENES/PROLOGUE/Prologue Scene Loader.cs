using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using System.Collections;
using UnityEngine.UI;

public class Prologue_SceneLoader : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "LEVEL1";
    [SerializeField] private PlayableDirector playableDirector;
    
    [Header("Transition Settings")]
    [SerializeField]private Image fadeImage; // Reference to a UI Image for fade effect
    [SerializeField] private float transitionDuration = 2f;

    private void Start()
    {
        // Ensure the fade image starts completely transparent (invisible) and turned off
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 0);
            fadeImage.gameObject.SetActive(false);
        }
        
        if (playableDirector == null)
        {
            playableDirector = GetComponent<PlayableDirector>();
        }
        
        if (playableDirector == null)
        {
            playableDirector = FindFirstObjectByType<PlayableDirector>();
        }

        if (playableDirector != null)
        {
            playableDirector.stopped += OnDirectorStopped;
        }
        else
        {
            StartCoroutine(FadeOutAndLoad());
        }
    }

    private void OnDirectorStopped(PlayableDirector director)
    {
        LoadNextLevel();
    }

    private IEnumerator FadeOutAndLoad()
    {
        if (fadeImage != null)
        {
            // Turn the image on before we start fading it
            fadeImage.gameObject.SetActive(true);
            
            float elapsedTime = 0f;
            Color startColor = fadeImage.color;
            Color targetColor = new Color(0, 0, 0, 1f); // Solid black

            // Gradually change the alpha from 0 to 1 over the duration
            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                fadeImage.color = Color.Lerp(startColor, targetColor, elapsedTime / transitionDuration);
                yield return null; // Wait until the next frame before continuing
            }

            // Ensure it's perfectly black at the end
            fadeImage.color = targetColor;
        }
        else
        {
            // Fallback: If you forgot to assign the image, just wait the duration
            yield return new WaitForSeconds(transitionDuration);
        }

        LoadNextLevel();
    }
    

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
    }

    private void OnDisable()
    {
        if (playableDirector != null)
        {
            playableDirector.stopped -= OnDirectorStopped;
        }
    }
}
