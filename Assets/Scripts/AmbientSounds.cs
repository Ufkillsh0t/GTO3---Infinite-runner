using UnityEngine;
using System.Collections;

public class AmbientSounds : MonoBehaviour
{

    //private PlayerScript player;

    //Ambient sounds
    public AudioClip[] ambientSounds;

    //AudioSource used for ambientSounds;
    private AudioSource ambient;

    public int ambientChance = 500;

    void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        ambient = gameObject.GetComponent<AudioSource>();
        //ambient.volume = PlayerPrefs.GetFloat(PrefKeys.AmbientVolume.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if (!ambient.isPlaying)
        {
            if (Random.Range(0, ambientChance) == 0) PlayRandomAmbientSound();
        }
    }

    private void PlayRandomAmbientSound()
    {
        if (ambient != null && ambientSounds != null && ambientSounds.Length > 0)
        {
            ambient.PlayOneShot(ambientSounds[Random.Range(0, ambientSounds.Length)]);
        }
        else
        {
            Debug.Log("The audiosource could not be found or there are no ambientsound avaible!");
        }
    }
}
