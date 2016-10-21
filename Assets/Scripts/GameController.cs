using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class GameController : MonoBehaviour
{

    public AudioMixer masterMixer;

    [Range(-80,20)]
    public float musicVolume = 80f;
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

    void Start()
    {
        InitializeSounds();
    }

    void Update()
    {
        UpdateSounds();
    }

    private void InitializeSounds()
    {
        float mmMusicVolume;
        masterMixer.GetFloat("musicVolume", out mmMusicVolume);
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

        musicVolume = currentPlayerPickupVolume;
        ambientVolume = currentAmbientVolume;
        enemyVolume = currentEnemyVolume;
        playerInteractionVolume = currentPlayerInteractionVolume;
        playerPickupVolume = currentPlayerPickupVolume;
        playerVolume = currentPlayerVolume;
    }

    private void UpdateSounds()
    {
        if (musicVolume != currentMusicVolume) SetMusicVolume(musicVolume);
        if (ambientVolume != currentAmbientVolume) SetAmbientVolume(ambientVolume);
        if (enemyVolume != currentEnemyVolume) SetEnemyVolume(enemyVolume);
        if (playerInteractionVolume != currentPlayerInteractionVolume) SetPlayerInteractionVolume(playerInteractionVolume);
        if (playerPickupVolume != currentPlayerPickupVolume) SetPlayerPickupVolume(playerPickupVolume);
        if (playerVolume != currentPlayerVolume) SetPlayerVolume(playerVolume);
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
}
