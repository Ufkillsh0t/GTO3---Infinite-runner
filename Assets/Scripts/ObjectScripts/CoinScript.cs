using UnityEngine;
using System.Collections;

public class CoinScript : MonoBehaviour {

    private static PlayerScript player;
    private Vector3 startPosition;
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
        transform.position = startPosition;
        player.collectedCoins++;
        Debug.Log("I got triggered!");
    }
}
