using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoaderManager : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LoadTargetScene());
    }

    IEnumerator LoadTargetScene()
    {
        string sceneToLoad = PlayerPrefs.GetString("TargetScene");

        // Small delay = fixes build cutscene skipping bug
        yield return new WaitForSeconds(0.5f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}