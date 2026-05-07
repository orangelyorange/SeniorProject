using UnityEngine;

public class ZoneLoaderTrigger : MonoBehaviour
{
    public string sceneToLoad;
    public string sceneToUnload;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneStreamingManager.Instance.LoadZone(sceneToLoad);

            if (!string.IsNullOrEmpty(sceneToUnload))
                SceneStreamingManager.Instance.UnloadZone(sceneToUnload);
        }
    }
}