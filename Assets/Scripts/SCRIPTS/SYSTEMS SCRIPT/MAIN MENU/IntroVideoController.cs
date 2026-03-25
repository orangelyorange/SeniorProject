using System;
using UnityEngine;
using UnityEngine.Video;

public class IntroVideoController : MonoBehaviour
{
    [Header("Video Settings")]
    public VideoPlayer videoPlayer;
    public GameObject videoScreen; 

    [Header("UI Settings")]
    [Tooltip("The parent GameObject containing all your Main Menu buttons and graphics.")]
    public GameObject mainMenuUI;

    private bool isVideoPlaying = false;

    void Start()
    {
        // Hide the Main Menu UI right as the scene loads
        if (mainMenuUI != null)
        {
            mainMenuUI.SetActive(false);
        }

        // Crucial Check: Ensure the VideoPlayer is assigned and has a source.
        if (videoPlayer == null || videoPlayer.source != VideoSource.VideoClip)
        {
            Debug.LogError("VideoPlayer is missing or has no VideoClip assigned. Skipping directly to Main Menu.");
            ShowMainMenu();
            return;
        }

        // Subscribe to the event that fires when the video finishes playing
        videoPlayer.loopPointReached += OnVideoFinished;

        // Start playing the video
        videoPlayer.Play();
        isVideoPlaying = true;
        Debug.Log("Playing intro video...");
    }

    private void Update()
    {
        //check for skip input only if the video is playing

        if (isVideoPlaying)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                Debug.Log("Playing intro video...");
                ShowMainMenu();
            }
        }
    }

    /// Called when the video naturally reaches its end.
    void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Intro video finished playing naturally. Showing main menu...");
        ShowMainMenu();
    }

    /// The core function to transition from the video to the menu UI.
    void ShowMainMenu()
    {
        // Ensure we clean up the subscription
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
            videoPlayer.Stop(); // Optional: Stop the player completely to save resources
        }

        // Hide the video screen so it doesn't block the UI
        if (videoScreen != null)
        {
            videoScreen.SetActive(false);
        }

        // Reveal the main menu
        if (mainMenuUI != null)
        {
            mainMenuUI.SetActive(true);
        }
        
        //Tell the MusicManager to start the music!
        if (MusicManager.instance != null)
        {
            MusicManager.instance.PlayMusic();
        }
    }
}