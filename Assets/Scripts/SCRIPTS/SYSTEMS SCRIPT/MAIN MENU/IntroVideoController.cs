using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroVideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    public string mainMenuSceneName = "Main Menu";

    void Start()
    {
        // Crucial Check: Ensure the VideoPlayer is assigned and has a source.
        if (videoPlayer == null || videoPlayer.source != VideoSource.VideoClip)
        {
            // The source must be set to 'VideoClip' and a file must be assigned in the Inspector.
            Debug.LogError("VideoPlayer component is not assigned or is missing an assigned VideoClip (e.g., your H.264 MP4 file). Loading Main Menu immediately.");
            LoadMainMenu();
            return;
        }

        // Subscribe to the event that fires when the video finishes playing
        videoPlayer.loopPointReached += OnVideoFinished;

        // Start playing the assigned H.264 video clip
        videoPlayer.Play();
        Debug.Log("Playing H.264 intro video...");
    }

    /// Called when the video naturally reaches its end.
    void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Intro video finished playing naturally. Loading main menu...");
        LoadMainMenu();
    }

    /// The core function to switch to the next scene.
    void LoadMainMenu()
    {
        // Ensure we clean up the subscription before loading the next scene
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }

        if (!Application.CanStreamedLevelBeLoaded(mainMenuSceneName))
        {
            Debug.LogError($"Scene '{mainMenuSceneName}' can't be loaded. Make sure it's added to Build Settings (Scenes In Build) and the name matches the scene asset exactly (e.g., 'Main Menu').");
            return;
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }
}