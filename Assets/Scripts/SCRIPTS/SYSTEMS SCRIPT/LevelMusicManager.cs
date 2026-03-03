using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))] // Automatically adds an AudioSource to the GameObject
public class LevelMusicManager : MonoBehaviour
{
    private AudioSource levelTrack;
    
    [Header("Fade Settings")]
    public float fadeDuration = 2.0f; // How many seconds it takes to reach full volume
    public float maxVolume = 1.0f;    // The maximum volume for this specific track (0.0 to 1.0)

    void Start()
    {
        // Grab the AudioSource component on this object
        levelTrack = GetComponent<AudioSource>();

        // Ensure the music starts at zero volume
        levelTrack.volume = 0f;
        
        // Start playing the track silently
        levelTrack.Play();

        // Begin the fade-in sequence
        StartCoroutine(FadeInMusic());
    }

    private IEnumerator FadeInMusic()
    {
        float currentTime = 0f;

        // Gradually increase the volume until the fade duration is reached
        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            
            // Mathf.Lerp smoothly transitions a number from 0 to your maxVolume
            levelTrack.volume = Mathf.Lerp(0f, maxVolume, currentTime / fadeDuration);
            
            yield return null; // Wait for the next frame
        }

        // Ensure the volume perfectly hits the target at the very end
        levelTrack.volume = maxVolume;
    }
}