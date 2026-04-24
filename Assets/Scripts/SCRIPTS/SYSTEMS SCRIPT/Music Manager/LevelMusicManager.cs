using UnityEngine;

public class LevelMusicManager : MonoBehaviour
{
    [Header("Level Music")]
    [SerializeField] private AudioClip levelTrack;
    [SerializeField] private bool fadeIn = true;

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(levelTrack, fadeIn);
            return;
        }

        Debug.LogWarning("LevelMusicManager: AudioManager instance is missing in the scene.");
    }
}
