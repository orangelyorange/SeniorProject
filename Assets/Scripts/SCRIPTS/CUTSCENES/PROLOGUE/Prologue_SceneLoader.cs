using UnityEngine;
using UnityEngine.SceneManagement;

public class Prologue_SceneLoader : MonoBehaviour
{
    private void OnEnable()
    {
        //only specifying the sceneName or sceneBuildIndex will load the scene with the single mode
        SceneManager.LoadScene("Level1", LoadSceneMode.Single);
    }
}
