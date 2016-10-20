using UnityEngine;
using System.Collections;


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
    //Sound when you almost fall of the edge.
    public AudioClip[] nearEdgeSounds;

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

    // Use this for initialization
    void Start()
    {
        AudioSource[] sources = gameObject.GetComponents<AudioSource>();
        if (sources[0] != null)
        {
            playerMovement = sources[0];
            playerMovement.loop = false;
        }
        else
        {
            Debug.Log("Audiosource 0 missing no playermovement sound!");
        }
        if (sources[0] != null)
        {
            playerInteraction = sources[1];
            playerInteraction.loop = false;
        }
        else
        {
            Debug.Log("Audiosource 1 missing no interaction sound!");
        }
        if (sources[0] != null)
        {
            playerPickup = sources[2];
            playerPickup.loop = false;
        }
        else
        {
            Debug.Log("Audiosource 2 missing no pickup sound!");
        }
    }

    public void PlayFootSteps()
    {
        if (playerMovement != null && footStepsSounds != null && footStepsSounds.Length > 0)
        {
            playerMovement.clip = footStepsSounds[Random.Range(0, (footStepsSounds.Length - 1))];
            playerMovement.Play();
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
            playerMovement.clip = jumpSounds[Random.Range(0, (footStepsSounds.Length - 1))];
            playerMovement.Play();
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
            playerInteraction.clip = interactionSounds[Random.Range(0, (interactionSounds.Length - 1))];
            playerInteraction.Play();
        }
        else
        {
            Debug.Log("Triggered PlayInteraction but no audioSource or interaction clips could be found");
        }
    }

    public void PlayPickup(PickUpObject po)
    {
        if(playerPickup != null && pickupSounds != null && pickupSounds.Length > 0)
        {
            switch (po)
            {
                case PickUpObject.Coin:
                    playerPickup.clip = pickupSounds[(int)PickUpObject.Coin];
                    break;
                case PickUpObject.Magnet:
                    playerPickup.clip = pickupSounds[(int)PickUpObject.Magnet];
                    break;             
            }
            playerPickup.Play();
        }
        else
        {
            Debug.Log("Triggered PlayPickup but no audioSource or pickup clips could be found");
        }
    }

}

