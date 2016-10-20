using UnityEngine;
using System.Collections;

public class CoinScript : MonoBehaviour {

    private static PlayerScript player;
    private Vector3 startPosition;
    public int coinValue = 1;
    private PickUpObject po = PickUpObject.Coin;

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
        transform.position = startPosition;
        player.collectedCoins++;
        player.pss.PlayPickup(po);
        Debug.Log("I got triggered!");
    }
}
