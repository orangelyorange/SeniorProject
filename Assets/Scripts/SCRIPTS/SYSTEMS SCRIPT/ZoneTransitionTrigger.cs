using UnityEngine;

public class ZoneTransitionTrigger : MonoBehaviour
{
    [Header("Scenes")]
    public string sceneToLoad;
    public string sceneToUnload;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (SceneStreamingManager.Instance == null)
        {
            Debug.LogError("SceneStreamingManager is missing in Main Scene!");
            return;
        }

        SceneStreamingManager.Instance.LoadZone(sceneToLoad);
        SceneStreamingManager.Instance.UnloadZone(sceneToUnload);
    }
}