using UnityEngine;
using System.Collections;

public class MagnetScript : MonoBehaviour
{

    private static PlayerScript player;
    private Vector3 startPosition;
    private float duration = 12f;
    private PickUpObject po = PickUpObject.Magnet;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        startPosition = transform.position;
    }

    // Use this for initialization
    void Start()
    {
        if (player == null) Debug.LogWarning("Player script could not be found!");
    }

    void OnTriggerEnter(Collider other)
    {
        transform.position = startPosition;
        if (!player.magnetUsed)
        {
            player.magnetUsed = true;
        }
        player.MagnetDuration = duration;
        player.pss.PlayPickup(po);
        Debug.Log("I got triggered!");
    }
}
