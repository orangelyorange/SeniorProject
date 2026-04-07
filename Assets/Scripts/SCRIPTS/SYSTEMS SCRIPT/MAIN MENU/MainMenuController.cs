using UnityEngine;
using UnityEngine.UI; // Required for interacting with UI Buttons
using UnityEngine.SceneManagement; 

public class MainMenuController : MonoBehaviour
{
    [Header("UI References")]
    public Button continueButton;

    void Start()
    {
        // When the Main Menu loads, check if our SaveManager exists
        if (SaveLoadManager.Instance != null)
        {
            // If there is NO save file, this turns the button off (grays it out)
            // If there IS a save file, it turns the button on
            continueButton.interactable = SaveLoadManager.Instance.HasSaveFile();
        }
    }

    // Link your "Continue" or "Load Game" button to this method
    public void OnContinueClicked()
    {
        if (SaveLoadManager.Instance != null)
        {
            // This triggers the immortal manager to read the file and load the saved scene!
            SaveLoadManager.Instance.LoadGame();
        }
    }

    // Link your "New Game" button to this method
    public void OnNewGameClicked()
    {
        // Replace "Level_1" with the exact name of your first game scene
        SceneManager.LoadScene("Level_1"); 
    }
}