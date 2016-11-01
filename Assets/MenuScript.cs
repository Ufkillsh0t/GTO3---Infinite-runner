using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public Text highScoreText;
    public Slider masterVolumeSlider;
    public Slider ambientSoundVolumeSlider;
    public Slider playerVolumeSlider;
    public Toggle slideToggle;

    public float masterVolume;
    public float ambientVolume;
    public float playerVolume;
    public int highscore = -1;
    public bool toggled;


    void Start()
    {
        if (highScoreText != null) GetHighScoreText();
        if (masterVolumeSlider != null) GetMasterVolume();
        if (ambientSoundVolumeSlider != null) GetAmbientVolume();
        if (playerVolumeSlider != null) GetPlayerVolume();
        if (slideToggle != null) GetSlideToggle();
    }


    void Update()
    {
        CheckVolumeChange();
    }

    public void CheckVolumeChange()
    {
        if (masterVolumeSlider != null && masterVolumeSlider.value != masterVolume) UpdateMasterVolume();
        if (ambientSoundVolumeSlider != null && ambientSoundVolumeSlider.value != masterVolume) UpdateAmbientVolume();
        if (playerVolumeSlider != null && playerVolumeSlider.value != masterVolume) UpdatePlayerVolume();
        if (slideToggle != null && slideToggle.isOn != toggled) UpdateSlideToggle();
    }

    public void LoadScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void UpdateMasterVolume()
    {
        masterVolume = masterVolumeSlider.value;
        PlayerPrefs.SetFloat(PrefKeys.MasterVolume.ToString(), masterVolume);
        PlayerPrefs.Save();
    }

    public void UpdateAmbientVolume()
    {
        ambientVolume = ambientSoundVolumeSlider.value;
        PlayerPrefs.SetFloat(PrefKeys.AmbientVolume.ToString(), ambientVolume);
        PlayerPrefs.Save();
    }

    public void UpdatePlayerVolume()
    {
        playerVolume = playerVolumeSlider.value;
        PlayerPrefs.SetFloat(PrefKeys.PlayerVolume.ToString(), playerVolume);
        PlayerPrefs.Save();
    }

    public void UpdateSlideToggle()
    {
        toggled = slideToggle.isOn;
        PlayerPrefs.SetInt(PrefKeys.SlideMovement.ToString(), (toggled == true) ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void GetHighScoreText()
    {
        if (PlayerPrefs.HasKey(PrefKeys.Highscore.ToString()))
        {
            highscore = PlayerPrefs.GetInt("Highscore");
            highScoreText.text = highScoreText.text + highscore.ToString();
        }
        else
        {
            PlayerPrefs.SetInt(PrefKeys.Highscore.ToString(), 0);
            PlayerPrefs.Save();
            highscore = PlayerPrefs.GetInt("Highscore");
            highScoreText.text = highScoreText.text + highscore.ToString();
        }
    }

    private void GetSlideToggle()
    {
        if (PlayerPrefs.HasKey(PrefKeys.SlideMovement.ToString()))
        {
            toggled = (PlayerPrefs.GetInt(PrefKeys.SlideMovement.ToString()) == 1) ? true : false;
            slideToggle.isOn = toggled;
            
        }
        else
        {
            PlayerPrefs.SetInt(PrefKeys.SlideMovement.ToString(), 1);
            PlayerPrefs.Save();
            toggled = (PlayerPrefs.GetInt(PrefKeys.SlideMovement.ToString()) == 1) ? true : false;
            slideToggle.isOn = toggled;
        }
    }

    public void GetMasterVolume()
    {
        if (PlayerPrefs.HasKey(PrefKeys.MasterVolume.ToString()))
        {
            masterVolume = PlayerPrefs.GetFloat(PrefKeys.MasterVolume.ToString());
            masterVolumeSlider.value = masterVolume;
        }
        else
        {
            PlayerPrefs.SetFloat(PrefKeys.MasterVolume.ToString(), 1);
            masterVolume = PlayerPrefs.GetFloat(PrefKeys.MasterVolume.ToString());
            masterVolumeSlider.value = masterVolume;
            PlayerPrefs.Save();
        }
    }

    public void GetAmbientVolume()
    {
        if (PlayerPrefs.HasKey(PrefKeys.AmbientVolume.ToString()))
        {
            ambientVolume = PlayerPrefs.GetFloat(PrefKeys.AmbientVolume.ToString());
            ambientSoundVolumeSlider.value = ambientVolume;
        }
        else
        {
            PlayerPrefs.SetFloat(PrefKeys.AmbientVolume.ToString(), 1);
            ambientVolume = PlayerPrefs.GetFloat(PrefKeys.AmbientVolume.ToString());
            ambientSoundVolumeSlider.value = ambientVolume;
            PlayerPrefs.Save();
        }
    }

    public void GetPlayerVolume()
    {
        if (PlayerPrefs.HasKey(PrefKeys.PlayerVolume.ToString()))
        {
            playerVolume = PlayerPrefs.GetFloat(PrefKeys.PlayerVolume.ToString());
            playerVolumeSlider.value = playerVolume;
        }
        else
        {
            PlayerPrefs.SetFloat(PrefKeys.PlayerVolume.ToString(), 1);
            playerVolume = PlayerPrefs.GetFloat(PrefKeys.PlayerVolume.ToString());
            playerVolumeSlider.value = playerVolume;
            PlayerPrefs.Save();
        }
    }
}
