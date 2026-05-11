using UnityEngine;
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
    public string _newGameLevel = "LEVEL0"; // fallback gameplay level
    private string levelToLoad;
    [SerializeField] private GameObject noSavedGameDialog = null;

    [Header("Cutscene Settings")]
    [SerializeField] private string prologueScene = "Prologue_Cutscene";

    // 🎬 NEW GAME FLOW (PROLOGUE FIRST)
    public void NewGameDialogYes()
    {
        Debug.Log("New Game → Loading Prologue First");
        StartCoroutine(NewGameFlow());
    }

    private IEnumerator NewGameFlow()
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(prologueScene);
    }

    // LOAD SAVED GAME
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

    // EXIT GAME
    public void ExitButton()
    {
        Application.Quit();
    }

    // VOLUME CONTROL
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0.0");
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ConfirmationBox());
    }

    public void ResetButton(string MenuType)
    {
        if (MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0.0");
            VolumeApply();
        }
    }

    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        confirmationPrompt.SetActive(false);
    }
}