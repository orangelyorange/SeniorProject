using UnityEngine;

// This ensures there's an AudioSource attached to the same GameObject
[RequireComponent(typeof(AudioSource))] 
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
            // Reset the global master volume to 100%
            AudioListener.volume = 1f;
            
            audioSource.Play();
            Debug.Log($"Background music '{audioSource.clip.name}' started playing successfully.");
            
            // Diagnostic logs to check for runtime volume or pitch issues
            Debug.Log($"Diagnostics: Source Volume = {audioSource.volume}, Mute = {audioSource.mute}, Pitch = {audioSource.pitch}, Spatial Blend = {audioSource.spatialBlend}");
            Debug.Log($"Diagnostics: Global AudioListener Volume = {AudioListener.volume}");
            
            if (Time.timeScale == 0)
            {
                Debug.LogWarning("Diagnostics: Time.timeScale is 0! Standard audio playback will be paused.");
            }
        }
    }
}