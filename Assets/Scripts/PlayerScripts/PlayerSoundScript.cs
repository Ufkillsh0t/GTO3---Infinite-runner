using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public enum PickUpObject
{
    Coin,
    Magnet
}


public class PlayerSoundScript : MonoBehaviour
{

    //Different footstep sounds
    public AudioClip[] footStepsSounds;
    //Jumping sounds
    public AudioClip[] jumpSounds;
    //Jump landing
    public AudioClip[] jumpLandingSounds;


    //Talking, Couching, Puffing, Screaming and Cheering for example.
    public AudioClip[] interactionSounds;
    //Dying
    public AudioClip[] deathSounds;
    //Pickup sounds
    public AudioClip[] pickupSounds;


    //The players current movement sound, think of a footstep
    public AudioSource playerMovement;
    //Think off puffing/the player talking
    public AudioSource playerInteraction;
    //Picking up coins
    public AudioSource playerPickup;

    //Voor de hoofd audio mixer;
    public AudioMixerGroup amg;

    //GameController;
    private GameController gc;

    private bool triggeredDeath;

    // Use this for initialization
    void Start()
    {
        AudioSource[] sources = gameObject.GetComponents<AudioSource>();
        if (sources[0] != null)
        {
            playerMovement = sources[0];
            playerMovement.loop = false;
            //playerMovement.volume = PlayerPrefs.GetFloat(PrefKeys.PlayerVolume.ToString());
        }
        else
        {
            Debug.Log("Audiosource 0 missing no playermovement sound!");
        }
        if (sources[1] != null)
        {
            playerInteraction = sources[1];
            playerInteraction.loop = false;
            //playerInteraction.volume = PlayerPrefs.GetFloat(PrefKeys.PlayerVolume.ToString());
        }
        else
        {
            Debug.Log("Audiosource 1 missing no interaction sound!");
        }
        if (sources[2] != null)
        {
            playerPickup = sources[2];
            playerPickup.loop = false;
            //playerPickup.volume = PlayerPrefs.GetFloat(PrefKeys.PlayerVolume.ToString());
        }
        else
        {
            Debug.Log("Audiosource 2 missing no pickup sound!");
        }

        gc = GameController.Instance;

        triggeredDeath = false;
    }

    public void PlayFootSteps()
    {
        if (playerMovement != null && footStepsSounds != null && footStepsSounds.Length > 0)
        {
            playerMovement.PlayOneShot(footStepsSounds[Random.Range(0, (deathSounds.Length))]);
        }
        else
        {
            Debug.Log("Triggered PlayFootSteps but no audioSource or footStep clips could be found");
        }
    }

    public void PlayJump()
    {
        if (playerMovement != null && jumpSounds != null && jumpSounds.Length > 0)
        {
            playerMovement.PlayOneShot(jumpSounds[Random.Range(0, (deathSounds.Length))]);
        }
        else
        {
            Debug.Log("Triggered PlayFootSteps but no audioSource or jump clips could be found");
        }
    }

    public void PlayInteraction()
    {
        if (playerInteraction != null && interactionSounds != null && interactionSounds.Length > 0)
        {
            playerInteraction.PlayOneShot(interactionSounds[Random.Range(0, (deathSounds.Length))]);
        }
        else
        {
            Debug.Log("Triggered PlayInteraction but no audioSource or interaction clips could be found");
        }
    }

    public void PlayPickup(PickUpObject po)
    {
        if (playerPickup != null && pickupSounds != null && pickupSounds.Length > 0)
        {
            switch (po)
            {
                case PickUpObject.Coin:
                    playerPickup.PlayOneShot(pickupSounds[(int)PickUpObject.Coin]);
                    break;
                case PickUpObject.Magnet:
                    playerPickup.PlayOneShot(pickupSounds[(int)PickUpObject.Magnet]);
                    break;
            }
        }
        else
        {
            Debug.Log("Triggered PlayPickup but no audioSource or pickup clips could be found");
        }
    }

    public void PlayDeathSound(bool instantDeath)
    {
        if (playerInteraction != null && deathSounds != null && deathSounds.Length > 0)
        {
            if (!triggeredDeath && !instantDeath)
            {
                StartCoroutine(DelayedLoad());
                triggeredDeath = true;
            }
            else
            {
                gc.Death();
                playerInteraction.clip = deathSounds[Random.Range(0, (deathSounds.Length))];
                playerInteraction.Play();
            }
        }
        else
        {
            Debug.Log("Triggered PlayInteraction but no audioSource or interaction clips could be found");
        }
    }

    public void PlayLanding()
    {
        if (playerMovement != null && jumpLandingSounds != null && jumpLandingSounds.Length > 0)
        {
            playerMovement.PlayOneShot(jumpLandingSounds[Random.Range(0, (jumpLandingSounds.Length))]);
        }
        else
        {
            Debug.Log("Triggered PlayFootSteps but no audioSource or jump clips could be found");
        }
    }

    IEnumerator DelayedLoad()
    {
        playerInteraction.clip = deathSounds[Random.Range(0, (deathSounds.Length))];
        playerInteraction.Play();

        yield return new WaitForSeconds((playerInteraction.clip.length / 2));

        gc.Death();
    }

}

