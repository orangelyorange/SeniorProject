using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

/// <summary>
/// MenuController handles main menu functionality including:
/// - Volume control and audio settings
/// - Game navigation (New Game, Load Game, Options, Exit)
/// - Settings persistence with PlayerPrefs
/// - Confirmation dialogs for user actions
/// </summary>
public class MenuController : MonoBehaviour
{
    [Header("=== Volume Settings ===")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 0.5f;

    [Header("=== Audio Sources ===")]
    [SerializeField] private AudioSource backgroundMusicSource = null; // Main menu background music
    [SerializeField] private AudioSource sfxSource = null; // Sound effects source

    [Header("=== UI Panels & Dialogs ===")]
    [SerializeField] private GameObject confirmationPrompt = null;
    [SerializeField] private GameObject noSavedGameDialog = null;

    [Header("=== Scene Settings ===")]
    [SerializeField] private string _newGameLevel = "Tutorial";
    private string levelToLoad;

    // Constants for PlayerPrefs keys
    private const string MASTER_VOLUME_KEY = "masterVolume";
    private const string SAVED_LEVEL_KEY = "SavedLevel";

    private void Start()
    {
        // Initialize UI and load saved settings
        InitializeAudioSettings();
        ValidateUI();
    }

    /// <summary>
    /// Initializes audio settings and restores saved volume from PlayerPrefs.
    /// </summary>
    private void InitializeAudioSettings()
    {
        // Load saved volume or use default
        float savedVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, defaultVolume);

        // Set AudioListener volume
        AudioListener.volume = savedVolume;

        // Update UI slider and text
        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
        }

        if (volumeTextValue != null)
        {
            volumeTextValue.text = savedVolume.ToString("0.0");
        }

        Debug.Log($"Audio initialized - Volume: {savedVolume}");
    }

    /// <summary>
    /// Validates that all required UI elements are assigned.
    /// </summary>
    private void ValidateUI()
    {
        string missingElements = "";

        if (volumeTextValue == null) missingElements += "- Volume Text Value\n";
        if (volumeSlider == null) missingElements += "- Volume Slider\n";
        if (confirmationPrompt == null) missingElements += "- Confirmation Prompt\n";
        if (noSavedGameDialog == null) missingElements += "- No Saved Game Dialog\n";
        if (backgroundMusicSource == null) missingElements += "- Background Music Source\n";

        if (!string.IsNullOrEmpty(missingElements))
        {
            Debug.LogError("MenuController is missing required UI elements:\n" + missingElements +
                "Please assign all elements in the Inspector.");
        }
    }

    #region === Game Flow Methods ===

    /// <summary>
    /// Handles "New Game" confirmation - Yes button.
    /// Loads the new game level.
    /// </summary>
    public void NewGameDialogYes()
    {
        Debug.Log($"Starting new game: {_newGameLevel}");
        SceneManager.LoadScene(_newGameLevel);
    }

    /// <summary>
    /// Handles "Load Game" confirmation - Yes button.
    /// Loads the saved level if it exists, otherwise shows no saved game dialog.
    /// </summary>
    public void LoadGameDialogYes()
    {
        if (PlayerPrefs.HasKey(SAVED_LEVEL_KEY))
        {
            levelToLoad = PlayerPrefs.GetString(SAVED_LEVEL_KEY);
            Debug.Log($"Loading saved level: {levelToLoad}");
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            Debug.LogWarning("No saved game found!");
            if (noSavedGameDialog != null)
            {
                noSavedGameDialog.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Exits the application.
    /// </summary>
    public void ExitButton()
    {
        Debug.Log("Exiting game...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    #endregion

    #region === Volume Control Methods ===

    /// <summary>
    /// Updates the volume in real-time as the slider moves.
    /// Called by the slider's OnValueChanged event.
    /// </summary>
    /// <param name="volume">Volume value from slider (0.0 to 1.0)</param>
    public void SetVolume(float volume)
    {
        // Validate inputs
        if (volumeSlider == null || volumeTextValue == null)
        {
            Debug.LogError("Volume UI elements are not assigned!");
            return;
        }

        // Set the master volume
        AudioListener.volume = Mathf.Clamp01(volume); // Clamp between 0 and 1

        // Update the text display
        volumeTextValue.text = volume.ToString("0.0");

        Debug.Log($"Volume changed to: {volume:F1}");
    }

    /// <summary>
    /// Applies volume settings and saves them to PlayerPrefs.
    /// Called by the "Apply" button in the volume settings panel.
    /// </summary>
    public void VolumeApply()
    {
        // Save current volume to PlayerPrefs
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, AudioListener.volume);
        PlayerPrefs.Save(); // Ensure data is written immediately

        Debug.Log($"Volume saved: {AudioListener.volume}");

        // Show confirmation box
        StartCoroutine(ConfirmationBox());
    }

    /// <summary>
    /// Resets audio settings to default values.
    /// Called by the "Reset" button in the volume settings panel.
    /// </summary>
    public void ResetButton(string menuType)
    {
        if (menuType == "Audio")
        {
            // Reset to default volume
            AudioListener.volume = defaultVolume;

            // Update slider value (this triggers SetVolume via OnValueChanged)
            if (volumeSlider != null)
            {
                volumeSlider.value = defaultVolume;
            }

            // Update text display
            if (volumeTextValue != null)
            {
                volumeTextValue.text = defaultVolume.ToString("0.0");
            }

            // Save the reset volume
            PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, defaultVolume);
            PlayerPrefs.Save();

            Debug.Log("Audio settings reset to default");

            // Show confirmation
            StartCoroutine(ConfirmationBox());
        }
    }

    #endregion

    #region === UI Feedback Methods ===

    /// <summary>
    /// Displays a confirmation message temporarily.
    /// </summary>
    private IEnumerator ConfirmationBox()
    {
        if (confirmationPrompt != null)
        {
            confirmationPrompt.SetActive(true);
            yield return new WaitForSeconds(2f);
            confirmationPrompt.SetActive(false);
        }
    }

    #endregion
}


/*using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;


public class MenuController : MonoBehaviour 
{
    [Header("Volume Setting")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 0.5f;

    [Header("Confirmation")]
    [SerializeField] private GameObject confirmationPrompt = null;

    [Header("Levels to Load")]
    //the thing we might load or run at any point when create new game
    public string _newGameLevel;
    //load level when we need it
    private string levelToLoad;
    [SerializeField] private GameObject noSavedGameDialog = null;

    //control when i click "yes"
    public void NewGameDialogYes()
    {
        SceneManager.LoadScene(_newGameLevel);
    }
    public void LoadGameDialogYes()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel");
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            noSavedGameDialog.SetActive(true);
        }

    }

    //exits game
    public void ExitButton()
    {
        Application.Quit();
    }

    //volume control
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0.0");
    }

    //apply volume settings and save them to player prefs
    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ConfirmationBox());
    }

    //reset button for audio settings
    public void ResetButton(string MenuType)
    {
        if  (MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;

            volumeSlider.text = defaultVolume.ToString("0.0");
            volumeTextValue.text = defaultVolume.ToString("0.0");
            VolumeApply();
        }   
    }

    //confirmation box for saving settings
    public IEnumerator ConfirmationBox() 
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        confirmationPrompt.SetActive(false);
    }

} */
