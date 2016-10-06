using UnityEngine;
using System.Collections;
using System;

public class CoinScript : MonoBehaviour, IResource {

    private static PlayerScript player;
    private Vector3 startPosition;
    public ResourceType resourceType = ResourceType.Coin;
    public int coinValue = 1;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        startPosition = transform.position;
    }

	// Use this for initialization
	void Start () {
        if (player == null) Debug.LogWarning("Player script could not be found!");
	}

    void OnTriggerEnter(Collider other)
    {
        PickUp();
    }

    public void PickUp()
    {
        transform.position = startPosition;
        player.collectedCoins++;
        Debug.Log("I got triggered!");
    }

    public ResourceType GetResourceType()
    {
        return resourceType;
    }
}
