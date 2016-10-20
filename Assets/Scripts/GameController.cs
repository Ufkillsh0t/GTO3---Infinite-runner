using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class GameController : MonoBehaviour {

    public AudioMixer masterMixer;


    public float musicVolume = 80f;
    private float currentMusicVolume;



    public void SetMusicLvl(float musicLvl)
    {
        masterMixer.SetFloat("ambientVolume", musicLvl);
    }


}
