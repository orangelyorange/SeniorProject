using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    public static string ActiveCheckpointID;

    private const string SaveKey = "ACTIVE_CHECKPOINT_ID";
    private const string SceneKey = "ACTIVE_CHECKPOINT_SCENE";

    private void Awake()
    {
        ActiveCheckpointID = PlayerPrefs.GetString(SaveKey, "");
    }

    public static void SetCheckpoint(string id)
    {
        ActiveCheckpointID = id;

        PlayerPrefs.SetString(SaveKey, id);
        PlayerPrefs.SetString(SceneKey, SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
    }
}