using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    //The score panel to display the score
    public GameObject scorePanel;

    //The death panel which will show once you are dead.
    public GameObject deathPanel;

    //The audiomixer which controls all the audio.
    public AudioMixer masterMixer;

    //The text of the deathPanel
    public Text deathPanelText;

    [Range(-80, 20)]
    public float masterVolume = 100f;
    private float currentMasterVolume;

    [Range(-80, 20)]
    public float musicVolume = 100f;
    private float currentMusicVolume;

    [Range(-80, 20)]
    public float ambientVolume = 100f;
    private float currentAmbientVolume;

    [Range(-80, 20)]
    public float enemyVolume = 100f;
    private float currentEnemyVolume;

    [Range(-80, 20)]
    public float playerInteractionVolume = 100f;
    private float currentPlayerInteractionVolume;

    [Range(-80, 20)]
    public float playerPickupVolume = 100f;
    private float currentPlayerPickupVolume;

    [Range(-80, 20)]
    public float playerVolume = 100f;
    private float currentPlayerVolume;


    private PlayerScript player;

    void Awake()
    {
        Time.timeScale = 1;
    }

    void Start()
    {
        if (scorePanel != null) scorePanel.SetActive(true);
        if (deathPanel != null) deathPanel.SetActive(false);


        InitializeSounds();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
    }

    void Update()
    {
        UpdateSounds();
    }

    private void InitializeSounds()
    {
        float mmMasterVolume;
        masterMixer.GetFloat("masterVolume", out mmMasterVolume);
        currentMasterVolume = mmMasterVolume;

        float mmMusicVolume;
        masterMixer.GetFloat("musicVolume", out mmMusicVolume); /*
        float pMusicVolume = PlayerPrefs.GetFloat(PrefKeys.PlayerVolume.ToString());
        if (mmMusicVolume != pMusicVolume) masterMixer.SetFloat("musicVolume", pMusicVolume);
        masterMixer.GetFloat("Master", out mmMusicVolume); */
        currentMusicVolume = mmMusicVolume;

        float mmAmbientVolume;
        masterMixer.GetFloat("ambientVolume", out mmAmbientVolume);
        currentAmbientVolume = mmAmbientVolume;

        float mmEnemyVolume;
        masterMixer.GetFloat("enemyVolume", out mmEnemyVolume);
        currentEnemyVolume = mmEnemyVolume;

        float mmPlayerInteractionVolume;
        masterMixer.GetFloat("playerInteractionVolume", out mmPlayerInteractionVolume);
        currentPlayerInteractionVolume = mmPlayerInteractionVolume;

        float mmPlayerPickupVolume;
        masterMixer.GetFloat("playerPickupVolume", out mmPlayerPickupVolume);
        currentPlayerPickupVolume = mmPlayerPickupVolume;

        float mmPlayerVolume;
        masterMixer.GetFloat("playerVolume", out mmPlayerVolume);
        currentPlayerVolume = mmMusicVolume;

        masterVolume = (PlayerPrefs.GetFloat(PrefKeys.MasterVolume.ToString()) * 100) - 80;
        musicVolume = currentPlayerPickupVolume;
        ambientVolume = (PlayerPrefs.GetFloat(PrefKeys.AmbientVolume.ToString()) * 100) - 80;
        enemyVolume = currentEnemyVolume;
        playerInteractionVolume = currentPlayerInteractionVolume;
        playerPickupVolume = currentPlayerPickupVolume;
        playerVolume = (PlayerPrefs.GetFloat(PrefKeys.PlayerVolume.ToString()) * 100) - 80;
        SetMusicVolume();
    }

    private void UpdateSounds()
    {
        SetMusicVolume();
    }

    private void SetMusicVolume()
    {
        if (masterVolume != currentMasterVolume) SetMasterVolume(masterVolume);
        if (musicVolume != currentMusicVolume) SetMusicVolume(musicVolume);
        if (ambientVolume != currentAmbientVolume) SetAmbientVolume(ambientVolume);
        if (enemyVolume != currentEnemyVolume) SetEnemyVolume(enemyVolume);
        if (playerInteractionVolume != currentPlayerInteractionVolume) SetPlayerInteractionVolume(playerInteractionVolume);
        if (playerPickupVolume != currentPlayerPickupVolume) SetPlayerPickupVolume(playerPickupVolume);
        if (playerVolume != currentPlayerVolume) SetPlayerVolume(playerVolume);
    }

    public void SetMasterVolume(float mv)
    {
        masterMixer.SetFloat("masterVolume", mv);
        currentMasterVolume = mv;
    }

    public void SetAmbientVolume(float av)
    {
        masterMixer.SetFloat("ambientVolume", av);
        currentAmbientVolume = av;
    }

    public void SetMusicVolume(float mv)
    {
        masterMixer.SetFloat("musicVolume", mv);
        currentMusicVolume = mv;
    }

    public void SetEnemyVolume(float ev)
    {
        masterMixer.SetFloat("enemyVolume", ev);
        currentEnemyVolume = ev;
    }

    public void SetPlayerInteractionVolume(float piv)
    {
        masterMixer.SetFloat("playerInteractionVolume", piv);
        currentPlayerInteractionVolume = piv;
    }

    public void SetPlayerPickupVolume(float ppv)
    {
        masterMixer.SetFloat("playerPickupVolume", ppv);
        currentPlayerPickupVolume = ppv;
    }

    public void SetPlayerVolume(float pv)
    {
        masterMixer.SetFloat("playerVolume", pv);
        currentPlayerVolume = pv;
    }

    public void Death()
    {
        int oldHighscore = PlayerPrefs.GetInt(PrefKeys.Highscore.ToString());

        if (oldHighscore < player.collectedCoins) PlayerPrefs.SetInt(PrefKeys.Highscore.ToString(), player.collectedCoins);

        int newHighscore = PlayerPrefs.GetInt(PrefKeys.Highscore.ToString());

        if (deathPanel != null)
        {
            if (deathPanelText != null) deathPanelText.text += newHighscore.ToString();
            deathPanel.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene(0);
        }

        Time.timeScale = 0;

        PlayerPrefs.Save();
    }
}
