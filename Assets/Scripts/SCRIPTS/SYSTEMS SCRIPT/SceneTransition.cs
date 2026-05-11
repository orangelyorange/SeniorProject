using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    void Awake()
    {
        // make this exist in ALL scenes automatically
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

    public void LoadScene(string targetScene)
    {
        Time.timeScale = 1f;
        PlayerPrefs.SetString("TargetScene", targetScene);
        SceneManager.LoadScene("SceneLoader");
    }
}