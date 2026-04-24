using UnityEngine;

// This ensures there's an AudioSource attached to the same GameObject
[RequireComponent(typeof(AudioSource))] 
[System.Obsolete("Use AudioManager instead.")]
public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    [SerializeField] public AudioSource audioSource;

    void Awake()
    {
        // Check if an instance of the MusicManager already exists
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject); // Uncomment to keep music playing across scenes
            audioSource = gameObject.GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
            return;
            // Stop running this Awake function if we are destroying the object
        }
    }
    
    
    /// Call this function to start the background music.
    public void PlayMusic()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(audioSource != null ? audioSource.clip : null);
            return;
        }

        if (audioSource == null)
        {
            Debug.LogError("MusicManager: AudioSource is missing!");
            return;
        }

        if (audioSource.clip == null)
        {
            Debug.LogError("MusicManager: No Audio Clip assigned to the Audio Source!");
            return;
        }

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
