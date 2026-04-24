using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool activateOnce = true;

    private bool isActivated;

    private const string CheckpointSceneKey = "CheckpointScene";
    private const string CheckpointXKey = "CheckpointX";
    private const string CheckpointYKey = "CheckpointY";
    private const string CheckpointZKey = "CheckpointZ";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        if (activateOnce && isActivated)
        {
            return;
        }

        SaveCheckpointPosition(transform.position);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Instance.checkpointActivate);
        }

        isActivated = true;
    }

    private static void SaveCheckpointPosition(Vector3 position)
    {
        PlayerPrefs.SetString(CheckpointSceneKey, SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat(CheckpointXKey, position.x);
        PlayerPrefs.SetFloat(CheckpointYKey, position.y);
        PlayerPrefs.SetFloat(CheckpointZKey, position.z);
        PlayerPrefs.Save();
    }

    public static bool TryGetCheckpointForCurrentScene(out Vector3 checkpointPosition)
    {
        checkpointPosition = Vector3.zero;

        string currentScene = SceneManager.GetActiveScene().name;
        if (!PlayerPrefs.HasKey(CheckpointSceneKey))
        {
            return false;
        }

        if (PlayerPrefs.GetString(CheckpointSceneKey) != currentScene)
        {
            return false;
        }

        if (!PlayerPrefs.HasKey(CheckpointXKey) || !PlayerPrefs.HasKey(CheckpointYKey))
        {
            return false;
        }

        float checkpointX = PlayerPrefs.GetFloat(CheckpointXKey);
        float checkpointY = PlayerPrefs.GetFloat(CheckpointYKey);
        float checkpointZ = PlayerPrefs.GetFloat(CheckpointZKey, 0f);
        checkpointPosition = new Vector3(checkpointX, checkpointY, checkpointZ);
        return true;
    }
}
