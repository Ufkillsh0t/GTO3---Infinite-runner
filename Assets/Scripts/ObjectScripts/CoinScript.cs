using UnityEngine;
using System.Collections;

public class CoinScript : MonoBehaviour {

    public int coinValue = 1;

	// Use this for initialization
	void Start () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("I got triggered!");
    }
}
