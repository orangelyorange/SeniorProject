using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class Prologue_SceneLoader : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "LEVEL1";
    [SerializeField] private PlayableDirector playableDirector;

    private void Start()
    {
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
            LoadNextLevel();
        }
    }

    private void OnDirectorStopped(PlayableDirector director)
    {
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
