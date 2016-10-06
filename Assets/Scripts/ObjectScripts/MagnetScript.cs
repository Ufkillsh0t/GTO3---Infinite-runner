using UnityEngine;
using System.Collections;
using System;

public class MagnetScript : MonoBehaviour, IResource
{

    private static PlayerScript player;
    private Vector3 startPosition;
    private float coliderScale = 2f;
    private float duration = 12f;
    public ResourceType resourceType = ResourceType.Magnet;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
    }

    // Use this for initialization
    void Start()
    {
        if (player == null) Debug.LogWarning("Player script could not be found!");
    }

    void OnTriggerEnter(Collider other)
    {
        PickUp();
    }

    public void PickUp()
    {
        transform.position = startPosition;
        if (!player.magnetUsed)
        {
            player.magnetUsed = true;
        }
        player.MagnetDuration = duration;
        Debug.Log("I got triggered!");
    }

    public ResourceType GetResourceType()
    {
        return resourceType;
    }
}
