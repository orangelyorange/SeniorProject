using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

/// <summary>
/// MenuController handles main menu functionality including:
/// - Volume control and audio settings with automatic event wiring
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
    [SerializeField] private AudioSource backgroundMusicSource = null;
    [SerializeField] private AudioSource sfxSource = null;

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
        Debug.Log("[MenuController] Starting initialization...");

        // Validate first
        ValidateUI();

        // Initialize audio settings
        InitializeAudioSettings();

        // CRITICAL: Wire up slider events (works even if not connected in Inspector)
        WireUpSliderEvents();

        Debug.Log("[MenuController] Initialization complete!");
    }

    /// <summary>
    /// Validates that all required UI elements are assigned.
    /// </summary>
    private void ValidateUI()
    {
        Debug.Log("[ValidateUI] Starting validation...");
        string missingElements = "";

        if (volumeTextValue == null) missingElements += "- Volume Text Value\n";
        if (volumeSlider == null) missingElements += "- Volume Slider\n";
        if (confirmationPrompt == null) missingElements += "- Confirmation Prompt\n";
        if (noSavedGameDialog == null) missingElements += "- No Saved Game Dialog\n";
        if (backgroundMusicSource == null) missingElements += "- Background Music Source\n";

        if (!string.IsNullOrEmpty(missingElements))
        {
            Debug.LogError("[MenuController] X CRITICAL - Missing UI elements:\n" + missingElements);
        }
        else
        {
            Debug.Log("[MenuController] v/ All UI elements validated");
        }
    }

    /// <summary>
    /// Initializes audio settings and restores saved volume from PlayerPrefs.
    /// </summary>
    private void InitializeAudioSettings()
    {
        Debug.Log("[InitializeAudioSettings] Starting...");

        // Load saved volume or use default
        float savedVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, defaultVolume);
        Debug.Log($"Loaded volume from PlayerPrefs: {savedVolume}");

        // Set AudioListener volume (CRITICAL - this is what controls audio globally)
        AudioListener.volume = Mathf.Clamp01(savedVolume);
        Debug.Log($"AudioListener.volume set to: {AudioListener.volume}");

        // Update UI slider
        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            Debug.Log($"Slider value set to: {savedVolume}");
        }

        // Update text display
        if (volumeTextValue != null)
        {
            UpdateVolumeText(savedVolume);
            Debug.Log($"Volume text updated");
        }

        // Verify audio is playing
        if (backgroundMusicSource != null)
        {
            if (backgroundMusicSource.clip != null)
            {
                if (!backgroundMusicSource.isPlaying)
                {
                    Debug.Log("[InitializeAudioSettings] Starting background music...");
                    backgroundMusicSource.Play();
                }
                Debug.Log($"Background music: {backgroundMusicSource.clip.name} - Playing: {backgroundMusicSource.isPlaying}");
            }
            else
            {
                Debug.LogError("[InitializeAudioSettings] X CRITICAL - Background Music Source has NO AudioClip assigned!");
            }
        }

        Debug.Log("[InitializeAudioSettings] Complete");
    }

    /// <summary>
    /// Wires up slider events in code (fallback if not done in Inspector).
    /// This is CRUCIAL to ensure the slider works!
    /// </summary>
    private void WireUpSliderEvents()
    {
        if (volumeSlider == null)
        {
            Debug.LogError("[WireUpSliderEvents] X Slider is NULL - cannot wire events!");
            return;
        }

        Debug.Log("[WireUpSliderEvents] Wiring slider OnValueChanged event...");

        // Remove any existing listeners (prevents duplicate calls)
        volumeSlider.onValueChanged.RemoveListener(SetVolume);

        // Add the listener
        volumeSlider.onValueChanged.AddListener(SetVolume);

        Debug.Log("[WireUpSliderEvents] v/ Slider event wired successfully");
    }

    #region === Volume Control Methods ===

    /// <summary>
    /// Updates the volume in real-time as the slider moves.
    /// This is called by Slider's OnValueChanged event.
    /// </summary>
    public void SetVolume(float volume)
    {
        Debug.Log($"[SetVolume] Called with value: {volume}");

        // Validate
        if (volumeSlider == null || volumeTextValue == null)
        {
            Debug.LogError("[SetVolume] X UI elements are NULL!");
            return;
        }

        // Clamp volume to 0-1 range
        float clampedVolume = Mathf.Clamp01(volume);

        // CRITICAL: Set the master volume
        AudioListener.volume = clampedVolume;
        Debug.Log($"[SetVolume] AudioListener.volume = {clampedVolume}");

        // Update text display
        UpdateVolumeText(clampedVolume);
    }

    /// <summary>
    /// Helper method to update volume text display.
    /// </summary>
    private void UpdateVolumeText(float volume)
    {
        if (volumeTextValue == null) return;

        // Convert to percentage (0-100)
        int percentageValue = Mathf.RoundToInt(volume * 100f);
        volumeTextValue.text = percentageValue.ToString() + "%";

        Debug.Log($"[UpdateVolumeText] Volume text updated to: {percentageValue}%");
    }

    /// <summary>
    /// Applies volume settings and saves them to PlayerPrefs.
    /// </summary>
    public void VolumeApply()
    {
        Debug.Log("[VolumeApply] Saving volume settings...");

        // Save current volume
        float currentVolume = AudioListener.volume;
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, currentVolume);
        PlayerPrefs.Save();

        Debug.Log($"[VolumeApply] v/ Volume saved: {currentVolume}");

        // Show confirmation
        StartCoroutine(ConfirmationBox());
    }

    /// <summary>
    /// Resets audio settings to default values.
    /// </summary>
    public void ResetButton(string menuType)
    {
        if (menuType != "Audio") return;

        Debug.Log("[ResetButton] Resetting audio to default...");

        // Reset AudioListener
        AudioListener.volume = defaultVolume;
        Debug.Log($"AudioListener.volume reset to: {defaultVolume}");

        // Update slider (will trigger SetVolume)
        if (volumeSlider != null)
        {
            volumeSlider.value = defaultVolume;
            Debug.Log($"Slider reset to: {defaultVolume}");
        }

        // Update text
        UpdateVolumeText(defaultVolume);

        // Save
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, defaultVolume);
        PlayerPrefs.Save();

        Debug.Log("[ResetButton] v/ Reset complete");

        // Show confirmation
        StartCoroutine(ConfirmationBox());
    }

    #endregion

    #region === Game Flow Methods ===

    public void NewGameDialogYes()
    {
        Debug.Log($"[NewGameDialogYes] Loading scene: {_newGameLevel}");
        SceneManager.LoadScene(_newGameLevel);
    }

    public void LoadGameDialogYes()
    {
        if (PlayerPrefs.HasKey(SAVED_LEVEL_KEY))
        {
            levelToLoad = PlayerPrefs.GetString(SAVED_LEVEL_KEY);
            Debug.Log($"[LoadGameDialogYes] Loading saved level: {levelToLoad}");
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            Debug.LogWarning("[LoadGameDialogYes] No saved game found!");
            if (noSavedGameDialog != null)
            {
                noSavedGameDialog.SetActive(true);
            }
        }
    }

    public void ExitButton()
    {
        Debug.Log("[ExitButton] Exiting game...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #endregion

    #region === UI Feedback ===

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

/*
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

*/

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
