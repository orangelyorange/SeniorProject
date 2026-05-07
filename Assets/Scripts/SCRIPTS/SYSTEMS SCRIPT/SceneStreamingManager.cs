using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStreamingManager : MonoBehaviour
{
    public static SceneStreamingManager Instance;

    private HashSet<string> loadedScenes = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadZone(string sceneName)
    {
        if (!loadedScenes.Contains(sceneName))
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }
    }

    public void UnloadZone(string sceneName)
    {
        if (loadedScenes.Contains(sceneName))
        {
            StartCoroutine(UnloadSceneAsync(sceneName));
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!op.isDone)
            yield return null;

        loadedScenes.Add(sceneName);
    }

    private IEnumerator UnloadSceneAsync(string sceneName)
    {
        AsyncOperation op = SceneManager.UnloadSceneAsync(sceneName);

        while (!op.isDone)
            yield return null;

        loadedScenes.Remove(sceneName);
    }
}