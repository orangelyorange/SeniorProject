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

}
