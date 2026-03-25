using UnityEngine;

// This ensures there's an AudioSource attached to the same GameObject
[RequireComponent(typeof(AudioSource))] 
public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    [SerializeField] private AudioSource audioSource;

    void Awake()
    {
        // Check if an instance of the MusicManager already exists
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject); // Uncomment to keep music playing across scenes
        }
        else
        {
            Destroy(gameObject);
            return; // Stop running this Awake function if we are destroying the object
        }

        // Grab the AudioSource component attached to this object
        audioSource = GetComponent<AudioSource>();
    }
    
    /// Call this function to start the background music.
    public void PlayMusic()
    {
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
            Debug.Log($"Background music '{audioSource.clip.name}' started playing successfully.");
        }
    }
}