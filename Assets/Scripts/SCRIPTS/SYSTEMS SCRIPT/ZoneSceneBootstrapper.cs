using UnityEngine;
using UnityEngine.SceneManagement;

public static class ZoneSceneBootstrapper
{
    private const string MainSceneName = "LEVEL2_MAIN";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureMainSceneLoaded()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (!IsZoneScene(activeScene))
            return;

        if (SceneManager.GetSceneByName(MainSceneName).isLoaded)
            return;

        if (SceneStreamingManager.Instance != null && PlayerLocator.GetPlayerGameObject() != null)
            return;

        if (Time.timeScale == 0f)
            Time.timeScale = 1f;

        SceneManager.LoadSceneAsync(MainSceneName, LoadSceneMode.Additive);
    }

    private static bool IsZoneScene(Scene scene)
    {
        return scene.IsValid() && scene.name.StartsWith("ZONE_");
    }
}
